/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/5 18:02:54 
 * 文件名：PortToIndexFactory 
 * 版本：V1.0.0 
 * 文件说明：
 * 
 * 对PortToIndex.xml文件的相关操作
 * 
 * 修改者：           
 * 时间：               
 * 修改说明： 
* ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiControl.Common;

namespace MultiControl.Functions
{
    class PortToIndexFactory
    {
        public string FileName { get; set; } = ConfigurationHelper.ReadConfig("PortToIndex_File");
        public PortToIndexFactory()
        {
            try
            {
                ds_client = new DataSet("Port_Arrangement");
                ds_client.ReadXml(FileName);
            }
            catch (Exception ex)
            {
                OpenFileDialog file = new OpenFileDialog();
                file.Title = "please provide correct PortToIndex.xml file.";
                file.Filter = "PortToIndex.xml";
                file.Multiselect = false;
                if (file.ShowDialog() == DialogResult.OK)
                {
                    FileName = file.FileName;
                    ConfigurationHelper.WriteConfig("PortToIndex_File", file.FileName);
                    ds_client = new DataSet("Port_Arrangement");
                    ds_client.ReadXml(FileName);
                }
                else
                {
                    throw ex;
                }
            }
        }
        public DataTable Node_Table
        {
            get
            {
                get_node_table();
                return _node_table;
            }

            set
            {
                _node_table = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                get_isenabled();
                return _isEnabled;
            }
            set
            {
                if (ds_client != null)
                {
                    var IsEnabled_Table = ds_client.Tables["IsEnabled"];
                    IsEnabled_Table.Rows[0]["Value"] = value;
                    _isEnabled = value;
                }
            }
        }

        private DataSet ds_client;
        private Boolean _isEnabled = true;

        private DataTable _node_table;

        /// <summary>
        /// 根据Port No获取Index
        /// </summary>
        /// <param name="portNo"></param>
        /// <returns></returns>
        public int GetIndex(string portNo)
        {
            DataTable node_table = Node_Table;
            if (node_table == null)
                return -1;
            foreach (DataRow node in node_table.Rows)
            {
                if (node["Port"].ToString() == portNo)
                {
                    int index = Int32.Parse(node["Index"].ToString());
                    return index;
                }
            }
            return -1;
        }
        /// <summary>
        /// 根据Index获取Port No
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPortNo(int index)
        {
            if (Node_Table == null)
                return "-1";
            foreach (DataRow node in Node_Table.Rows)
            {
                int iindex = Int32.Parse(node["Index"].ToString());
                if (iindex == index)
                    return node["Port"].ToString();
            }
            return "-1";
        }
        /// <summary>
        /// 将USB Port No和Index配对
        /// </summary>
        /// <param name="portNo"></param>
        /// <param name="index"></param>
        public void ArrangePortNoToIndex(string portNo, int index)
        {
            if (portNo == String.Empty || index < 0)
                return;
            for (int iindex = 0; iindex != Node_Table.Rows.Count; iindex++)
            {
                DataRow node = Node_Table.Rows[iindex];
                int iiindex = Int32.Parse(node["Index"].ToString());
                if (iiindex == index)
                {
                    node["Port"] = portNo;
                    break;
                }
            }
        }
        /// <summary>
        /// 获取空闲Index列表
        /// </summary>
        /// <returns></returns>
        public List<Int32> GetIdleIndexList()
        {
            List<Int32> index_list = new List<int>();
            foreach (DataRow node in Node_Table.Rows)
            {
                int iindex = Int32.Parse(node["Index"].ToString());
                if (node["Port"].ToString() == "-1" && !index_list.Contains(iindex))
                    index_list.Add(iindex);

            }
            index_list.Sort();
            return index_list;
        }
        /// <summary>
        /// 获取已经设置好的端口列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetArrangedPortNoList()
        {
            List<string> pono_list = new List<string>();
            foreach (DataRow node in Node_Table.Rows)
            {
                string portNo = node["Port"].ToString();
                if (node["Port"].ToString() != "-1" && !pono_list.Contains(portNo))
                    pono_list.Add(portNo);
            }
            pono_list.Sort();
            return pono_list;
        }
        /// <summary>
        /// 将改动更新到xml文件中
        /// </summary>
        public void Save_To_xml()
        {
            if (ds_client != null)
            {
                ds_client.WriteXml(FileName);
            }
        }
        void get_node_table()
        {
            if (ds_client != null)
                _node_table = ds_client.Tables["Node"];
        }

        void get_isenabled()
        {
            if (ds_client != null)
            {
                var IsEnabled_Table = ds_client.Tables["IsEnabled"];
                Boolean.TryParse(IsEnabled_Table.Rows[0]["Value"].ToString(), out _isEnabled);
            }
        }
    }
}
