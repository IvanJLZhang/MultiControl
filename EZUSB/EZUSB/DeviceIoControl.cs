/* ----------------------------------------------------------
文件名称：DeviceIoControl.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：
    V1.0	2011年10月10日
			实现对DeviceIoControl接口的PInvoke

参考资料：
    http://www.pinvoke.net/
------------------------------------------------------------ */
using System;
using System.Runtime.InteropServices;

namespace Splash.IO.PORTS
{
    #region ENUM
    public enum USB_HUB_NODE : uint
    {
        UsbHub,
        UsbMIParent
    }

    public enum USB_CONNECTION_STATUS
    {
        NoDeviceConnected,
        DeviceConnected,
        DeviceFailedEnumeration,
        DeviceGeneralFailure,
        DeviceCausedOvercurrent,
        DeviceNotEnoughPower,
        DeviceNotEnoughBandwidth,
        DeviceHubNestedTooDeeply,
        DeviceInLegacyHub
    }

    public enum USB_DEVICE_SPEED : byte
    {
        UsbLowSpeed,    // 低速USB 1.1
        UsbFullSpeed,   // 全速USB 1.1
        UsbHighSpeed,   // 高速USB 2.0
        UsbSuperSpeed   // 极速USB 3.0
    }
    #endregion

    public partial class USB
    {
        internal const Int32 IOCTL_GET_HCD_DRIVERKEY_NAME = 0x220424;
        internal const Int32 IOCTL_USB_GET_ROOT_HUB_NAME = 0x220408;
        internal const Int32 IOCTL_USB_GET_NODE_CONNECTION_NAME = 0x220414;
        internal const Int32 IOCTL_USB_GET_NODE_INFORMATION = 0x220408;
        internal const Int32 IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX = 0x220448;
        internal const Int32 IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = 0x220410;

        internal const Int32 MAXIMUM_USB_STRING_LENGTH = 255;
        internal const Int32 USB_STRING_DESCRIPTOR_TYPE = 3;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct USB_HCD_DRIVERKEY_NAME
        {
            public Int32 ActualLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String Name;
        }

        #region USB_NODE_INFORMATION
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct USB_HUB_DESCRIPTOR
        {
            public Byte bDescriptorLength;
            public Byte bDescriptorType;    // 描述符类型：0x29
            public Byte bNumberOfPorts;     // 支持的下游端口数目
            public Int16 wHubCharacteristics;   // 特征描述
            public Byte bPowerOnToPowerGood;    // 从端口加电到端口正常工作的时间间隔(以2ms为单位)
            public Byte bHubControlCurrent;     // 设备所需最大电流
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public Byte[] bRemoveAndPowerMask;  // 指示连接在集线器端口的设备是否可移走
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_HUB_INFORMATION
        {
            public USB_HUB_DESCRIPTOR HubDescriptor;
            public Byte HubIsBusPowered;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_MI_PARENT_INFORMATION
        {
            public Int32 NumberOfInterfaces;
        };
        
        [StructLayout(LayoutKind.Explicit)]
        internal struct UsbNodeUnion
        {
            [FieldOffset(0)]
            public USB_HUB_INFORMATION HubInformation;
            [FieldOffset(0)]
            public USB_MI_PARENT_INFORMATION MiParentInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_NODE_INFORMATION
        {
            public USB_HUB_NODE NodeType;
            public UsbNodeUnion u;
        }
        #endregion

        #region USB_NODE_CONNECTION_INFORMATION
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct USB_DEVICE_DESCRIPTOR
        {
            public Byte bLength;
            public Byte bDescriptorType;
            public UInt16 bcdUSB;
            public Byte bDeviceClass;
            public Byte bDeviceSubClass;
            public Byte bDeviceProtocol;
            public Byte bMaxPacketSize0;
            public UInt16 idVendor;
            public UInt16 idProduct;
            public UInt16 bcdDevice;
            public Byte iManufacturer;
            public Byte iProduct;
            public Byte iSerialNumber;
            public Byte bNumConfigurations;
        }       

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct USB_ENDPOINT_DESCRIPTOR
        {
            public Byte bLength;
            public Byte bDescriptorType;
            public Byte bEndpointAddress;
            public Byte bmAttributes;
            public UInt16 wMaxPacketSize;
            public Byte bInterval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct USB_PIPE_INFO
        {
            public USB_ENDPOINT_DESCRIPTOR EndpointDescriptor;
            public UInt32 ScheduleOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct USB_NODE_CONNECTION_INFORMATION_EX
        {
            public Int32 ConnectionIndex;
            public USB_DEVICE_DESCRIPTOR DeviceDescriptor;
            public Byte CurrentConfigurationValue;
            public Byte Speed;
            public Byte DeviceIsHub;
            public Int16 DeviceAddress;
            public Int32 NumberOfOpenPipes;
            public USB_CONNECTION_STATUS ConnectionStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public USB_PIPE_INFO[] PipeList;
        }
        #endregion

        #region USB_DESCRIPTOR_REQUEST
        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_SETUP_PACKET
        {
            public Byte bmRequest;
            public Byte bRequest;
            public UInt16 wValue;
            public UInt16 wIndex;
            public UInt16 wLength;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct USB_STRING_DESCRIPTOR
        {
            public Byte bLength;
            public Byte bDescriptorType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXIMUM_USB_STRING_LENGTH)]
            public String bString;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_DESCRIPTOR_REQUEST
        {
            public Int32 ConnectionIndex;
            public USB_SETUP_PACKET SetupPacket;       
            public USB_STRING_DESCRIPTOR Data;
        }
        #endregion

        #region USB_NODE_CONNECTION_DRIVERKEY_NAME
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct USB_NODE_CONNECTION_DRIVERKEY_NAME
        {
            public Int32 ConnectionIndex;
            public Int32 ActualLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXIMUM_USB_STRING_LENGTH)]
            public String DriverKeyName;
        }
        #endregion

        #region DeviceIoControl
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            IntPtr lpInBuffer,
            Int32 nInBufferSize,
            ref USB_HCD_DRIVERKEY_NAME lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 nBytesReturned,
            IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            ref USB_NODE_INFORMATION lpInBuffer,
            Int32 nInBufferSize,
            ref USB_NODE_INFORMATION lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 lpBytesReturned,
            IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            ref USB_NODE_CONNECTION_INFORMATION_EX lpInBuffer,
            Int32 nInBufferSize,
            ref USB_NODE_CONNECTION_INFORMATION_EX lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 lpBytesReturned,
            IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            ref USB_DESCRIPTOR_REQUEST lpInBuffer,
            Int32 nInBufferSize,
            ref USB_DESCRIPTOR_REQUEST lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 lpBytesReturned,
            IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            ref USB_NODE_CONNECTION_DRIVERKEY_NAME lpInBuffer,
            Int32 nInBufferSize,
            ref USB_NODE_CONNECTION_DRIVERKEY_NAME lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 lpBytesReturned,
            IntPtr lpOverlapped
            );
        #endregion
    }
}
