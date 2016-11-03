/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/2 15:12:50 
 * 文件名：config_inc 
 * 版本：V1.0.0 
 * 文件说明：
 * 定义相关配置信息与常量的类
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
using System.Threading.Tasks;

namespace MultiControl.Common
{
    public class config_inc
    {
        public const string PQAA_SW_VERSION = "V1.4.0";
        /// <summary>
        /// 文件根目录
        /// </summary>
        public const string CFG_FILE_ROOT = "/Android/data/com.wistron.generic.pqaa/files/";
        /// <summary>
        /// PQAA config目录
        /// </summary>
        public const string CFG_FILE_PQAA = CFG_FILE_ROOT + "pqaa_config";

        public const string SPECIFIC_TAG_PATH = CFG_FILE_ROOT + "path.verify.pass";
    }

    public enum TEST_RESULT{
        PASS = 1, FAIL
    }
}
