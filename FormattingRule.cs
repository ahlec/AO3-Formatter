using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AO3_Formatter
{
    public class FormattingRule
    {
        private string _inputRule;
        private string _outputRule;

        /// <summary>
        /// This will be used with a string.Format(...) call, and so should
        /// contain a {0} parameter wrapped around the rule.
        /// </summary>
        public string Input
        {
            get { return string.Format(_inputRule, "Hello World"); }
            set { _inputRule = value; }
        }

        /// <summary>
        /// This will be used with a string.Format(...) call, and so should
        /// contain a {0} parameter wrapped around the rule.
        /// </summary>
        public string Output
        {
            get { return string.Format(_outputRule, "Hello World"); }
            set { _outputRule = value; }
        }
    }
}
