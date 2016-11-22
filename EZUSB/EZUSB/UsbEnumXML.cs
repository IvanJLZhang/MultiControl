/* ----------------------------------------------------------
文件名称：UsbEnumXML.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年10月28日
			将USB设备枚举信息导出为XML文档
------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Splash.IO.PORTS
{
    /// <summary>
    /// 将USB设备信息写入XML文件
    /// </summary>
    public partial class USB
    {
        /// <summary>
        /// 将USB设备枚举信息导出为XML文档
        /// </summary>
        /// <param name="xmlFileName">保存的XML文件名</param>
        /// <returns>
        ///     true：成功
        ///     false：失败
        /// </returns>
        public static Boolean EnumUsbToXML(String xmlFileName)
        {   // 创建根节点
            XElement RootNode = new XElement("Computer",
                new XAttribute("MachineName", System.Environment.MachineName));

            // 深度遍历主控制器
            HostControllerInfo[] HostControllersCollection = USB.AllHostControllers;
            if (HostControllersCollection != null)
            {
                Int32 ControllerIndex = 1;
                foreach (HostControllerInfo item in HostControllersCollection)
                {   // 创建主控制器节点                    
                    XElement HostControllerNode = new XElement("HostController" + ControllerIndex,
                        new XAttribute("Name", item.Name),  // 设备名称
                        new XAttribute("PNPDeviceID", item.PNPDeviceID), // 设备ID
                        new XAttribute("HcdDriverKeyName", item.HcdDriverKeyName)    // 驱动键名
                        );
                    RootNode.Add(HostControllerNode);
                    ControllerIndex++;

                    // 创建根集线器节点
                    String RootHubPath = USB.GetUsbRootHubPath(item.PNPDeviceID);
                    AddHubNode(HostControllerNode, RootHubPath, "RootHub");
                }
            }

            // 创建XML文档
            XDocument xmlTree = new XDocument(RootNode);

            // 存储文件，序列化时对XML进行格式设置（缩进）
            xmlTree.Save(xmlFileName, SaveOptions.None);            
            return true;
        }
        
        /// <summary>
        /// 增加集线器节点
        /// </summary>
        /// <param name="ParentNode">父节点</param>
        /// <param name="HubPath">集线器路径</param>
        private static void AddHubNode(XElement ParentNode, String HubPath, String HubNodeName)
        {
            UsbNodeInformation[] NodeInfoCollection = USB.GetUsbNodeInformation(HubPath);
            if (NodeInfoCollection != null)
            {
                USB_HUB_NODE NodeType = NodeInfoCollection[0].NodeType;
                XElement HubNode = new XElement(HubNodeName,                
                    new XAttribute("Name", NodeInfoCollection[0].Name),
                    new XAttribute("PNPDeviceID", NodeInfoCollection[0].PNPDeviceID),
                    new XAttribute("Path", NodeInfoCollection[0].DevicePath),
                    new XAttribute("NodeType", NodeType)
                    );

                if (NodeType == USB_HUB_NODE.UsbHub)
                {
                    Int32 NumberOfPorts = NodeInfoCollection[0].NumberOfPorts;
                    HubNode.Add(new XAttribute("NumberOfPorts", NumberOfPorts),
                        new XAttribute("HubIsBusPowered", NodeInfoCollection[0].HubIsBusPowered),                        
                        new XAttribute("HubCharacteristics", "0x" + NodeInfoCollection[0].HubCharacteristics.ToString("X4")),
                        new XAttribute("PowerOnToPowerGood", NodeInfoCollection[0].PowerOnToPowerGood),
                        new XAttribute("HubControlCurrent", NodeInfoCollection[0].HubControlCurrent)
                        );

                    // 深度遍历端口
                    UsbNodeConnectionInformation[] NodeConnectionInfoCollection = USB.GetUsbNodeConnectionInformation(HubPath, NumberOfPorts);
                    if (NodeConnectionInfoCollection != null)
                    {
                        foreach (UsbNodeConnectionInformation NodeConnectionInfo in NodeConnectionInfoCollection)
                        {   // 增加端口节点
                            AddPortNode(HubNode, NodeConnectionInfo);
                        }
                    }
                }
                else
                {
                    HubNode.Add("NumberOfInterfaces", NodeInfoCollection[0].NumberOfInterfaces);
                }

                ParentNode.Add(HubNode);
            }
        }

        /// <summary>
        /// 增加端口节点
        /// </summary>
        /// <param name="HubNode">集线器节点</param>
        /// <param name="NodeConnectionInfo">USB设备节点连接信息</param>
        private static void AddPortNode(XElement HubNode, UsbNodeConnectionInformation NodeConnectionInfo)
        {
            String DevicePath = NodeConnectionInfo.DevicePath;
            Int32 ConnectionIndex = NodeConnectionInfo.ConnectionIndex;
            USB_CONNECTION_STATUS ConnectionStatus = NodeConnectionInfo.ConnectionStatus;    

            // 创建端口节点
            XElement PortNode = new XElement("Port" + ConnectionIndex,
                new XAttribute("DevicePath", DevicePath),
                new XAttribute("ConnectionIndex", ConnectionIndex),
                new XAttribute("ConnectionStatus", NodeConnectionInfo.ConnectionStatus)
                );

            if (ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
            {
                Boolean DeviceIsHub = NodeConnectionInfo.DeviceIsHub;
                PortNode.Add(new XAttribute("DeviceIsHub", DeviceIsHub),
                    new XAttribute("CurrentConfigurationValue", NodeConnectionInfo.CurrentConfigurationValue),
                    new XAttribute("Speed", NodeConnectionInfo.Speed),
                    new XAttribute("DeviceAddress", NodeConnectionInfo.DeviceAddress),
                    new XAttribute("NumberOfOpenPipes", NodeConnectionInfo.NumberOfOpenPipes)
                    );

                // 设备描述符信息
                AddDeviceDescriptorNode(PortNode, ref NodeConnectionInfo.DeviceDescriptor);

                // 管道信息
                AddPipeInfoNode(PortNode, ref NodeConnectionInfo.PipeList);

                // 外部集线器
                if (DeviceIsHub)
                {   // 获取外部Hub设备路径
                    String ExternalHubPath = GetExternalHubPath(DevicePath, ConnectionIndex);

                    // 增加外部集线器节点
                    AddHubNode(PortNode, ExternalHubPath, "ExternalHub");
                }
            }           

            HubNode.Add(PortNode);
        }

        /// <summary>
        /// 增加设备描述符节点
        /// </summary>
        /// <param name="PortNode"></param>
        /// <param name="DeviceDescriptor"></param>
        private static void AddDeviceDescriptorNode(XElement PortNode, ref UsbDeviceDescriptor DeviceDescriptor)
        {
            XElement DeviceDescriptorNode = new XElement("DeviceDescriptor",
                new XAttribute("bDescriptorType", "0x" + DeviceDescriptor.bDescriptorType.ToString("X2")),
                new XAttribute("UsbVersion", DeviceDescriptor.UsbVersion),
                new XAttribute("bDeviceClass", "0x" + DeviceDescriptor.bDeviceClass.ToString("X2")),
                new XAttribute("bDeviceSubClass", "0x" + DeviceDescriptor.bDeviceSubClass.ToString("X2")),
                new XAttribute("bDeviceProtocol", "0x" + DeviceDescriptor.bDeviceProtocol.ToString("X2")),
                new XAttribute("bMaxPacketSize0", DeviceDescriptor.bMaxPacketSize0),  
                new XAttribute("bNumConfigurations", DeviceDescriptor.bNumConfigurations)
                );

            if (DeviceDescriptor.idVendor != 0)
            {
                DeviceDescriptorNode.Add(new XAttribute("idVendor", "0x" + DeviceDescriptor.idVendor.ToString("X4")));                
            }

            if (DeviceDescriptor.idProduct != 0)
            {
                DeviceDescriptorNode.Add(new XAttribute("idProduct", "0x" + DeviceDescriptor.idProduct.ToString("X4")));
            }

            if (!String.IsNullOrEmpty(DeviceDescriptor.DeviceVersion))
            {
                DeviceDescriptorNode.Add(new XAttribute("DeviceVersion", DeviceDescriptor.DeviceVersion));
            }

            if (!String.IsNullOrEmpty(DeviceDescriptor.Manufacturer))
            {
                DeviceDescriptorNode.Add(new XAttribute("Manufacturer", DeviceDescriptor.Manufacturer));
            }

            if (!String.IsNullOrEmpty(DeviceDescriptor.Product))
            {
                DeviceDescriptorNode.Add(new XAttribute("Product", DeviceDescriptor.Product));
            }

            if (!String.IsNullOrEmpty(DeviceDescriptor.SerialNumber))
            {
                DeviceDescriptorNode.Add(new XAttribute("SerialNumber", DeviceDescriptor.SerialNumber));
            }

            PortNode.Add(DeviceDescriptorNode);
        }

        /// <summary>
        /// 增加管道信息节点
        /// </summary>
        /// <param name="PortNode">端口节点</param>
        /// <param name="PipeList">管道信息列表</param>
        private static void AddPipeInfoNode(XElement PortNode, ref List<UsbPipeInfo> PipeList)
        {
            if(PipeList != null)
            {
                XElement PipeListNode = new XElement("PipeList");
                Int32 PipeIndex = 1;
                foreach(UsbPipeInfo item in PipeList)
                {
                    XElement PipeInfoNode = new XElement("Pipe" + PipeIndex,
                        new XAttribute("ScheduleOffset", item.ScheduleOffset),
                        new XAttribute("bDescriptorType", "0x" + item.bDescriptorType.ToString("X2")),
                        new XAttribute("bEndpointAddress", "0x" + item.bEndpointAddress.ToString("X2")),
                        new XAttribute("bmAttributes", "0x" + item.bmAttributes.ToString("X2")),
                        new XAttribute("wMaxPacketSize", item.wMaxPacketSize),
                        new XAttribute("bInterval", item.bInterval)
                        );

                    PipeListNode.Add(PipeInfoNode);
                    PipeIndex++;
                }

                PortNode.Add(PipeListNode);
            } 
        }
    }
}
