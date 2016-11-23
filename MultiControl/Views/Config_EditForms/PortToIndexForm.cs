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
        public PortToIndexForm(string importPortNumber)
        {
            InitializeComponent();
            this.Load += PortToIndexForm_Load;
            this.ImportPortNumber = importPortNumber;
        }

        private void PortToIndexForm_Load(object sender, EventArgs e)
        {
            InitializeData();
        }

        public string ImportPortNumber { get; set; } = String.Empty;
        void InitializeData()
        {
            this.tb_PortNo.Text = this.ImportPortNumber;

            int index = PortToIndexFactory.GetIndex(this.tb_PortNo.Text);
            if (index <= -1)
            {// 新的端口
                var index_list = PortToIndexFactory.GetIdleIndexStringList();

                this.cb_Index.Items.Clear();
                this.cb_Index.Items.Add(String.Empty);
                this.cb_Index.Items.AddRange(index_list.ToArray());
                this.cb_Index.SelectedIndex = 0;
            }
            else
            {// 如果端口已经被注册， 则关闭页面
                this.Close();
            }

            this.chb_isEnabled.Checked = PortToIndexFactory.IsEnabled;
        }

        private void chb_isEnabled_CheckedChanged(object sender, EventArgs e)
        {
            PortToIndexFactory.IsEnabled = this.chb_isEnabled.Checked;
        }
        /// <summary>
        /// 保存参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (this.cb_Index.Text == String.Empty)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                int index = -1;
                Int32.TryParse(this.cb_Index.Text, out index);
                PortToIndexFactory.ArrangePortNoToIndex(this.tb_PortNo.Text, index);
                this.DialogResult = DialogResult.OK;
            }
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
