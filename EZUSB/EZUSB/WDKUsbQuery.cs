/* ----------------------------------------------------------
文件名称：WDKUsbQuery.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年09月05日
			基于WDK获取设备路径集合
------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Splash.IO.PORTS
{
    /// <summary>
    /// 基于WDK获取设备路径集合
    /// </summary>
    public partial class USB
    {
        /// <summary>
        /// 获取同时指定设备安装类GUID和设备接口类GUID的设备的设备路径集合
        /// </summary>
        /// <param name="setupClassGuid">设备安装类GUID，Empty忽视</param>
        /// <param name="interfaceClassGuid">设备接口类GUID</param>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetDevicePath(Guid setupClassGuid, Guid interfaceClassGuid, String Enumerator = null)
        {   // 设备接口类GUID
            if (interfaceClassGuid == Guid.Empty) return null;

            // 根据设备安装类GUID创建空的设备信息集合
            IntPtr DeviceInfoSet;
            if (setupClassGuid == Guid.Empty)
                DeviceInfoSet = SetupDiCreateDeviceInfoList(IntPtr.Zero, IntPtr.Zero);
            else
                DeviceInfoSet = SetupDiCreateDeviceInfoList(ref setupClassGuid, IntPtr.Zero);

            if (DeviceInfoSet == new IntPtr(-1)) return null;

            // 根据设备接口类GUID获取新的设备信息集合
            IntPtr hDevInfo = SetupDiGetClassDevsEx(
                ref interfaceClassGuid,
                Enumerator,
                IntPtr.Zero,
                DIGCF.DIGCF_DEVICEINTERFACE | DIGCF.DIGCF_PRESENT,
                DeviceInfoSet,
                null,
                IntPtr.Zero);
            if (hDevInfo == new IntPtr(-1)) return null;

            // 枚举所有设备
            List<String> DevicePathList = new List<String>();

            // 获取设备接口
            UInt32 MemberIndex = 0;
            SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = SP_DEVICE_INTERFACE_DATA.Empty;
            while (SetupDiEnumDeviceInterfaces(hDevInfo, null, ref interfaceClassGuid, MemberIndex++, ref DeviceInterfaceData))
            {   // 获取接口细节             
                SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();

                // 区分32位操作系统和64位操作系统
                DeviceInterfaceDetailData.cbSize = (UInt32)((IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
                if (SetupDiGetDeviceInterfaceDetail(hDevInfo,
                    ref DeviceInterfaceData,
                    ref DeviceInterfaceDetailData,
                    Marshal.SizeOf(DeviceInterfaceDetailData),
                    IntPtr.Zero,
                    null))
                {   // 获取设备路径
                    DevicePathList.Add(DeviceInterfaceDetailData.DevicePath);
                }
            }

            SetupDiDestroyDeviceInfoList(hDevInfo);

            if (DevicePathList.Count == 0)
                return null;
            else
                return DevicePathList.ToArray();
        }

        /// <summary>
        /// 获取具有指定设备接口类GUID的设备的设备路径集合
        /// </summary>
        /// <param name="interfaceClassGuid">设备接口类GUID</param>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetDevicePath(Guid interfaceClassGuid, String Enumerator = null)
        {
            return GetDevicePath(Guid.Empty, interfaceClassGuid, Enumerator);
        }

        /// <summary>
        /// 获取指定设备安装类GUID和USB接口的设备的设备路径集合
        /// </summary>
        /// <param name="setupClassGuid">设备安装类GUID，Empty忽视</param>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetUsbDevicePath(Guid setupClassGuid, String Enumerator = null)
        {
            return GetDevicePath(setupClassGuid, new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}"), Enumerator);
        }

        /// <summary>
        /// 获取指定USB设备的设备路径集合
        /// </summary>
        /// <param name="PnPEntity">即插即用设备实体信息</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetUsbDevicePath(PnPEntityInfo PnPEntity)
        {
            return GetDevicePath(PnPEntity.ClassGuid, new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}"), PnPEntity.PNPDeviceID);
        }

        /// <summary>
        /// 获取具有USB接口的设备的设备路径集合
        /// </summary>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetUsbDevicePath(String Enumerator = null)
        {
            return GetDevicePath(Guid.Empty, new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}"), Enumerator);
        }         

        /// <summary>
        /// 获取指定设备安装类GUID和HID接口的设备的设备路径集合
        /// </summary>
        /// <param name="setupClassGuid">设备安装类GUID，Empty忽视</param>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetHidDevicePath(Guid setupClassGuid, String Enumerator = null)
        {
            return GetDevicePath(setupClassGuid, new Guid("{4D1E55B2-F16F-11CF-88CB-001111000030}"), Enumerator);
        }

        /// <summary>
        /// 获取指定HID设备的设备路径集合
        /// </summary>
        /// <param name="PnPEntity">即插即用设备实体信息</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetHidDevicePath(PnPEntityInfo PnPEntity)
        {
            return GetDevicePath(PnPEntity.ClassGuid, new Guid("{4D1E55B2-F16F-11CF-88CB-001111000030}"), PnPEntity.PNPDeviceID);
        }

        /// <summary>
        /// 获取具有HID接口的设备的设备路径集合
        /// </summary>
        /// <param name="Enumerator">枚举器：USB、PCI、PCMCIA、SCSI或者设备实例ID</param>
        /// <returns>设备路径集合</returns>
        public static String[] GetHidDevicePath(String Enumerator = null)
        {
            return GetDevicePath(Guid.Empty, new Guid("{4D1E55B2-F16F-11CF-88CB-001111000030}"), Enumerator);
        }            
    }
}
