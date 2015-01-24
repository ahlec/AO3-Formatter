using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AO3_Formatter
{
    public partial class DisplayForm : Form
    {
        FanficPiece _piece;

        public DisplayForm(FanficPiece piece)
        {
            InitializeComponent();
            _piece = piece;
            if (_piece != null)
            {
                txtOutput.Lines = piece.Lines;
            }
        }

        private void onDisplayRules(object sender, EventArgs e)
        {
            new RulesDisplay().ShowDialog(this);
        }

        private void onSave(object sender, EventArgs e)
        {
            if (_piece == null)
            {
                MessageBox.Show("You do not have a fanfic open right now!", "AO3 Formatter",
                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveDialog.Filter = "HTML files (*.htm)|*.htm|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.FileName = string.Concat(_piece.SourceFilename, ".htm");
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileStream = saveDialog.OpenFile();
                if (fileStream != null)
                {
                    StreamWriter writer = new StreamWriter(fileStream);
                    string[] lines = _piece.Lines;
                    for (int index = 0; index < lines.Length; ++index)
                    {
                        if (index < lines.Length - 1)
                        {
                            writer.WriteLine(lines[index]);
                        }
                        else
                        {
                            writer.Write(lines[index]);
                        }
                    }
                    writer.Close();
                    MessageBox.Show("File successfully saved!", "AO3 Formatter", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void onClose(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnTextAreaFocus(object sender, EventArgs e)
        {
            txtOutput.SelectAll();
            Clipboard.SetText(txtOutput.Text);
        }

        private void OnTextAreaClick(object sender, EventArgs e)
        {
            txtOutput.SelectAll();
        }

        private void OnAboutClicked(object sender, EventArgs e)
        {
            MessageBox.Show(string.Concat("Core version: ", VERSIONING.CORE_VERSION),
                "AO3 Formatter", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
