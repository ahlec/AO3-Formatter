using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AO3_Formatter
{
    public class FanficPiece
    {
        private static Regex s_horizontalLineBreak = new Regex("^(=|-){3,}$");
        private static Regex s_indentation = new Regex(@"^(\t+\s*|\s{3,})");
        private static Regex s_multipleConsecutiveWhitespaceCharacters = new Regex(@"( ){2,}");
        private static Regex s_reduceHtmlWhitespace = new Regex(@"(^|>)\s*(<|$)");

        /// <summary>
        /// This uses a lookbehind (`(?<!\*)`) to make sure there is no previous asterisk
        /// immediately before this one (eg, don't confuse * with **). Then it matches the
        /// opening asterisk and creates a capture group bound to $1 for the text inside. To
        /// finish this, it matches either a single asterisk, or the end of the line (to make
        /// sure that every open element has a close element); here we also have a lookahead to
        /// make sure that we aren't followed immediately by another asterisk.
        /// </summary>
        private static Regex s_italics = new Regex(@"(?<!\*)\*{1}([^\*]+?)(\*{1}|$)(?!\*)");
        private static Regex s_bold = new Regex(@"(?<!\*)\*{2}([^\*]+?)(\*{2}|$)(?!\*)"); // Just a quick manip of italics
        private static Regex s_underline = new Regex(@"_+([^\*]+?)_+");

        private List<string> _lines = new List<string>();

        private FanficPiece(string filePath)
        {
            SourceFilename = Path.GetFileNameWithoutExtension(filePath);
        }

        public static FanficPiece Parse(string filePath)
        {
            FanficPiece piece = new FanficPiece(filePath);
            foreach (string line in File.ReadAllLines(filePath))
            {
                piece.FeedLine(line);
            }

            return piece;
        }

        public string SourceFilename { get; private set; }

        public string[] Lines
        {
            get { return _lines.ToArray(); }
        }

        private void FeedLine(string line)
        {
            // Is this a horizontal line break? If it is, then we insert an <hr /> and not a <p>
            // so we want to observe this special case and break early
            if (s_horizontalLineBreak.IsMatch(line))
            {
                _lines.Add("<hr />");
                return;
            }
            
            // We want to use it as an XElement for a little bit, to make sure that we can parse this into
            // well-qualified XML/HTML.
            string containerClasses = string.Join(" ", DetermineHtmlClasses(line));
            StringBuilder lineBuilder = new StringBuilder(line);
            PrepRawString(ref lineBuilder);
            ManipulateLine(ref lineBuilder);
            if (lineBuilder.Length == 0)
            {
                if (_lines.Count > 0 && !string.IsNullOrWhiteSpace(_lines[_lines.Count - 1]))
                {
                    _lines.Add(string.Empty);
                }
                return;
            }
            lineBuilder.Insert(0, "<p>");
            lineBuilder.Append("</p>");
            XElement lineElement = XElement.Parse(lineBuilder.ToString(), LoadOptions.None);
            if (!string.IsNullOrWhiteSpace(containerClasses))
            {
                lineElement.Add(new XAttribute("class", containerClasses));
            }
            _lines.Add(FinishLine(lineElement));
        }

        public static IEnumerable<FormattingRule> GetFormattingRules()
        {
            yield return new FormattingRule()
            {
                Input = "*{0}*",
                Output = "<em>{0}</em>"
            };
            yield return new FormattingRule()
            {
                Input = "**{0}**",
                Output = "<strong>{0}</strong>"
            };
            yield return new FormattingRule()
            {
                Input = "[tab character]{0}",
                Output = "<p class=\"indent\">{0}</p>"
            };
            yield return new FormattingRule()
            {
                Input = "_{0}_",
                Output = "<u>{0}</u>"
            };
            yield return new FormattingRule()
            {
                Input = "--",
                Output = "—"
            };
            foreach (FormattingRule projectRule in GetProjectFormattingRules())
            {
                yield return projectRule;
            }
        }

        // **********
        // Utility functions
        //
        // (You shouldn't really need to touch these)
        // **********
        private bool IsLineIndented(string line)
        {
            return s_indentation.IsMatch(line);
        }

        /// <summary>
        /// Returns the line without any indentation, if there is any.
        /// </summary>
        private string GetLineWithoutIndent(string line)
        {
            return s_indentation.Replace(line, "");
        }

        private void ApplyRegexToStringBuilder(ref StringBuilder lineBuilder, Regex regex,
            string replacement)
        {
            // This is janky as shit but there wasn't a more elegant solution on Stack Overflow
            // and this works the purpose.
            lineBuilder = new StringBuilder(regex.Replace(lineBuilder.ToString(), replacement));
        }

        private void PrepRawString(ref StringBuilder lineBuilder)
        {
            // This function takes the raw line and performs some initial transformations to it. These
            // include, but are not necessarily limited to:
            //      - Removal of any whitespace indentation (this function is called after a time when the
            //        indentation is processed
            //      - Transformation of the double hyphen into an em-dash
            //      - Condensing multiple space characters into a single space character (a weird anomaly
            //        encountered when writing in WriteMonkey.
            //
            // These can be removed if the project so specifies, but they are pretty core standards for my
            // writing style, and so probably don't need to be changed. Additional project-specific parsing
            // logic functions exist to allow projects to hook in at a more normal, standardised time.

            ApplyRegexToStringBuilder(ref lineBuilder, s_indentation, "");
            lineBuilder.Replace("<", "&lt;");
            lineBuilder.Replace(">", "&gt;");
            lineBuilder.Replace("--", "&#8212;");
            ApplyRegexToStringBuilder(ref lineBuilder, s_multipleConsecutiveWhitespaceCharacters, " ");
            ApplyRegexToStringBuilder(ref lineBuilder, s_bold, "<strong>$1</strong>");
            ApplyRegexToStringBuilder(ref lineBuilder, s_italics, "<em>$1</em>");
            ApplyRegexToStringBuilder(ref lineBuilder, s_underline, "<u>$1</u>");
        }

        private string FinishLine(XElement element)
        {
            string xml = element.ToString();
            xml = xml.Replace("—", "&mdash;"); // Because I want to, that's why
            xml = s_reduceHtmlWhitespace.Replace(xml, "$1$2");
            xml = xml.Replace("\n", "");
            xml = xml.Replace("\r", "");
            return xml;
        }

        // **********
        // Parsing logic functions
        // **********
        private IEnumerable<string> DetermineHtmlClasses(string line)
        {
            /* =============================================
             * Insert custom project logic here; for instance, if you want to make any line that is
             * indented but begins with "[H]" should have class="hiccup textMessage" added to it and
             * ignore the default class="indent" applied to any line which is indented.
             * 
             * Note that the utility function `IsLineIndented(...)` is meant to be used here rather
             * than self-implemented logic, so that both the tab-character and 3/4/5+ space characters
             * are both treated as indentations.
             * 
             * Don't forget -- to end early, use `yield break;`!
             * ==============================================
             **/


            // ------------------ Default logic ------------------------- //
            if (IsLineIndented(line))
            {
                yield return "indent";
            }
        }

        private void ManipulateLine(ref StringBuilder lineBuilder)
        {
            /* =============================================
             * Insert custom project logic here; for instance, if you want to transform any instance
             * of "[>H]" into "<span class='name'>(H) </span>" then this would be performed here.
             * 
             * General transformations are already performed by the time this function is called (ie,
             *     "*What did you say?*" has already become "<em>What did you say?</em>").
             * 
             * Please be aware of what you're doing! Make sure to close any and all tags that you open,
             * because this is going to become well-formatted XML!!
             * ==============================================
             **/



        }

        private static IEnumerable<FormattingRule> GetProjectFormattingRules()
        {
            /* =============================================
             * Insert custom project logic here; for every project-specific rule, a rule should be provided
             * so that even if the formatter hasn't been used in months, there is still non-code based documentation
             * of what transformations are supported.
             * 
             * Please be aware that both FormattingRule::Input and FormattingRule::Output are going to be
             * subjected to string.Format(...). These two properties should wrap around a `{0}` which will be
             * replaced with stock text demonstrating the transformation.
             * ==============================================
             **/

            yield break;
        }
    }
}
