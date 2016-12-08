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
    public partial class FormMapping : Form
    {

        public FormMapping()
        {
            InitializeComponent();
        }

        private void FormMapping_Load(object sender, EventArgs e)
        {
            LoadConfig();
            this.label1.Text = "Current Config Table\r\n    * Double Click To Edit Model Name\r\n    * Click ... To Change Config Folder\r\n";
        }

        private void LoadConfig()
        {
            if (SpecifiedConfigPathFactory.Model_Table == null || SpecifiedConfigPathFactory.Model_Table.Rows.Count <= 0)
            {
                DataTable Model_Table = new DataTable();
                Model_Table.TableName = "model";
                Model_Table.Columns.Add("Name");
                Model_Table.Columns.Add("Brand");
                Model_Table.Columns.Add("Path");
                Model_Table.Columns.Add("Estimate");

                SpecifiedConfigPathFactory.Model_Table = Model_Table;
            }
            dataGridViewPath.DataSource = SpecifiedConfigPathFactory.Model_Table;

            if (this.dataGridViewPath.Columns["Path"] != null)
            {
                this.dataGridViewPath.Columns["Path"].Width = 320;
            }
            if (this.dataGridViewPath.Columns["Estimate"] != null)
            {
                this.dataGridViewPath.Columns["Estimate"].Width = 64;
            }
            DataGridViewButtonColumn col = new DataGridViewButtonColumn();
            col.Name = "Choose";
            col.Text = "...";
            col.ToolTipText = "Config Path";
            col.Width = 25;
            col.UseColumnTextForButtonValue = true;
            col.HeaderText = "";
            this.dataGridViewPath.Columns.Add(col);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SpecifiedConfigPathFactory.Save();
            Close();
        }
        private void dataGridViewPath_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridViewPath.Columns[e.ColumnIndex].Name.CompareTo("Choose") == 0)
            {
                if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)this.dataGridViewPath.Rows[e.RowIndex].Cells["Path"];
                    cell.Value = folderBrowserDialog1.SelectedPath;
                }
            }
        }
        private void editTestItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditItems items = new FormEditItems();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            items.ConfigFolder = cfg;
            items.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            items.ShowDialog();
        }

        private void editMoniPowerCFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_MONIPOWER;
            cfgDlg.ShowDialog();
        }

        private void editSysInfoCFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_SYSINFO;
            cfgDlg.ShowDialog();
        }

        private void editWIFICFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_WIFI;
            cfgDlg.ShowDialog();
        }

        private void editAudioLoopbackCFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_AUDIOLOOPBACK;
            cfgDlg.ShowDialog();
        }

        private void editMultiTestGPSCFGtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_GPS;
            cfgDlg.ShowDialog();
        }

        private void wIFIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_WIFI;
            cfgDlg.ShowDialog();
        }

        private void bTToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void gPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEditCfg cfgDlg = new FormEditCfg();
            string cfg = this.dataGridViewPath.CurrentRow.Cells["Path"].Value.ToString();
            cfgDlg.ConfigFolder = cfg;
            cfgDlg.Title = this.dataGridViewPath.CurrentRow.Cells["Name"].Value.ToString();
            cfgDlg.mCFGTYPE = FormEditCfg.CfgType.CT_GPS;
            cfgDlg.ShowDialog();
        }
    }
}
