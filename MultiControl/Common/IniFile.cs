using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MultiControl
{
    class IniFile
    {
        public string Path;

        public IniFile(string path)
        {
            this.Path = path;
        }

        #region 声明读写INI文件的API函数 
        [DllImport("kernel32")] 
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath); 

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath); 

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion

        public void IniWriteValue(string section, string key, string iValue) 
        {
            WritePrivateProfileString(section, key, iValue, this.Path);
        }

        public string IniReadValue(string section, string key) 
        { 
            StringBuilder temp = new StringBuilder(255); 

            int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path); 
            return temp.ToString();
        }

        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];

            int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
            return temp;
        }

    }
}
