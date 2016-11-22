/* ----------------------------------------------------------
文件名称：ListViewUsbItem.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年11月08日
			为USB设备枚举信息生成ListView数据源
------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using Splash.IO.PORTS;

namespace WPFUsbView
{
    /// <summary>
    /// TreeView节点对象
    /// </summary>
    internal class ListViewUsbItem
    {
        /// <summary>
        /// USB属性名
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// USB属性值
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Name">USB属性名</param>
        /// <param name="Value">USB属性值</param>
        public ListViewUsbItem(String Name, String Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        /// <summary>
        /// 生成USB属性列表
        /// </summary>
        /// <param name="Data">用于生成列表的USB数据</param>
        /// <returns>属性列表</returns>
        public static List<ListViewUsbItem> UsbDetail(Object Data)
        {
            if (Data is String)
            {   // 机器名
                String Info = Data as String;
                if (!String.IsNullOrEmpty(Info))
                {
                    String[] Content = Info.Split(new Char[] { ':' });
                    if (Content.Length == 2)
                    {
                        return new List<ListViewUsbItem>(1) { new ListViewUsbItem(Content[0], Content[1]) };
                    }
                }
            }
            else if (Data is HostControllerInfo)
            {   // 主控制器信息
                HostControllerInfo Info = (HostControllerInfo)Data;
                return new List<ListViewUsbItem>(3)
                {
                    new ListViewUsbItem("Name", Info.Name), 
                    new ListViewUsbItem("PNPDeviceID", Info.PNPDeviceID),                                       
                    new ListViewUsbItem("HcdDriverKeyName", Info.HcdDriverKeyName)
                };
            }
            else if (Data is UsbNodeInformation)
            {   // USB节点信息
                UsbNodeInformation Info = (UsbNodeInformation)Data;

                List<ListViewUsbItem> Items = new List<ListViewUsbItem>();
                Add(ref Items, Info);
                return Items;                
            }
            else if (Data is UsbNodeConnectionInformation)
            {   // USB节点连接信息
                UsbNodeConnectionInformation Info = (UsbNodeConnectionInformation)Data;
                if(Info.ConnectionStatus != USB_CONNECTION_STATUS.DeviceConnected)
                    return null;

                List<ListViewUsbItem> Items = new List<ListViewUsbItem>();
                Add(ref Items, Info);
                return Items;        
            }
            else if (Data is ExternalHubInfo)
            {   // 外部Hub信息
                ExternalHubInfo Info = (ExternalHubInfo)Data;

                List<ListViewUsbItem> Items = new List<ListViewUsbItem>();

                // 加入USB节点信息
                Items.Add(new ListViewUsbItem("Node Information:", null));
                Add(ref Items, Info.NodeInfo);

                // 加入USB节点连接信息
                Items.Add(new ListViewUsbItem(null, null));
                Items.Add(new ListViewUsbItem("Node Connection Information:", null));
                Add(ref Items, Info.NodeConnectionInfo);

                return Items;
            }

            return null;
        }

        /// <summary>
        /// 增加USB节点信息
        /// </summary>
        /// <param name="Items">要增加的列表</param>
        /// <param name="Info">要增加的信息</param>
        private static void Add(ref List<ListViewUsbItem> Items, UsbNodeInformation Info)
        {
            if (Info.NodeType == USB_HUB_NODE.UsbHub)
            {
                Items.Add(new ListViewUsbItem("Name", Info.Name));
                Items.Add(new ListViewUsbItem("PNPDeviceID", Info.PNPDeviceID));
                Items.Add(new ListViewUsbItem("DevicePath", Info.DevicePath));
                Items.Add(new ListViewUsbItem("NodeType", Info.NodeType.ToString()));
                Items.Add(new ListViewUsbItem("HubIsBusPowered", Info.HubIsBusPowered.ToString()));
                Items.Add(new ListViewUsbItem("NumberOfPorts", Info.NumberOfPorts.ToString()));
                Items.Add(new ListViewUsbItem("HubCharacteristics", "0x" + Info.HubCharacteristics.ToString("X4")));
                Items.Add(new ListViewUsbItem("PowerOnToPowerGood", (Info.PowerOnToPowerGood * 2).ToString() + "ms"));
                Items.Add(new ListViewUsbItem("HubControlCurrent", Info.HubControlCurrent.ToString()));
            }
            else
            {
                Items.Add(new ListViewUsbItem("Name", Info.Name));
                Items.Add(new ListViewUsbItem("PNPDeviceID", Info.PNPDeviceID));
                Items.Add(new ListViewUsbItem("DevicePath", Info.DevicePath));
                Items.Add(new ListViewUsbItem("NodeType", Info.NodeType.ToString()));
                Items.Add(new ListViewUsbItem("NumberOfInterfaces", Info.NumberOfInterfaces.ToString()));
            }
        }

        /// <summary>
        /// 增加USB节点连接信息
        /// </summary>
        /// <param name="Items">要增加的列表</param>
        /// <param name="Info">要增加的信息</param>
        private static void Add(ref List<ListViewUsbItem> Items, UsbNodeConnectionInformation Info)
        {
            Items.Add(new ListViewUsbItem("DevicePath", Info.DevicePath));
            Items.Add(new ListViewUsbItem("ConnectionIndex", Info.ConnectionIndex.ToString()));
            Items.Add(new ListViewUsbItem("CurrentConfigurationValue", "0x" + Info.CurrentConfigurationValue.ToString("X2")));
            Items.Add(new ListViewUsbItem("Speed", ((USB_DEVICE_SPEED)Info.Speed).ToString()));
            Items.Add(new ListViewUsbItem("DeviceIsHub", Info.DeviceIsHub.ToString()));
            Items.Add(new ListViewUsbItem("DeviceAddress", Info.DeviceAddress.ToString()));
            Items.Add(new ListViewUsbItem("NumberOfOpenPipes", Info.NumberOfOpenPipes.ToString()));

            // 设备描述符
            Items.Add(new ListViewUsbItem(null, null));
            Items.Add(new ListViewUsbItem("Device Descriptor:", null));
            Items.Add(new ListViewUsbItem("DescriptorType", "0x" + Info.DeviceDescriptor.bDescriptorType.ToString("X2")));
            Items.Add(new ListViewUsbItem("UsbVersion", Info.DeviceDescriptor.UsbVersion));
            Items.Add(new ListViewUsbItem("DeviceClass", "0x" + Info.DeviceDescriptor.bDeviceClass.ToString("X2")));
            Items.Add(new ListViewUsbItem("DeviceSubClass", "0x" + Info.DeviceDescriptor.bDeviceSubClass.ToString("X2")));
            Items.Add(new ListViewUsbItem("DeviceProtocol", "0x" + Info.DeviceDescriptor.bDeviceProtocol.ToString("X2")));
            Items.Add(new ListViewUsbItem("MaxPacketSize0", Info.DeviceDescriptor.bMaxPacketSize0.ToString()));
            Items.Add(new ListViewUsbItem("idVendor", "0x" + Info.DeviceDescriptor.idVendor.ToString("X4")));
            Items.Add(new ListViewUsbItem("idProduct", "0x" + Info.DeviceDescriptor.idProduct.ToString("X4")));
            Items.Add(new ListViewUsbItem("DeviceVersion", Info.DeviceDescriptor.DeviceVersion));
            Items.Add(new ListViewUsbItem("Manufacturer", Info.DeviceDescriptor.Manufacturer));
            Items.Add(new ListViewUsbItem("Product", Info.DeviceDescriptor.Product));
            Items.Add(new ListViewUsbItem("SerialNumber", Info.DeviceDescriptor.SerialNumber));
            Items.Add(new ListViewUsbItem("NumConfigurations", Info.DeviceDescriptor.bNumConfigurations.ToString()));

            // 管道信息
            foreach (UsbPipeInfo Pipe in Info.PipeList)
            {
                Items.Add(new ListViewUsbItem(null, null));
                Items.Add(new ListViewUsbItem("Endpoint Descriptor:", null));
                Items.Add(new ListViewUsbItem("ScheduleOffset", Pipe.ScheduleOffset.ToString()));
                Items.Add(new ListViewUsbItem("DescriptorType", "0x" + Pipe.bDescriptorType.ToString("X2")));
                Items.Add(new ListViewUsbItem("EndpointAddress", "0x" + Pipe.bEndpointAddress.ToString("X2")));
                Items.Add(new ListViewUsbItem("bmAttributes", "0x" + Pipe.bmAttributes.ToString("X2")));
                Items.Add(new ListViewUsbItem("MaxPacketSize", Pipe.wMaxPacketSize.ToString()));
                Items.Add(new ListViewUsbItem("Interval", "0x" + Pipe.bInterval.ToString("X2")));
            }
        }
    }
}
