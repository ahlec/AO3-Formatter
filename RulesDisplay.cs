using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AO3_Formatter
{
    public partial class RulesDisplay : Form
    {
        public RulesDisplay()
        {
            InitializeComponent();

            var bs = new BindingSource();
            List<FormattingRule> rules = FanficPiece.GetFormattingRules().ToList();
            bs.DataSource = rules;
            dataGridView1.DataSource = bs; 
        }
    }
}
