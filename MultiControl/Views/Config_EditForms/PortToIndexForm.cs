using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiControl.Common;
using MultiControl.Functions;

namespace MultiControl
{
    public partial class PortToIndexForm : Form
    {
        PortToIndexFactory factory;
        public PortToIndexForm(string importPortNumber)
        {
            InitializeComponent();
            this.ImportPortNumber = importPortNumber;
            InitializeData();
        }
        public string ImportPortNumber { get; set; } = String.Empty;

        public Int32 Index { get; set; } = -1;
        void InitializeData()
        {
            factory = new PortToIndexFactory();

            if (!String.IsNullOrEmpty(ImportPortNumber))
            {
                this.cb_usb_port_no.Items.Clear();
                this.cb_usb_port_no.Items.Add(ImportPortNumber);
                this.cb_usb_port_no.SelectedIndex = 0;
            }
            else
            {// 对已经设置好的进行查看或者重新设置
                var portNoList = factory.GetArrangedPortNoList();
                this.cb_usb_port_no.Items.Clear();
                this.cb_usb_port_no.Items.AddRange(portNoList.ToArray());

                this.cb_usb_port_no.SelectedIndex = -1;
            }
            this.chb_isEnabled.Checked = factory.IsEnabled;
        }

        private void btn_ViewTable_Click(object sender, EventArgs e)
        {
            IndexToPortTable table_form = new IndexToPortTable(factory.Node_Table);
            table_form.ShowDialog();
        }

        private void chb_isEnabled_CheckedChanged(object sender, EventArgs e)
        {
            factory.IsEnabled = this.chb_isEnabled.Checked;// 更新到xml文件中

            this.cb_Index.Enabled = this.cb_usb_port_no.Enabled = this.chb_isEnabled.Checked;//= this.btn_ViewTable.Enabled
        }

        private void cb_usb_port_no_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portNo = this.cb_usb_port_no.SelectedIndex >= 0 ? this.cb_usb_port_no.Text : String.Empty;
            if (portNo == String.Empty)
                return;
            string arrangedIndex = String.Empty;
            int index = factory.GetIndex(portNo);
            var index_list = factory.GetIdleIndexList();
            if (index != -1)
            {
                arrangedIndex = index.ToString();
            }
            this.cb_Index.Items.Clear();
            this.cb_Index.Items.Add(arrangedIndex);

            List<string> index_str_list = new List<string>();
            foreach (var iindex in index_list)
            {
                index_str_list.Add(iindex.ToString());
            }
            this.cb_Index.Items.AddRange(index_str_list.ToArray());

            this.cb_Index.SelectedIndex = 0;
        }

        private void cb_Index_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portNo = this.cb_usb_port_no.SelectedIndex >= 0 ? this.cb_usb_port_no.Text : String.Empty;
            int index = this.cb_Index.Text != String.Empty ? Int32.Parse(this.cb_Index.Text) : -1;
            factory.ArrangePortNoToIndex(portNo, index);
            this.Index = index;
            //btn_OK_Click(null, null);
        }
        /// <summary>
        /// 保存参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OK_Click(object sender, EventArgs e)
        {
            string portNo = this.cb_usb_port_no.SelectedIndex >= 0 ? this.cb_usb_port_no.Text : String.Empty;
            int index = this.cb_Index.Text != String.Empty ? Int32.Parse(this.cb_Index.Text) : -1;
            factory.ArrangePortNoToIndex(portNo, index);
            this.Index = index;

            factory.Save_To_xml();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
