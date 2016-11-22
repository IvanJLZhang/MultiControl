using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiControl.Functions;

namespace MultiControl
{
    public partial class FormSDCard : Form
    {
        public FormSDCard()
        {
            InitializeComponent();
        }

        private void FormSDCard_Load(object sender, EventArgs e)
        {
            LoadConfig();
            this.labelDescription.Text = "Current Config Table\r\n    * Double Click To Edit Model Name\r\n    * Double Click To Edit SDCard Path\r\n";
        }

        private void LoadConfig()
        {
            dataGridViewSDCard.DataSource = SDCardPathFactory.SDCardPath_Table;

            if (this.dataGridViewSDCard.Columns["InternalCard"] != null)
            {
                this.dataGridViewSDCard.Columns["InternalCard"].Width = 320;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            SDCardPathFactory.Save();
            Close();
        }
    }
}
