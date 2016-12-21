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

namespace MultiControl.Views
{
    public partial class Login : Form
    {
        static DataTable _operator = null;
        #region 全局变量
        /// <summary>
        /// 登录的操作员信息
        /// </summary>
        public static DataRow Operator
        {
            get
            {
                if (_operator == null)
                    return null;
                return _operator.Rows[0];
            }
        }
        #endregion
        public Login()
        {
            InitializeComponent();
            this.Load += Login_Load;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            if (Operator != null)
            {
                this.tb_operator.Text = Operator["operator_name"].ToString();
                this.tb_purchase_no.Text = Operator["purchase_no"].ToString();
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (this.tb_operator.Text == String.Empty || this.tb_purchase_no.Text == String.Empty)
                return;
            _operator = new DataTable();
            _operator.Columns.Add("operator_name");
            _operator.Columns.Add("purchase_no");
            _operator.Columns.Add("operator_id");
            _operator.Columns.Add("purchase_id");
            _operator.Columns.Add("work_station_id");
            _operator.Columns.Add("site");
            DataRow newrow = _operator.NewRow();
            newrow["operator_name"] = this.tb_operator.Text.Trim();
            newrow["purchase_no"] = this.tb_purchase_no.Text.Trim();
            if (MainWindow.m_enable_mysql)
            {
                _operator.Rows.Add(newrow);
                try
                {
                    LoginFactory.Operator_Login();
                }
                catch (Exception ex)
                {
                    common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
                }
            }
            else
            {
                newrow["operator_id"] = -1;
                newrow["purchase_id"] = -1;
                newrow["work_station_id"] = -1;
                newrow["site"] = "unknown";
                _operator.Rows.Add(newrow);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            _operator = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Login_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_login_Click(null, null);
            }
        }
    }
}
