/* ----------------------------------------------------------
文件名称：Environment.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年10月10日
			获取系统语言设置
------------------------------------------------------------ */
using System;
using System.Runtime.InteropServices;

namespace Splash
{
    public class Environment
    {
        [DllImport("kernel32.dll")]
        private static extern UInt16 GetUserDefaultUILanguage();

        [DllImport("kernel32.dll")]
        private static extern UInt16 GetSystemDefaultLangID();

        [DllImport("kernel32.dll")]
        private static extern UInt16 GetUserDefaultLangID();

        /// <summary>
        /// 用户缺省界面语言
        /// </summary>
        public static UInt16 UserDefaultUILanguage
        {
            get
            {
                return GetUserDefaultUILanguage();
            }
        }

        /// <summary>
        /// 用户缺省语言
        /// </summary>
        public static UInt16 UserDefaultLangID
        {
            get
            {
                return GetUserDefaultLangID();
            }
        }

        /// <summary>
        /// 系统缺省语言
        /// </summary>
        public static UInt16 SystemDefaultLangID
        {
            get
            {
                return GetSystemDefaultLangID();
            }
        }        
    }
}
