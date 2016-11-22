/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/8 14:09:47 
 * 文件名：UsbDeviceInfo 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiControl.Common
{
    public class UsbDeviceInfoEx : IEqualityComparer<UsbDeviceInfoEx>
    {
        public string SerialNumber = String.Empty;
        public string Port_Path = String.Empty;
        public Int32 Port_Index = -1;
        public string ModelName = String.Empty;
        public Int32 Index = -1;

        public Int32 idVender = 0;
        public Int32 idProduct = 0;
        public string Product = String.Empty;
        /// <summary>
        /// 由Port path和port index组成的port值
        /// </summary>
        public string Port
        {
            get
            {
                return $"#{Port_Index}--{Port_Path}";
            }
        }
        public bool Equals(UsbDeviceInfoEx x, UsbDeviceInfoEx y)
        {
            return x.SerialNumber.Equals(y.SerialNumber);
        }

        public int GetHashCode(UsbDeviceInfoEx obj)
        {
            return obj.SerialNumber.GetHashCode();
        }

        public static UsbDeviceInfoEx Default = new UsbDeviceInfoEx();
    }
}
