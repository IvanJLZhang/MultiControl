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
    public partial class FormSDCard : Form
    {
        private DataSet mDataSet;

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
            mDataSet = new DataSet("ConfigPath");
            mDataSet.ReadXml("ConfigPath.xml");

            dataGridViewSDCard.DataSource = mDataSet.Tables[0];

            if (this.dataGridViewSDCard.Columns["InternalCard"] != null)
            {
                this.dataGridViewSDCard.Columns["InternalCard"].Width = 320;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            mDataSet.WriteXml("ConfigPath.xml");
            Close();
        }
    }
}
