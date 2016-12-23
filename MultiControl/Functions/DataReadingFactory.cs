/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/12/6 16:49:46 
 * 文件名：DataReadingFactory 
 * 版本：V1.0.0 
 * 文件说明：
 * 处理data reading功能的类
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
using Helpers;
using MultiControl.Views;
using System.Diagnostics;
using SMEConnector;

namespace MultiControl.Functions
{
    public class DataReadingFactory
    {
        private DataSet ds_client;

        private DataTable _android_Report_Table;
        public DataTable Android_Report_Table
        {
            get
            {
                if (_android_Report_Table == null)
                {
                    InitAndroidReportTable();
                }
                return _android_Report_Table;
            }
            set
            {
                _android_Report_Table = value;
            }
        }
        private DataTable _dr_items_table;
        public DataTable DR_Items_Table
        {
            get
            {
                if (_dr_items_table == null)
                    InitDRItemsTable();
                return _dr_items_table;
            }
            set
            {
                _dr_items_table = value;
            }
        }
        public async Task<Dictionary<string, string>> ReadwInfoFileData(string wInfo_file, int ThreadIndex, string xmlFilePath, Stopwatch sw)
        {
            if (!File.Exists(wInfo_file))
            {
                common.m_log.Add($"{wInfo_file} no such file.");
                return null;
            }

            DateTime start = DateTime.Now;
            Dictionary<string, string> wInfoList = new Dictionary<string, string>();
            using (FileStream fs = new FileStream(wInfo_file, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        string item = await reader.ReadLineAsync();
                        if (item.Contains("="))
                        {
                            string[] items = item.Split('=');
                            wInfoList.Add(items[0], items[1]);
                        }
                    }
                }
            }
            if (wInfoList.Count <= 0)
                return null;

            #region xml_table
            DataTable android_report = Android_Report_Table;
            DataRow newrow = android_report.NewRow();
            foreach (DataColumn item in android_report.Columns)
            {
                newrow[item.ColumnName] = "N/A";// 默认值均设为N/A
                if (wInfoList.ContainsKey(item.ColumnName))
                {
                    newrow[item.ColumnName] = wInfoList[item.ColumnName];
                }
            }
            newrow["RESULT"] = TEST_RESULT.PASS.ToString();
            newrow["SOFTWARE_VER"] = config_inc.MULTICONTROL_VERSION;
            newrow["WORKSTATION_NAME"] = common.GetComputerName();
            newrow["START_TIME"] = DateTime.Now.Subtract(sw.Elapsed);
            newrow["TOTAL_TIME"] = sw.Elapsed;
            newrow["TransactionRef"] = common.GetMD5Code(DateTime.Now.Ticks.ToString() + newrow["SERIAL_NUMBER"].ToString());
            newrow["Port"] = ThreadIndex;
            android_report.Rows.Add(newrow.ItemArray);
            Android_Report_Table = android_report;
            #endregion

