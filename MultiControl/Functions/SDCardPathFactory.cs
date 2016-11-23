/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/18 16:25:41 
 * 文件名：SDCardPathFactory 
 * 版本：V1.0.0 
 * 文件说明：
 * 
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
using MultiControl.Common;

namespace MultiControl.Functions
{
    class SDCardPathFactory
    {
        private static string data_path = String.Empty;

        private static DataSet Specified_Config = new DataSet("ConfigData");

        public static DataTable SDCardPath_Table
        {
            get
            {
                return Specified_Config.Tables["model"];
            }
        }

        public SDCardPathFactory(string config_path)
        {
            data_path = config_path;
            if (File.Exists(data_path))
            {
                load_data();
            }
            else
            {
                initialize();
            }
        }

        public static void Save()
        {
            Specified_Config.WriteXml(data_path);
        }
        private void initialize()
        {
            common.m_log.Add($"initialize {data_path} data.");
            Specified_Config = new DataSet("SDCardPath");
            DataTable model_table = new DataTable();
            model_table.TableName = "model";

            model_table.Columns.Add("Name");
            model_table.Columns.Add("InternalCard");

            #region 初始化原始数据
            int cnt = 1;
            DataRow newRow = model_table.NewRow();
            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/storage/sdcard0";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard2";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/external_sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard-ext";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard/external_sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/extsdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/extsd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/emmc";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/extern_sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/ext_sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/ext_card";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/_ExternalSD";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/storage/extsdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/storage/emulated/0";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/storage/sdcard1";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/sdcard2";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/sdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/sdcard/sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/sdcard/external_sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/storage";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard/sd";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/exsdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/mnt/sdcard/extStorages/sdcard";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/ext_card";
            model_table.Rows.Add(newRow.ItemArray);


            newRow["Name"] = $"Sample{cnt++}";
            newRow["InternalCard"] = "/sotrage/exsdcard";
            model_table.Rows.Add(newRow.ItemArray);


            #endregion

            Specified_Config.Tables.Clear();
            Specified_Config.Tables.Add(model_table);
            Save();
        }
        private void load_data()
        {
            common.m_log.Add($"load {data_path} data.");
            Specified_Config.ReadXml(data_path);
        }
    }
}
