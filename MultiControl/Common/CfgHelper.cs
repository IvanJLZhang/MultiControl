/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/26 17:26:31 
 * 文件名：CfgHelper 
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
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MultiControl.Common
{
    public class ConfigurationHelper
    {
        /// <summary>
        /// 读取key值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadConfig(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            if (config.AppSettings.Settings.AllKeys.Contains(key))
            {
                return config.AppSettings.Settings[key].Value;
            }
            return String.Empty;
        }
        /// <summary>
        /// 写入config值， 如果key不存在的话则新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteConfig(string key, object value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            if (config.AppSettings.Settings.AllKeys.Contains(key))
            {
                config.AppSettings.Settings[key].Value = value.ToString();

            }
            else
            {
                config.AppSettings.Settings.Add(key, value.ToString());
            }

            config.Save(ConfigurationSaveMode.Modified);

            // 写入以后刷新结点
            ConfigurationManager.RefreshSection("appSettings");
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteConfig(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            if (config.AppSettings.Settings.AllKeys.Contains(key))
            {
                config.AppSettings.Settings.Remove(key);
            }
        }

        /// <summary>
        /// 初始化config档
        /// </summary>
        public static void Initialize()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            config.AppSettings.Settings.Clear();

            config.Save(ConfigurationSaveMode.Full);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