            #region database
            DataTable dr_items = DR_Items_Table;
            newrow = dr_items.NewRow();
            foreach (DataColumn item in dr_items.Columns)
            {
                newrow[item.ColumnName] = "N/A";
                if (wInfoList.ContainsKey(item.ColumnName))
                {
                    newrow[item.ColumnName] = wInfoList[item.ColumnName];
                }
            }
            newrow["Transaction ID"] = common.GetMD5Code(DateTime.Now.Ticks.ToString() + newrow["Serial Number"].ToString());
            newrow["Site"] = Login.Operator["site"];
            newrow["Error Code"] = "N/A";
            newrow["Product Version"] = config_inc.MULTICONTROL_VERSION;
            newrow["Total time"] = sw.Elapsed;
            newrow["Server Time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            newrow["LocalTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            newrow["TimeCreated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            newrow["Company"] = common.GetCompanyname();
            newrow["Port Number"] = ThreadIndex;
            newrow["saveXmlPath"] = xmlFilePath;
            dr_items.Rows.Add(newrow.ItemArray);
            _dr_items_table = dr_items;
            sw.Stop();
            #endregion
            return wInfoList;
        }
        void InitAndroidReportTable()
        {
            _android_Report_Table = new DataTable();
            _android_Report_Table.TableName = "Model";
            _android_Report_Table.Columns.Add("SERIAL_NUMBER");
            _android_Report_Table.Columns.Add("MEID_hex");
            _android_Report_Table.Columns.Add("MEID_dec");
            _android_Report_Table.Columns.Add("MEIDwithCheckDigit");
            _android_Report_Table.Columns.Add("IMEI1");
            _android_Report_Table.Columns.Add("IMEI2");
            _android_Report_Table.Columns.Add("RESULT");
            _android_Report_Table.Columns.Add("ERRORLIST");
            _android_Report_Table.Columns.Add("SOFTWARE_VER");
            _android_Report_Table.Columns.Add("OEM_NAME");
            _android_Report_Table.Columns.Add("MODEL_NAME");
            _android_Report_Table.Columns.Add("WORKSTATION_NAME");
            _android_Report_Table.Columns.Add("ROOTED");
            _android_Report_Table.Columns.Add("START_TIME");
            _android_Report_Table.Columns.Add("TOTAL_TIME");
            _android_Report_Table.Columns.Add("OS_VERSION");
            _android_Report_Table.Columns.Add("BASEBAND_VERSION");
            _android_Report_Table.Columns.Add("BUILD_NUMBER");
            _android_Report_Table.Columns.Add("MAC_ADDRESS");
            _android_Report_Table.Columns.Add("TransactionRef");
            _android_Report_Table.Columns.Add("PurchaseOrder");
            _android_Report_Table.Columns.Add("Carrier");
            _android_Report_Table.Columns.Add("Color");
            _android_Report_Table.Columns.Add("Memory");
            _android_Report_Table.Columns.Add("ModelNumber");
            _android_Report_Table.Columns.Add("Port");
        }
        void InitDRItemsTable()
        {
            _dr_items_table = new DataTable();
            _dr_items_table.Columns.Add("OS");
            _dr_items_table.Columns.Add("UserName");
            _dr_items_table.Columns.Add("Transaction ID");
            _dr_items_table.Columns.Add("Serial Number");
            _dr_items_table.Columns.Add("Site");
            _dr_items_table.Columns.Add("Error Code");
            _dr_items_table.Columns.Add("Product Version");
            _dr_items_table.Columns.Add("Model");
            _dr_items_table.Columns.Add("Port Number");
            _dr_items_table.Columns.Add("Data Read Operator");
            _dr_items_table.Columns.Add("Total time");
            _dr_items_table.Columns.Add("MacAddress");
            _dr_items_table.Columns.Add("Jailbroken");
            _dr_items_table.Columns.Add("UDID");
            _dr_items_table.Columns.Add("Server Time");
            _dr_items_table.Columns.Add("Computer Name");
            _dr_items_table.Columns.Add("SIMExist");
            _dr_items_table.Columns.Add("Purchase Number");
            _dr_items_table.Columns.Add("UserDomainName");
            _dr_items_table.Columns.Add("LastCarrier");
            _dr_items_table.Columns.Add("DefaultCarrier");
            _dr_items_table.Columns.Add("LocalTime");
            _dr_items_table.Columns.Add("TimeCreated");
            _dr_items_table.Columns.Add("Company");
            _dr_items_table.Columns.Add("IMEI1");
            _dr_items_table.Columns.Add("IMEI2");
            _dr_items_table.Columns.Add("MEID");
            _dr_items_table.Columns.Add("Make");
            _dr_items_table.Columns.Add("Model Number");
            _dr_items_table.Columns.Add("Memory Size");
            _dr_items_table.Columns.Add("batterylevel");
            _dr_items_table.Columns.Add("color");
            _dr_items_table.Columns.Add("saveXmlPath");
            _dr_items_table.Columns.Add("RegulatoryModelNumber");
            _dr_items_table.Columns.Add("Battery Charge Cycle");
            _dr_items_table.Columns.Add("Mainboard Serial Number");
            _dr_items_table.Columns.Add("Battery ID");
            _dr_items_table.Columns.Add("Carrier Lock");
        }
        public async Task Save(string data_path)
        {
            ds_client = new DataSet("transactionReport");
            ds_client.Tables.Add(this.Android_Report_Table);
            ds_client.WriteXml(data_path);
            string xmlStr = String.Empty;
            using (StreamReader reader = new StreamReader(data_path))
            {
                xmlStr = await reader.ReadToEndAsync();
                xmlStr = xmlStr.Replace("<Model>\r\n", "");
                xmlStr = xmlStr.Replace("</Model>\r\n", "");
            }
            using (StreamWriter writer = new StreamWriter(data_path))
            {
                await writer.WriteAsync(xmlStr);
            }
        }
        /// <summary>
        /// 上传到SME
        /// </summary>
        /// <param name="data_path"></param>
        /// <returns></returns>
        public async Task<string> UploadToSME(string data_path)
        {
            using (StreamReader reader = new StreamReader(data_path))
            {
                try
                {
                    string xmlStr = await reader.ReadToEndAsync();
                    WebConnector webconnector = new WebConnector();
                    StringBuilder sb = new StringBuilder();
                    string session_id = webconnector.GetSessionId();
                    webconnector.SendXml(session_id, xmlStr, sb);
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        #region database methods
        public string Insert_one_record_to_database()
        {
            if (DR_Items_Table == null || DR_Items_Table.Rows.Count <= 0)
                return String.Empty;
            DataRow record = DR_Items_Table.Rows[0];
            string insert = @"
INSERT INTO `db_android_dr`.`tbl_records` (`work_station_id`, `user_id`, `purchase_no_id`, `Site`, 
`Product Version`, `Serial Number`, `OS`, `Transaction ID`, `Error Code`, `Model`, `Port Number`, 
`Server Time`, `LocalTime`, `Total time`, 
`MacAddress`, `Jailbroken`, `UDID`, `SIMExist`, `LastCarrier`, `DefaultCarrier`, `Company`, `IMEI1`, 
`MEID`, `IMEI2`, 
`Make`, `Model Number`, `Memory Size`, `batteryLevel`, `color`, `SaveXmlPath`, `RegulatoryModelNumber`, 
`Battery Charge Cycle`, `Mainboard Serial Number`, `Battery ID`, `Carrier Lock`, `TimeCreated`) 
VALUES (@work_station_id, @user_id, @purchase_no_id, @Site, @PD_Version, @SN, @OS, @T_ID, 
@EC, @Model, @PN, @ST, @LT, @TT, @MA, @J, @UUID, @SIMExist, @LastCarrier, @DefaultCarrier,
@Company, @IMEI1, @MEID, @IMEI2, @Make, @MN, @MS, @BL, @Color, @SaveXmlPath, @RMN, @BCC, @MSN, @BID, @CL, @TC);
";

            try
            {
                var ret = MySqlHelper.ExecuteNonQuery(MySqlHelper.ConnectionString, insert,
                #region parameter
                     new MySql.Data.MySqlClient.MySqlParameter("@work_station_id", Login.Operator["work_station_id"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@user_id", Login.Operator["operator_id"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@purchase_no_id", Login.Operator["purchase_id"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@Site", Login.Operator["site"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@PD_Version", record["Product Version"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@SN", record["Serial Number"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@OS", record["OS"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@T_ID", record["Transaction ID"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@EC", record["Error Code"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@Model", record["Model"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@PN", record["Port Number"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@ST", record["Server Time"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@LT", record["LocalTime"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@TT", record["Total time"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@MA", record["MacAddress"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@J", record["Jailbroken"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@UUID", record["UDID"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@SIMExist", record["SIMExist"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@LastCarrier", record["LastCarrier"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@DefaultCarrier", record["DefaultCarrier"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@Company", record["Company"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@IMEI1", record["IMEI1"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@IMEI2", record["IMEI2"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@MEID", record["MEID"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@Make", record["Make"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@MN", record["Model Number"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@MS", record["Memory Size"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@BL", record["batterylevel"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@Color", record["color"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@SaveXmlPath", record["saveXmlPath"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@RMN", record["RegulatoryModelNumber"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@BCC", record["Battery Charge Cycle"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@MSN", record["Mainboard Serial Number"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@BID", record["Battery ID"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@CL", record["Carrier Lock"]),
                     new MySql.Data.MySqlClient.MySqlParameter("@TC", DateTime.Now)
                #endregion
                     );

                string query = @"
select id from `db_android_dr`.`tbl_records` 
where `Transaction ID` = @TID";
                string id = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                    new MySql.Data.MySqlClient.MySqlParameter("@TID", record["Transaction ID"])).ToString();
                return id;
            }
            catch (Exception ex)
            {
                common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
                return String.Empty;
            }
        }
        #endregion
    }
}
