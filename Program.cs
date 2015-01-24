using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AO3_Formatter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string start = "*hello world!* so you see?";
            var sup = new System.Text.RegularExpressions.Regex(@"(?<!\*)\*{1}([^\*]+?)(\*{1}|$)(?!\*)");
            string end = sup.Replace(start, "<em>$1</em>");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Determine the source filename
            string sourceFilename = null;
            if (args.Length > 0)
            {
                sourceFilename = args[0];
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Please select source file.";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                DialogResult result = openFileDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    MessageBox.Show("Please come again!", "AO3 Formatter", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                sourceFilename = openFileDialog.FileName;
            }

            FanficPiece piece;
            //try
            {
                piece = FanficPiece.Parse(sourceFilename);
            }
            /*catch (Exception ex)
            {
                string errorMessage = "Completely unknown error!";
                if (ex != null)
                {
                    errorMessage = string.Concat("[", ex.GetType().Name, "] ",
                        ex.Message);
                }

                MessageBox.Show(errorMessage, "AO3 Formatter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            DisplayForm form = new DisplayForm(piece);
            form.Activate();
            Application.Run(form);
        }
    }
}
