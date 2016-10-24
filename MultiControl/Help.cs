using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiControl
{
    public partial class Help : Form
    {
        private string mLicensedKey = string.Empty;

        public Help()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void setLicensedKey(string key)
        {
            mLicensedKey = key;
        }

        private void Help_Load(object sender, EventArgs e)
        {
            this.textBoxKey.Text = mLicensedKey;
        }
    }
}
