/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/12/13 15:56:29 
 * 文件名：LoginFactory 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using MultiControl.Common;
using MultiControl.Views;

namespace MultiControl.Functions
{
    class LoginFactory
    {
        public static void Operator_Login()
        {
            if (Login.Operator == null)
                return;
            #region user
            string query = @"
select id from `tbl_user`
where `u_uid` = @u_uid;
";
            var user = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                 new MySql.Data.MySqlClient.MySqlParameter("@u_uid", Login.Operator["operator_name"]
                 ));
            if (user == null)
            {// 如果没有， 则新增
                string insert = @"
INSERT INTO `db_android_dr`.`tbl_user` (`u_uid`, `u_uname`, `u_create_time`, `u_last_login_time`) VALUES (@u_uid, @u_uname, @u_create_time, @u_last_login_time);";
                var ret = MySqlHelper.ExecuteNonQuery(MySqlHelper.ConnectionString, insert,
                    new MySql.Data.MySqlClient.MySqlParameter("@u_uid", Login.Operator["operator_name"]),
                    new MySql.Data.MySqlClient.MySqlParameter("@u_uname", Login.Operator["operator_name"]),
                    new MySql.Data.MySqlClient.MySqlParameter("@u_create_time", DateTime.Now),
                    new MySql.Data.MySqlClient.MySqlParameter("@u_last_login_time", DateTime.Now)
                    );
            }
            else
            {// 如果存在， 更新登录时间
                string update = @"
UPDATE `db_android_dr`.`tbl_user` SET `u_last_login_time`=@u_last_login_time WHERE `u_uid`=@u_uid;";
                var ret = MySqlHelper.ExecuteNonQuery(MySqlHelper.ConnectionString, update,
                    new MySql.Data.MySqlClient.MySqlParameter("@u_uid", Login.Operator["operator_name"]),
                    new MySql.Data.MySqlClient.MySqlParameter("@u_last_login_time", DateTime.Now)
                    );
            }

            user = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                                 new MySql.Data.MySqlClient.MySqlParameter("@u_uid", Login.Operator["operator_name"])
                                 );
            if (user != null)
            {
                Login.Operator["operator_id"] = user;
            }
            #endregion

            #region purchase no
            query = @"
select id from tbl_purchase_no 
where p_purchase_no = @purchase_no; ";
            var purchase = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                new MySql.Data.MySqlClient.MySqlParameter("@purchase_no", Login.Operator["purchase_no"]));

            if (purchase == null)
            {
                string insert = @"
insert into tbl_purchase_no(`p_purchase_no`, `p_description`, `p_create_time`) 
VALUES (@purchase_no, @description, @create_time);";
                var ret = MySqlHelper.ExecuteNonQuery(MySqlHelper.ConnectionString, insert,
                    new MySql.Data.MySqlClient.MySqlParameter("@purchase_no", Login.Operator["purchase_no"]),
                    new MySql.Data.MySqlClient.MySqlParameter("@description", Login.Operator["purchase_no"]),
                    new MySql.Data.MySqlClient.MySqlParameter("@create_time", DateTime.Now)
                    );
            }

            purchase = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                new MySql.Data.MySqlClient.MySqlParameter("@purchase_no", Login.Operator["purchase_no"]));

            if (purchase != null)
            {
                Login.Operator["purchase_id"] = purchase;
            }
            #endregion

            #region work_station
            query_work_station_id();
            #endregion
            string ip_address = common.GetIPAddress();
            Login.Operator["site"] = query_site_string(ip_address);
        }

        static void query_work_station_id()
        {
            string computer_name = common.GetComputerName();
            string user_name = common.GetUserName();
            string user_domain_name = common.GetUserDomainName();

            string query = @"
select id from tbl_work_station 
where w_station = @station 
and w_user_name = @user_name;";
            var ret = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                new MySql.Data.MySqlClient.MySqlParameter("@station", computer_name),
                new MySql.Data.MySqlClient.MySqlParameter("@user_name", user_name));
            if (ret != null)
            {// 同一台机器， 同一个帐号登录， 则返回id号
                Login.Operator["work_station_id"] = ret;
                return;
            }

            // 插入新登录的帐号， 并查询返回id号
            string insert = @"
insert into tbl_work_station(`w_station`, `w_user_name`, `w_user_domain_name`, `w_create_time`) 
VALUES (@station, @user_name, @user_domain_name, @create_time);";
            ret = MySqlHelper.ExecuteNonQuery(MySqlHelper.ConnectionString, insert,
                new MySql.Data.MySqlClient.MySqlParameter("@station", computer_name),
                new MySql.Data.MySqlClient.MySqlParameter("@user_name", user_name),
                new MySql.Data.MySqlClient.MySqlParameter("@user_domain_name", user_domain_name),
                new MySql.Data.MySqlClient.MySqlParameter("@create_time", DateTime.Now)
                );

            ret = MySqlHelper.ExecuteScalar(MySqlHelper.ConnectionString, query,
                new MySql.Data.MySqlClient.MySqlParameter("@station", computer_name),
                new MySql.Data.MySqlClient.MySqlParameter("@user_name", user_name));
            if (ret != null)
            {
                Login.Operator["work_station_id"] = ret;
            }
        }

        static string query_site_string(string ip_address)
        {
            string[] ip_nodes = ip_address.Split('.');
            if (ip_nodes.Length <= 2)
                return null;
            string ip_id = ip_nodes[1];
            string query = @"
select i_en_code, i_ch_code from tbl_ip_dictionary 
where id = @id;";

            DataRow site_code = MySqlHelper.ExecuteDataRow(MySqlHelper.ConnectionString, query,
                new MySql.Data.MySqlClient.MySqlParameter("@id", ip_id));
            string site_string = String.Empty;
            if (site_code == null)
            {
                site_string = ip_address;
            }
            else
            {
                site_string = String.Format("{0}({1})", site_code["i_ch_code"], site_code["i_en_code"]);
            }
            return site_string;
        }

    }
}
