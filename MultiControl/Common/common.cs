/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/27 9:21:25 
 * 文件名：common 
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LogHelper;

namespace MultiControl.Common
{
    class common
    {
        public static LogMsg m_log;

        /// <summary>
        /// 获取MD5字符串
        /// </summary>
        /// <param name="oldStr"></param>
        /// <returns></returns>
        public static string GetMD5Code(string oldStr)
        {
            //将输入转换为ASCII 字符编码
            ASCIIEncoding enc = new ASCIIEncoding();
            //将字符串转换为字节数组
            byte[] buffer = enc.GetBytes(oldStr);
            //创建MD5实例
            MD5 md5 = new MD5CryptoServiceProvider();
            //进行MD5加密
            byte[] hash = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            //拼装加密后的字符
            for (int i = 0; i < hash.Length; i++)
            {
                sb.AppendFormat("{0:x2}", hash[i]);
            }
            //输出加密后的字符串
            return sb.ToString();
        }
        /// <summary>
        /// 判断是否是数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 将content内容写入file中， 当文件不存在时创建
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        public static void CreateFile(string file, string content)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(content);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取电脑名称
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch
            {
                return "unknown";
            }
        }
        public static string GetUserName()
        {
            try
            {
                return System.Environment.UserName;
            }
            catch
            {
                return "unknown";
            }
        }

        public static string GetUserDomainName()
        {
            try
            {
                return System.Environment.UserDomainName;
            }
            catch
            {
                return "unknown";
            }
        }

        public static string GetIPAddress()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        /// <summary>
        /// 筛选出command命令执行的结果
        /// </summary>
        /// <param name="response"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string FilterCommandResult(string response, string command)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(response))
                return result;

            string[] arr = Regex.Split(response, "\r\n", RegexOptions.IgnoreCase);
            int start = 0, end = 0;
            for (int index = 0; index != arr.Length; index++)
            {
                var str = arr[index];
                if (str.Contains(command))
                    start = index + 1;
                if (str.Contains(">exit"))
                    end = index;
            }
            if (start > end)
                return String.Empty;
            for (int index = start; index < end; index++)
            {
                if (!String.IsNullOrEmpty(arr[index]))
                    result += arr[index] + "\r\n";
            }
            return result.Trim();
        }
        /// <summary>
        /// 从command执行中筛选出model
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string FilterModelName(string result)
        {
            string model = String.Empty;
            if (String.IsNullOrEmpty(result))
                return model;

            string[] arr = Regex.Split(result, "\r\n");
            foreach (var o in arr)
            {
                if (!o.Contains("*") && !String.IsNullOrEmpty(o))
                {
                    model += o;
                }
            }
            return model;
        }

        /// <summary>
        /// 索引device
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static int IndexDevice(DutDevice[] deviceList, string sn)
        {
            for (int index = 0; index < deviceList.Length; index++)
            {
                var device = deviceList[index];
                if (!String.IsNullOrEmpty(device.SerialNumber) && device.SerialNumber == sn)
                    return index;
            }
            return -1;
        }
        /// <summary>
        /// 静态方法： 删除Excel进程
        /// </summary>
        public static void DeleteConhostExe()
        {
            Process[] ExcelProcess = Process.GetProcessesByName("conhost");
            foreach (var o in ExcelProcess)
                if (o.MainWindowTitle == "")
                {
                    try
                    {
                        o.Kill();
                    }
                    catch { }
                }
        }

        public static Int32 ConvertVersionStringToInt(string version)
        {// V1.6.5
            if (String.IsNullOrEmpty(version))
                return -1;
            string return_value = String.Empty;
            version = version.Replace("V", "");
            string[] versions = version.Split('.');
            foreach (var item in versions)
            {
                return_value += item;
            }
            int return_value_int = -1;
            Int32.TryParse(return_value, out return_value_int);
            return return_value_int;
        }
    }
}
