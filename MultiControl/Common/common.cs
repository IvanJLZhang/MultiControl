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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiControl.Common
{
    class common
    {
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
                string name = System.Environment.UserDomainName;
                if (string.IsNullOrEmpty(name))
                {
                    name = System.Environment.MachineName;
                }
                //return System.Environment.GetEnvironmentVariable("ComputerName");
                return name;
            }
            catch
            {
                return "unknown";
            }
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

        public static ArrayList GetDeviceList(String result)
        {
            Debug.WriteLine(result);
            ArrayList deviceList = new ArrayList();
            string[] devices = Regex.Split(result, "\r\n", RegexOptions.IgnoreCase);
            foreach (string device in devices)
            {
                if (device.Contains("\tdevice"))
                {
                    string[] serials = Regex.Split(device, "\t", RegexOptions.IgnoreCase);
                    deviceList.Add(serials[0]);
                }
            }
            return deviceList;
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
    }
}
