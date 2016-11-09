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
    public class UsbDeviceInfo : IEqualityComparer<UsbDeviceInfo>
    {
        public string SerialNumber = String.Empty;
        public string PortNumber = String.Empty;
        public string ModelName = String.Empty;
        public Int32 Index = -1;

        public bool Equals(UsbDeviceInfo x, UsbDeviceInfo y)
        {
            return x.SerialNumber.Equals(y.SerialNumber);
        }

        public int GetHashCode(UsbDeviceInfo obj)
        {
            return obj.SerialNumber.GetHashCode();
        }

        public static UsbDeviceInfo Default = new UsbDeviceInfo();
    }
}
