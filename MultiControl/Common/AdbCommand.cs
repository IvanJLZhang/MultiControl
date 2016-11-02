/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/27 10:19:41 
 * 文件名：AdbCommand 
 * 版本：V1.0.0 
 * 文件说明：
 * 常用adb命令字符串集
 * 
 * 修改者：           
 * 时间：               
 * 修改说明： 
* ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiControl.Common
{
    class AdbCommand
    {
        public const string Adb_kill_server = "adb kill-server";

        public const string Adb_start_server = "adb start-server";

        public const string Adb_devices = "adb devices";
    }
}
