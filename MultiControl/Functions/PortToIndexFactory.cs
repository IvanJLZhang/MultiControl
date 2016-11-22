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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiControl.Common;

namespace MultiControl.Functions
{
    public class PortToIndexFactory
    {
        private static string data_path = String.Empty;

        private static DataSet ds_client = new DataSet("Port_Arrangement");

        public PortToIndexFactory(string config_path)
        {
            data_path = config_path;
            if (File.Exists(data_path))
            {
                LoadData();
            }
            else
            {
                inittialize();
            }
        }
        public static DataTable Node_Table
        {
            get
            {
                return ds_client.Tables["Node"];
            }
        }

        public static bool IsEnabled
        {
            get
            {
                var IsEnabled_Table = ds_client.Tables["IsEnabled"];
                bool isenabled = true;
                Boolean.TryParse(IsEnabled_Table.Rows[0]["Value"].ToString(), out isenabled);
                return isenabled;
            }
            set
            {
                if (ds_client != null)
                {
                    var IsEnabled_Table = ds_client.Tables["IsEnabled"];
                    IsEnabled_Table.Rows[0]["Value"] = value;
                    Save();
                }
            }
        }


        /// <summary>
        /// 根据Port No获取Index
        /// </summary>
        /// <param name="portNo"></param>
        /// <returns></returns>
        public static int GetIndex(string portNo)
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
        public string GetPortNo(int Index)
        {
            DataTable node_table = Node_Table;
            if (Node_Table == null)
                return "-1";
            foreach (DataRow node in node_table.Rows)
            {
                int iindex = Int32.Parse(node["Index"].ToString());
                if (iindex == Index)
                    return node["Port"].ToString();
            }
            return "-1";
        }
        /// <summary>
        /// 将USB Port No和Index配对
        /// </summary>
        /// <param name="portNo"></param>
        /// <param name="index"></param>
        public static void ArrangePortNoToIndex(string portNo, int index)
        {
            if (portNo == String.Empty || index < 0)
                return;
            DataTable node_table = Node_Table;
            for (int iindex = 0; iindex != node_table.Rows.Count; iindex++)
            {// 如果端口已经被配置， 则重新置为空
                DataRow node = Node_Table.Rows[iindex];
                if (node["Port"].ToString() == portNo)
                {
                    node["Port"] = String.Empty;
                }
            }
            for (int iindex = 0; iindex != node_table.Rows.Count; iindex++)
            {
                DataRow node = Node_Table.Rows[iindex];
                int iiindex = Int32.Parse(node["Index"].ToString());
                if (iiindex == index)
                {
                    node["Port"] = portNo;
                    break;
                }
            }
            Save();
        }

        /// <summary>
        /// 重置所有端口
        /// </summary>
        public static void inittialize()
        {
            common.m_log.Add($"initialize {data_path} data.");
            DataTable isenabled_table = new DataTable();
            isenabled_table.TableName = "IsEnabled";
            isenabled_table.Columns.Add("Value");

            DataRow row = isenabled_table.NewRow();
            row["Value"] = true.ToString();
            isenabled_table.Rows.Add(row.ItemArray);

            DataTable node_table = new DataTable();
            node_table.TableName = "Node";

            node_table.Columns.Add("Index");
            node_table.Columns.Add("Port");
            int rowCount = MainWindow.m_Rows * MainWindow.m_Cols;

            DataRow newRow = node_table.NewRow();
            for (int index = 0; index < rowCount; index++)
            {
                newRow["Index"] = index + 1;
                newRow["Port"] = String.Empty;
                node_table.Rows.Add(newRow.ItemArray);
            }
            ds_client.Tables.Clear();
            ds_client.Tables.Add(isenabled_table);
            ds_client.Tables.Add(node_table);

            Save();
        }

        private void LoadData()
        {
            common.m_log.Add($"load {data_path} data.");

            ds_client.ReadXml(data_path);
        }

        /// <summary>
        /// 获取空闲Index列表
        /// </summary>
        /// <returns></returns>
        public static List<Int32> GetIdleIndexList()
        {
            DataTable node_table = Node_Table;
            List<Int32> index_list = new List<int>();
            foreach (DataRow node in node_table.Rows)
            {
                int index = Int32.Parse(node["Index"].ToString());
                if (node["Port"].ToString() == "" && !index_list.Contains(index))
                    index_list.Add(index);
            }
            //index_list.Sort();
            return index_list;
        }
        public static List<String> GetIdleIndexStringList()
        {
            DataTable node_table = Node_Table;
            List<String> index_list = new List<String>();
            foreach (DataRow node in node_table.Rows)
            {
                int index = Int32.Parse(node["Index"].ToString());
                if (node["Port"].ToString() == "" && !index_list.Contains(index.ToString()))
                    index_list.Add(index.ToString());
            }
            //index_list.Sort();
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
        public static void Save()
        {
            ds_client.WriteXml(data_path);
        }
    }
}
