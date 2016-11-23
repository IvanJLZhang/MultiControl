/* ----------------------------------------------------------
文件名称：SetupApi.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：
    V1.0	2011年09月05日
			实现对setupapi.dll接口的PInvoke

参考资料：
    http://www.pinvoke.net/
------------------------------------------------------------ */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Splash.IO.PORTS
{    
    /// <summary>
    /// SetupApi.dll的PInvoke
    /// </summary>
    public partial class USB
    {
        #region ENUM
        [Flags]
        private enum DIGCF
        {
            DIGCF_DEFAULT = 0x00000001,
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,          // 设备安装类
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010,     // 设备接口类
        }
        #endregion

        #region STRUCT
        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public static readonly SP_DEVINFO_DATA Empty = new SP_DEVINFO_DATA(Marshal.SizeOf(typeof(SP_DEVINFO_DATA)));
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public IntPtr Reserved;

            private SP_DEVINFO_DATA(int size)
            {
                cbSize = (UInt32)size;
                ClassGuid = Guid.Empty;
                DevInst = 0;
                Reserved = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public static readonly SP_DEVICE_INTERFACE_DATA Empty = new SP_DEVICE_INTERFACE_DATA(Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA)));
            public UInt32 cbSize;
            public Guid InterfaceClassGuid;
            public UInt32 Flags;
            public UIntPtr Reserved;

            private SP_DEVICE_INTERFACE_DATA(int size)
            {
                cbSize = (uint)size;
                InterfaceClassGuid = Guid.Empty;
                Flags = 0;
                Reserved = UIntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String DevicePath;
        }
        #endregion

        #region API
        #region SetupDiGetClassDevs
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] String Enumerator,
            IntPtr hwndParent,
            DIGCF Flags
            );        

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevs(
            IntPtr ClassGuid,       // null 
            String Enumerator,
            IntPtr hwndParent,
            DIGCF Flags
            );
        #endregion

        #region SetupDiGetClassDevsEx
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevsEx(
            ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] String Enumerator,
            IntPtr hwndParent,
            DIGCF Flags,
            IntPtr DeviceInfoSet,
            [MarshalAs(UnmanagedType.LPTStr)] String MachineName,
            IntPtr Reserved
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevsEx(
            IntPtr ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] String Enumerator,
            IntPtr hwndParent,
            DIGCF Flags,
            IntPtr DeviceInfoSet,
            [MarshalAs(UnmanagedType.LPTStr)] String MachineName,
            IntPtr Reserved
            );           
        #endregion

        #region SetupDiEnumDeviceInterfaces
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInterfaces(
            IntPtr hDevInfo,
            ref SP_DEVINFO_DATA devInfo,
            ref Guid interfaceClassGuid,
            UInt32 memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInterfaces(
            IntPtr hDevInfo,
            [MarshalAs(UnmanagedType.AsAny)] Object devInfo,
            ref Guid interfaceClassGuid,
            UInt32 memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
            );
        #endregion

        #region SetupDiGetDeviceInterfaceDetail
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            Int32 deviceInterfaceDetailDataSize,
            out Int32 requiredSize,
            ref SP_DEVINFO_DATA deviceInfoData
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            Int32 deviceInterfaceDetailDataSize,
            out Int32 requiredSize,
            [MarshalAs(UnmanagedType.AsAny)] object deviceInfoData      // null
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            Int32 deviceInterfaceDetailDataSize,
            IntPtr requiredSize,                                        // null
            [MarshalAs(UnmanagedType.AsAny)] object deviceInfoData      // null
            );
        #endregion

        #region SetupDiCreateDeviceInfoList
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiCreateDeviceInfoList(IntPtr ClassGuid, IntPtr hwndParent);
        #endregion

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern Boolean SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, UInt32 MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        #region SetupDiGetDeviceInstanceId
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiGetDeviceInstanceId(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            StringBuilder DeviceInstanceId,
            Int32 DeviceInstanceIdSize,
            out Int32 RequiredSize
            );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiGetDeviceInstanceId(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            StringBuilder DeviceInstanceId,
            Int32 DeviceInstanceIdSize,
            IntPtr RequiredSize
            );
        #endregion        
        #endregion
    }
}
