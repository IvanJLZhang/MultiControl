/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/16 17:01:03 
 * 文件名：SpecifiedConfigPathFactory 
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
    class SpecifiedConfigPathFactory
    {
        private static string data_path = String.Empty;

        private static DataSet Specified_Config = new DataSet("ConfigData");

        public static DataTable Model_Table
        {
            get
            {
                return Specified_Config.Tables["model"];
            }
            set
            {
                Specified_Config.Tables.Clear();
                Specified_Config.Tables.Add(value);
            }
        }

        public SpecifiedConfigPathFactory(string config_path)
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
        public static void initialize()
        {
            common.m_log.Add($"initialize {data_path} data.");
            Specified_Config = new DataSet("ConfigData");
            DataTable model_table = new DataTable();
            model_table.TableName = "model";

            model_table.Columns.Add("Name");
            model_table.Columns.Add("Brand");
            model_table.Columns.Add("Path");
            model_table.Columns.Add("Estimate");
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
