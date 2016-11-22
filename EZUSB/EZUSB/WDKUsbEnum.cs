/* ----------------------------------------------------------
文件名称：WDKUsbEnum.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年10月10日
			基于WDK枚举USB设备
------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace Splash.IO.PORTS
{
    /// <summary>
    /// USB主控制器信息
    /// </summary>
    public struct HostControllerInfo
    {
        public String PNPDeviceID;      // 设备ID
        public String Name;             // 设备名称
        public String HcdDriverKeyName; // 驱动键名
    }

    /// <summary>
    /// USB Hub信息
    /// </summary>
    public struct UsbHubInfo
    {
        public String PNPDeviceID;      // 设备ID
        public String Name;             // 设备名称
        public String Status;           // 设备状态
    }

    /// <summary>
    /// USB HUB节点信息
    /// </summary>
    public struct UsbNodeInformation
    {
        public String DevicePath;           // 设备路径
        public String PNPDeviceID;          // 设备ID        
        public String Name;                 // 设备名称

        public USB_HUB_NODE NodeType;       // 节点类型

        // USB_HUB_INFORMATION
        public Boolean HubIsBusPowered;     // 供电方式：true-总线供电 false-独立供电
        public Int32 NumberOfPorts;         // 端口数
        public Int16 HubCharacteristics;    // 特征描述
        public Byte PowerOnToPowerGood;     // 从端口加电到端口正常工作的时间间隔(以2ms为单位)
        public Byte HubControlCurrent;      // 设备所需最大电流

        // USB_MI_PARENT_INFORMATION
        public Int32 NumberOfInterfaces;    // 接口数
    }

    /// <summary>
    /// USB设备描述符
    /// </summary>
    public struct UsbDeviceDescriptor
    {
        public Byte bDescriptorType;    // 描述符类型 USB_DEVICE_DESCRIPTOR_TYPE
        public String UsbVersion;       // USB规格版本号
        public Byte bDeviceClass;       // 设备类型
        public Byte bDeviceSubClass;    // 设备子类型
        public Byte bDeviceProtocol;    // 设备协议
        public Byte bMaxPacketSize0;    // 最大封包大小
        public UInt16 idVendor;         // VID
        public UInt16 idProduct;        // PID
        public String DeviceVersion;    // 设备版本号
        public String Manufacturer;     // 制造商
        public String Product;          // 产品描述
        public String SerialNumber;     // 序列号
        public Byte bNumConfigurations; // 配置总数
    }

    /// <summary>
    /// USB管道信息
    /// </summary>
    public struct UsbPipeInfo
    {
        public UInt32 ScheduleOffset;
        public Byte bDescriptorType;
        public Byte bEndpointAddress;
        public Byte bmAttributes;
        public UInt16 wMaxPacketSize;
        public Byte bInterval;
    }

    /// <summary>
    /// USB节点连接信息
    /// </summary>
    public struct UsbNodeConnectionInformation
    {
        public String DevicePath;           // 设备路径
        public Int32 ConnectionIndex;       // 端口号

        public UsbDeviceDescriptor DeviceDescriptor;

        public Byte CurrentConfigurationValue;  // 当前设备配置
        public Byte Speed;                  // 设备速度
        public Boolean DeviceIsHub;         // 是否是集线器        
        public Int32 DeviceAddress;         // 设备地址
        public Int32 NumberOfOpenPipes;     // 管道数
        public USB_CONNECTION_STATUS ConnectionStatus;  // 连接状态

        public List<UsbPipeInfo> PipeList;  // 管道信息
    }

    /// <summary>
    /// 外接Hub设备信息
    /// </summary>
    public struct ExternalHubInfo
    {
        public UsbNodeInformation NodeInfo;    // 外接Hub设备节点信息
        public UsbNodeConnectionInformation NodeConnectionInfo; // 外接Hub设备节点连接信息               
    }

    /// <summary>
    /// USB设备枚举
    /// </summary>
    public partial class USB
    {
        #region HostController
        /// <summary>
        /// USB主控制器
        /// </summary>
        public static HostControllerInfo[] AllHostControllers
        {
            get
            {
                List<HostControllerInfo> HostControllers = new List<HostControllerInfo>();

                // 获取USB控制器及其相关联的设备实体
                ManagementObjectCollection MOC = new ManagementObjectSearcher("SELECT * FROM Win32_USBController").Get();
                if (MOC != null)
                {
                    foreach (ManagementObject MO in MOC)
                    {
                        HostControllerInfo Element;
                        Element.PNPDeviceID = MO["PNPDeviceID"] as String;  // 设备ID
                        Element.Name = MO["Name"] as String;    // 设备描述
                        Element.HcdDriverKeyName = USB.GetHcdDriverKeyName(Element.PNPDeviceID);
                        HostControllers.Add(Element);
                    }
                }

                if (HostControllers.Count == 0) return null; else return HostControllers.ToArray();
            }
        }

        /// <summary>
        /// 获取驱动键名
        /// </summary>
        /// <param name="PNPDeviceID">USB主控制器设备ID</param>
        /// <returns>获取设备驱动在注册表HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class下的路径</returns>
        public static String GetHcdDriverKeyName(String PNPDeviceID)
        {
            if (String.IsNullOrEmpty(PNPDeviceID)) return null;

            // 打开设备
            IntPtr hHCDev = Kernel32.CreateFile(
                "\\\\.\\" + PNPDeviceID.Replace('\\', '#') + "#{3ABF6F2D-71C4-462A-8A92-1E6861E6AF27}",
                NativeFileAccess.GENERIC_WRITE,
                NativeFileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeFileMode.OPEN_EXISTING,
                IntPtr.Zero,
                IntPtr.Zero);
            if (hHCDev == Kernel32.INVALID_HANDLE_VALUE) return null;

            // 获取驱动键名
            Int32 nBytesReturned;
            USB_HCD_DRIVERKEY_NAME Buffer = new USB_HCD_DRIVERKEY_NAME();
            Boolean Status = DeviceIoControl(hHCDev,
                IOCTL_GET_HCD_DRIVERKEY_NAME,
                IntPtr.Zero,
                0,
                ref Buffer,
                Marshal.SizeOf(Buffer),
                out nBytesReturned,
                IntPtr.Zero);

            // 关闭设备
            Kernel32.CloseHandle(hHCDev);
            return Status ? Buffer.Name : null;
        }
        #endregion

        #region USBHUB
        /// <summary>
        /// USB Hub信息集合
        /// </summary>
        public static UsbHubInfo[] AllUsbHubs
        {
            get
            {
                List<UsbHubInfo> UsbHubs = new List<UsbHubInfo>();

                // 获取USB控制器及其相关联的设备实体
                ManagementObjectCollection MOC = new ManagementObjectSearcher("SELECT * FROM Win32_USBHub").Get();
                if (MOC != null)
                {
                    foreach (ManagementObject MO in MOC)
                    {
                        UsbHubInfo Element;
                        Element.PNPDeviceID = MO["PNPDeviceID"] as String;  // 设备ID
                        Element.Name = MO["Name"] as String;        // 设备描述    
                        Element.Status = MO["Status"] as String;    // 设备状态
                        UsbHubs.Add(Element);
                    }
                }

                if (UsbHubs.Count == 0) return null; else return UsbHubs.ToArray();
            }
        }

        /// <summary>
        /// USB ROOT HUB设备路径
        /// </summary>
        /// <param name="PNPDeviceID">USB主控制器设备ID</param>
        /// <returns>USB ROOT HUB设备路径</returns>
        public static String GetUsbRootHubPath(String PNPDeviceID)
        {
            if (String.IsNullOrEmpty(PNPDeviceID)) return null;

            // 打开设备
            IntPtr hHCDev = Kernel32.CreateFile(
                "\\\\.\\" + PNPDeviceID.Replace('\\', '#') + "#{3ABF6F2D-71C4-462A-8A92-1E6861E6AF27}",
                NativeFileAccess.GENERIC_WRITE,
                NativeFileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeFileMode.OPEN_EXISTING,
                IntPtr.Zero,
                IntPtr.Zero);
            if (hHCDev == Kernel32.INVALID_HANDLE_VALUE) return null;

            // 获取USB ROOT HUB名称
            Int32 nBytesReturned;
            USB_HCD_DRIVERKEY_NAME Buffer = new USB_HCD_DRIVERKEY_NAME();
            Boolean Status = DeviceIoControl(hHCDev,
                IOCTL_USB_GET_ROOT_HUB_NAME,
                IntPtr.Zero,
                0,
                ref Buffer,
                Marshal.SizeOf(Buffer),
                out nBytesReturned,
                IntPtr.Zero);

            // 关闭设备
            Kernel32.CloseHandle(hHCDev);
            return Status ? Buffer.Name : null;
        }

        /// <summary>
        /// USB HUB设备名称
        /// </summary>
        /// <param name="DevicePath">设备路径</param>
        /// <returns>设备名称</returns>
        public static String GetUsbHubName(String DevicePath)
        {
            if (String.IsNullOrEmpty(DevicePath)) return null;

            // 从设备路径中提取设备ID
            String DeviceID = DevicePath.Substring(0, DevicePath.LastIndexOf('#')).Replace('#', '_');

            // 从Win32_USBHub获取设备描述
            ManagementObjectCollection MOC = new ManagementObjectSearcher("SELECT * FROM Win32_USBHub WHERE DeviceID LIKE '" + DeviceID + "'").Get();
            if (MOC != null)
            {
                foreach (ManagementObject MO in MOC)
                {
                    return MO["Name"] as String;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取USB HUB节点信息
        /// </summary>
        /// <param name="DevicePath">USB HUB设备路径</param>
        /// <returns>节点信息</returns>
        public static UsbNodeInformation[] GetUsbNodeInformation(String DevicePath)
        {
            if (String.IsNullOrEmpty(DevicePath)) return null;

            // 打开设备文件
            IntPtr hHubDevice = Kernel32.CreateFile(
                "\\\\.\\" + DevicePath,
                NativeFileAccess.GENERIC_WRITE,
                NativeFileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeFileMode.OPEN_EXISTING,
                IntPtr.Zero,
                IntPtr.Zero);
            if (hHubDevice == Kernel32.INVALID_HANDLE_VALUE) return null;

            // 查询节点信息
            Int32 nBytesReturned;
            USB_NODE_INFORMATION Buffer = new USB_NODE_INFORMATION();
            Boolean Status = DeviceIoControl(hHubDevice,
                IOCTL_USB_GET_NODE_INFORMATION,
                ref Buffer,
                Marshal.SizeOf(Buffer),
                ref Buffer,
                Marshal.SizeOf(Buffer),
                out nBytesReturned,
                IntPtr.Zero);

            // 关闭设备文件
            Kernel32.CloseHandle(hHubDevice);
            if (!Status) return null;

            UsbNodeInformation Node = new UsbNodeInformation();
            Node.NodeType = Buffer.NodeType;    // 节点类型
            //Node.PNPDeviceID = DevicePath.Substring(0, DevicePath.LastIndexOf('#')).Replace('#', '\\'); // 设备ID
            Node.DevicePath = DevicePath;       // 设备路径
            Node.Name = GetUsbHubName(DevicePath);  // 设备名称
            if (Buffer.NodeType == USB_HUB_NODE.UsbHub)
            {
                Node.NumberOfPorts = Buffer.u.HubInformation.HubDescriptor.bNumberOfPorts;         // 端口数
                Node.HubIsBusPowered = Convert.ToBoolean(Buffer.u.HubInformation.HubIsBusPowered);  // 供电方式
                Node.HubCharacteristics = Buffer.u.HubInformation.HubDescriptor.wHubCharacteristics;
                Node.PowerOnToPowerGood = Buffer.u.HubInformation.HubDescriptor.bPowerOnToPowerGood;
                Node.HubControlCurrent = Buffer.u.HubInformation.HubDescriptor.bHubControlCurrent;
            }
            else
            {
                Node.NumberOfInterfaces = Buffer.u.MiParentInformation.NumberOfInterfaces;  // 接口数
            }

            return new UsbNodeInformation[1] { Node };
        }
        #endregion

        #region NODECONNECTION
        /// <summary>
        /// 获取USB节点连接信息
        /// </summary>
        /// <param name="DevicePath">设备路径</param>
        /// <param name="NumberOfPorts">端口总数</param>
        /// <returns>USB节点信息连接信息集合</returns>
        public static UsbNodeConnectionInformation[] GetUsbNodeConnectionInformation(String DevicePath, Int32 NumberOfPorts)
        {
            if (String.IsNullOrEmpty(DevicePath)) return null;

            // 打开设备文件
            IntPtr hHubDevice = Kernel32.CreateFile(
                "\\\\.\\" + DevicePath,
                NativeFileAccess.GENERIC_WRITE,
                NativeFileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeFileMode.OPEN_EXISTING,
                IntPtr.Zero,
                IntPtr.Zero);
            if (hHubDevice == Kernel32.INVALID_HANDLE_VALUE) return null;

            List<UsbNodeConnectionInformation> NodeCollection = new List<UsbNodeConnectionInformation>();

            // 枚举端口
            USB_NODE_CONNECTION_INFORMATION_EX Buffer = new USB_NODE_CONNECTION_INFORMATION_EX();
            for (Int32 ConnectionIndex = 1; ConnectionIndex <= NumberOfPorts; ConnectionIndex++)
            {
                // 查询节点信息
                Int32 nBytesReturned;
                Buffer.ConnectionIndex = ConnectionIndex;
                Boolean Status = DeviceIoControl(hHubDevice,
                    IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX,
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    out nBytesReturned,
                    IntPtr.Zero);
                if (Status)
                {
                    // 确定语言ID
                    UInt16 LanguageID = SelectLanguageID(hHubDevice, ConnectionIndex);

                    // 提取信息
                    UsbNodeConnectionInformation Node = new UsbNodeConnectionInformation();

                    Node.DevicePath = DevicePath;
                    Node.ConnectionIndex = Buffer.ConnectionIndex;
                    Node.ConnectionStatus = Buffer.ConnectionStatus;
                    if (Buffer.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                    {
                        Node.CurrentConfigurationValue = Buffer.CurrentConfigurationValue;
                        Node.Speed = Buffer.Speed;
                        Node.DeviceIsHub = Convert.ToBoolean(Buffer.DeviceIsHub);
                        Node.DeviceAddress = Buffer.DeviceAddress;
                        Node.NumberOfOpenPipes = Buffer.NumberOfOpenPipes;

                        // 设备描述符
                        Node.DeviceDescriptor.bDescriptorType = Buffer.DeviceDescriptor.bDescriptorType;
                        Node.DeviceDescriptor.bDeviceClass = Buffer.DeviceDescriptor.bDeviceClass;
                        Node.DeviceDescriptor.bDeviceSubClass = Buffer.DeviceDescriptor.bDeviceSubClass;
                        Node.DeviceDescriptor.bDeviceProtocol = Buffer.DeviceDescriptor.bDeviceProtocol;

                        Node.DeviceDescriptor.UsbVersion = BcdVersionToString(Buffer.DeviceDescriptor.bcdUSB); // USB版本号
                        Node.DeviceDescriptor.DeviceVersion = BcdVersionToString(Buffer.DeviceDescriptor.bcdDevice);    // 设备版本号

                        Node.DeviceDescriptor.idVendor = Buffer.DeviceDescriptor.idVendor;      // 厂商标识
                        Node.DeviceDescriptor.idProduct = Buffer.DeviceDescriptor.idProduct;    // 产品标识

                        if (LanguageID != 0)
                        {
                            if (Buffer.DeviceDescriptor.iSerialNumber != 0)
                            {   // 序列号
                                Node.DeviceDescriptor.SerialNumber = GetStringDescriptor(hHubDevice,
                                    Buffer.ConnectionIndex,
                                    Buffer.DeviceDescriptor.iSerialNumber,
                                    LanguageID);
                            }

                            if (Buffer.DeviceDescriptor.iManufacturer != 0)
                            {   // 制造商名称
                                Node.DeviceDescriptor.Manufacturer = GetStringDescriptor(hHubDevice,
                                    Buffer.ConnectionIndex,
                                    Buffer.DeviceDescriptor.iManufacturer,
                                    LanguageID);
                            }

                            if (Buffer.DeviceDescriptor.iProduct != 0)
                            {   // 产品名称
                                Node.DeviceDescriptor.Product = GetStringDescriptor(hHubDevice,
                                    Buffer.ConnectionIndex,
                                    Buffer.DeviceDescriptor.iProduct,
                                    LanguageID);
                            }
                        }

                        Node.DeviceDescriptor.bMaxPacketSize0 = Buffer.DeviceDescriptor.bMaxPacketSize0;
                        Node.DeviceDescriptor.bNumConfigurations = Buffer.DeviceDescriptor.bNumConfigurations;

                        // 管道信息
                        Node.PipeList = new List<UsbPipeInfo>();
                        for (Int32 PipeIndex = 0; PipeIndex < Buffer.NumberOfOpenPipes; PipeIndex++)
                        {
                            UsbPipeInfo PipeInfo;

                            PipeInfo.ScheduleOffset = Buffer.PipeList[PipeIndex].ScheduleOffset;
                            PipeInfo.bDescriptorType = Buffer.PipeList[PipeIndex].EndpointDescriptor.bDescriptorType;
                            PipeInfo.bEndpointAddress = Buffer.PipeList[PipeIndex].EndpointDescriptor.bEndpointAddress;
                            PipeInfo.bmAttributes = Buffer.PipeList[PipeIndex].EndpointDescriptor.bmAttributes;
                            PipeInfo.wMaxPacketSize = Buffer.PipeList[PipeIndex].EndpointDescriptor.wMaxPacketSize;
                            PipeInfo.bInterval = Buffer.PipeList[PipeIndex].EndpointDescriptor.bInterval;

                            Node.PipeList.Add(PipeInfo);
                        }
                    }

                    NodeCollection.Add(Node);
                }
            }

            // 关闭设备文件
            Kernel32.CloseHandle(hHubDevice);

            // 返回结果
            if (NodeCollection.Count == 0)
                return null;
            else
                return NodeCollection.ToArray();
        }

        /// <summary>
        /// 获取字符串描述符
        /// </summary>
        /// <param name="hHubDevice">USB Hub设备句柄</param>
        /// <param name="ConnectionIndex">连接索引号</param>
        /// <param name="DescriptorIndex">描述符索引号</param>
        /// <param name="LanguageID">语言ID</param>
        /// <returns>字符串描述符</returns>
        public static String GetStringDescriptor(IntPtr hHubDevice, Int32 ConnectionIndex, Byte DescriptorIndex, UInt16 LanguageID)
        {
            USB_DESCRIPTOR_REQUEST Buffer = new USB_DESCRIPTOR_REQUEST();
            Buffer.ConnectionIndex = ConnectionIndex;
            Buffer.SetupPacket.wValue = (UInt16)((USB_STRING_DESCRIPTOR_TYPE << 8) | DescriptorIndex);
            Buffer.SetupPacket.wIndex = LanguageID;
            Buffer.SetupPacket.wLength = MAXIMUM_USB_STRING_LENGTH;
            Int32 nBytesReturned;
            Boolean Status = DeviceIoControl(hHubDevice,
                    IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    out nBytesReturned,
                    IntPtr.Zero);
            if (Status)
                return Buffer.Data.bString;
            else
                return null;
        }

        /// <summary>
        /// 选择语言ID
        /// </summary>
        /// <param name="hHubDevice">USB Hub设备句柄</param>
        /// <param name="ConnectionIndex">连接索引号</param>
        /// <returns></returns>
        public static UInt16 SelectLanguageID(IntPtr hHubDevice, Int32 ConnectionIndex)
        {
            // 获取支持的语言列表
            String SupportedLanguagesString = GetStringDescriptor(hHubDevice, ConnectionIndex, 0, 0);
            if (String.IsNullOrEmpty(SupportedLanguagesString)) return 0;

            UInt16 UserDefaultUILanguage = Splash.Environment.UserDefaultUILanguage;
            if (SupportedLanguagesString.IndexOf(Convert.ToChar(UserDefaultUILanguage)) != -1)
            {   // 用户缺省界面语言
                return UserDefaultUILanguage;
            }
            else if (SupportedLanguagesString.IndexOf(Convert.ToChar(0x0409)) != -1)
            {   // 美国英语 0x0409
                return 0x0409;
            }
            else
            {   // 第一个可选择的LANGID
                return Convert.ToUInt16(SupportedLanguagesString[0]);
            }
        }
        #endregion

        #region EXTERNALHUB
        /// <summary>
        /// 获取外接Hub设备路径
        /// </summary>
        /// <param name="ParentDevicePath">上层Hub设备路径</param>
        /// <param name="ConnectionIndex">连接索引号</param>
        /// <returns>外接Hub设备路径</returns>
        public static String GetExternalHubPath(String ParentDevicePath, Int32 ConnectionIndex)
        {
            if (String.IsNullOrEmpty(ParentDevicePath)) return null;

            // 打开设备文件
            IntPtr hParentHubDevice = Kernel32.CreateFile(
                "\\\\.\\" + ParentDevicePath,
                NativeFileAccess.GENERIC_WRITE,
                NativeFileShare.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeFileMode.OPEN_EXISTING,
                IntPtr.Zero,
                IntPtr.Zero);
            if (hParentHubDevice == Kernel32.INVALID_HANDLE_VALUE) return null;

            USB_NODE_CONNECTION_DRIVERKEY_NAME Buffer = new USB_NODE_CONNECTION_DRIVERKEY_NAME();
            Buffer.ConnectionIndex = ConnectionIndex;
            Int32 nBytesReturned;
            Boolean Status = DeviceIoControl(hParentHubDevice,
                    IOCTL_USB_GET_NODE_CONNECTION_NAME,
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    out nBytesReturned,
                    IntPtr.Zero);

            // 关闭设备文件
            Kernel32.CloseHandle(hParentHubDevice);

            if (Status)
                return Buffer.DriverKeyName;
            else
                return null;
        }

        /// <summary>
        /// 获取外接Hub设备路径
        /// </summary>
        /// <param name="hParentHubDevice">上层Hub设备句柄</param>
        /// <param name="ConnectionIndex">连接索引号</param>
        /// <returns>外接Hub设备路径</returns>
        public static String GetExternalHubPath(IntPtr hParentHubDevice, Int32 ConnectionIndex)
        {
            if (hParentHubDevice == IntPtr.Zero || ConnectionIndex <= 0) return null;

            USB_NODE_CONNECTION_DRIVERKEY_NAME Buffer = new USB_NODE_CONNECTION_DRIVERKEY_NAME();
            Buffer.ConnectionIndex = ConnectionIndex;
            Int32 nBytesReturned;
            Boolean Status = DeviceIoControl(hParentHubDevice,
                    IOCTL_USB_GET_NODE_CONNECTION_NAME,
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    ref Buffer,
                    Marshal.SizeOf(Buffer),
                    out nBytesReturned,
                    IntPtr.Zero);

            if (Status)
                return Buffer.DriverKeyName;
            else
                return null;
        }
        #endregion

        #region BCDVERSION
        /// <summary>
        /// 版本BCD编码转字符串
        /// </summary>
        /// <param name="bcd">版本BCD编码</param>
        /// <returns>版本字符串</returns>
        private static String BcdVersionToString(UInt16 bcd)
        {
            StringBuilder sb = new StringBuilder(5);

            // 主版本号
            Int32 BIT4 = (bcd >> 12) & 0x0F;
            if (BIT4 != 0) sb.Append(BIT4.ToString());

            BIT4 = (bcd >> 8) & 0x0F;
            sb.Append(BIT4.ToString());

            sb.Append(".");

            // 子版本号
            BIT4 = (bcd >> 4) & 0x0F;
            sb.Append(BIT4.ToString());

            BIT4 = bcd & 0x0F;
            if (BIT4 != 0) sb.Append(BIT4.ToString());

            return sb.ToString();
        }
        #endregion
    }
}
