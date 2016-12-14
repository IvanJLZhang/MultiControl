using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiControl.Common;

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
        private void Help_Load(object sender, EventArgs e)
        {
            this.textBoxKey.Text = File.ReadAllText("license.dat", Encoding.Unicode).Trim();
            this.lbl_tool_version.Text = config_inc.MULTICONTROL_VERSION + " " + config_inc.BUILD_DATE;
        }
    }
}
