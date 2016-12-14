
/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/11/18 11:20:31 
 * 文件名：UsbPortTree 
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
using LibUsbDotNet.Info;
using MultiControl.Common;
using Splash.IO.PORTS;

namespace MultiControl.Functions
{
    public class UsbPortTree
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 节点数据
        /// </summary>
        public Object Data { get; set; }

        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<UsbPortTree> Children { get; set; }

        /// <summary>
        /// 连接的外部Hub数目
        /// </summary>
        public static Int32 ConnectedHubs = 0;

        /// <summary>
        /// 连接的USB设备数目
        /// </summary>
        public static Int32 ConnectedDevicesCnt = 0;

        public static List<UsbPortTree> _allUsbDevices;
        /// <summary>
        /// 静态根节点
        /// </summary>
        public static List<UsbPortTree> AllUsbDevices
        {
            get
            {
                return _allUsbDevices;
            }
        }

        public static List<UsbDeviceInfoEx> _connectedAndroidDevices = new List<UsbDeviceInfoEx>();

        public static List<UsbDeviceInfoEx> ConnectedAndroidDevices
        {
            get
            {
                return _connectedAndroidDevices;
            }
        }
        public static void RefreshUsbPort()
        {
            // 初始化
            ConnectedHubs = 0;      // 连接的外部Hub数目
            ConnectedDevicesCnt = 0;   // 连接的USB设备数目
            ConnectedAndroidDevices.Clear();
            // 创建根节点
            UsbPortTree Root = new UsbPortTree();
            //Root.Icon = ImageComputer;
            Root.Name = "Computer";
            Root.Data = "Machine Name:" + System.Environment.MachineName;

            // 子节点列表
            // 深度遍历主控制器
            HostControllerInfo[] HostControllersCollection = USB.AllHostControllers;
            if (HostControllersCollection != null)
            {
                List<UsbPortTree> HCNodeCollection = new List<UsbPortTree>(HostControllersCollection.Length);
                foreach (HostControllerInfo item in HostControllersCollection)
                {   // 创建主控制器节点
                    UsbPortTree HCNode = new UsbPortTree();
                    //HCNode.Icon = ImageHostController;
                    HCNode.Name = item.Name;
                    HCNode.Data = item;

                    // 创建根集线器节点
                    String RootHubPath = USB.GetUsbRootHubPath(item.PNPDeviceID);
                    HCNode.Children = AddHubNode(RootHubPath, "RootHub");

                    HCNodeCollection.Add(HCNode);
                }

                Root.Children = HCNodeCollection;
            }
            _allUsbDevices = new List<UsbPortTree>(1) { Root };
        }

        /// <summary>
        /// Hub节点
        /// </summary>
        /// <param name="HubPath">Hub路径</param>
        /// <param name="HubNodeName">节点显示名称</param>
        /// <returns>Hub节点集合</returns>
        private static List<UsbPortTree> AddHubNode(String HubPath, String HubNodeName)
        {
            UsbNodeInformation[] NodeInfoCollection = USB.GetUsbNodeInformation(HubPath);
            if (NodeInfoCollection != null)
            {
                UsbPortTree HubNode = new UsbPortTree();
                //HubNode.Icon = ImageHub;
                if (String.IsNullOrEmpty(NodeInfoCollection[0].Name))
                {
                    HubNode.Name = HubNodeName;
                }
                else
                {
                    HubNode.Name = NodeInfoCollection[0].Name;
                }
                HubNode.Data = NodeInfoCollection[0];

                if (NodeInfoCollection[0].NodeType == USB_HUB_NODE.UsbHub)
                {
                    HubNode.Children = AddPortNode(HubPath, NodeInfoCollection[0].NumberOfPorts);
                }
                else
                {
                    HubNode.Children = null;
                }

                return new List<UsbPortTree>(1) { HubNode };
            }

            return null;
        }

        /// <summary>
        /// Port节点
        /// </summary>
        /// <param name="HubPath">Hub路径</param>
        /// <param name="NumberOfPorts">端口数</param>
        /// <returns>Port节点集合</returns>
        private static List<UsbPortTree> AddPortNode(String HubPath, Int32 NumberOfPorts)
        {
            // 深度遍历端口
            UsbNodeConnectionInformation[] NodeConnectionInfoCollection = USB.GetUsbNodeConnectionInformation(HubPath, NumberOfPorts);
            if (NodeConnectionInfoCollection != null)
            {
                List<UsbPortTree> PortNodeCollection = new List<UsbPortTree>(NumberOfPorts);
                foreach (UsbNodeConnectionInformation NodeConnectionInfo in NodeConnectionInfoCollection)
                {   // 增加端口节点
                    UsbPortTree PortNode = new UsbPortTree();

                    //PortNode.Icon = ImageDevice;
                    PortNode.Name = "[Port" + NodeConnectionInfo.ConnectionIndex + "]" + NodeConnectionInfo.ConnectionStatus;
                    PortNode.Data = NodeConnectionInfo;
                    PortNode.Children = null;
                    if (NodeConnectionInfo.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                    {
                        // 设备连接
                        ConnectedDevicesCnt++; // 连接的USB设备数目
                        if (!String.IsNullOrEmpty(NodeConnectionInfo.DeviceDescriptor.Product))
                        {   // 产品名称
                            PortNode.Name = String.Concat(PortNode.Name, ": ", NodeConnectionInfo.DeviceDescriptor.Product);
                            common.m_log.Add($"{PortNode.Name}");
                            common.m_log.Add($"P_ID:{NodeConnectionInfo.DeviceDescriptor.idProduct};V_ID:{NodeConnectionInfo.DeviceDescriptor.idVendor};Product:{NodeConnectionInfo.DeviceDescriptor.Product}");
                            if (NodeConnectionInfo.DeviceDescriptor.Product.ToUpper().Contains("ANDROID"))
                            {
                                UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                                device.Port_Path = NodeConnectionInfo.DevicePath;
                                device.Port_Index = NodeConnectionInfo.ConnectionIndex;
                                device.Product = NodeConnectionInfo.DeviceDescriptor.Product;
                                device.SerialNumber = NodeConnectionInfo.DeviceDescriptor.SerialNumber;
                                device.idProduct = NodeConnectionInfo.DeviceDescriptor.idProduct;
                                device.idVender = NodeConnectionInfo.DeviceDescriptor.idVendor;
                                if (!_connectedAndroidDevices.Contains(device, UsbDeviceInfoEx.Default))
                                {
                                    _connectedAndroidDevices.Add(device);
                                }
                            }
                        }

                        if (NodeConnectionInfo.DeviceIsHub)
                        {
                            // 获取外部Hub设备路径
                            String ExternalHubPath = USB.GetExternalHubPath(NodeConnectionInfo.DevicePath, NodeConnectionInfo.ConnectionIndex);
                            UsbNodeInformation[] NodeInfoCollection = USB.GetUsbNodeInformation(HubPath);
                            if (NodeInfoCollection != null)
                            {
                                PortNode.Data = new ExternalHubInfo { NodeInfo = NodeInfoCollection[0], NodeConnectionInfo = NodeConnectionInfo };
                                if (NodeInfoCollection[0].NodeType == USB_HUB_NODE.UsbHub)
                                {
                                    PortNode.Children = AddPortNode(ExternalHubPath, NodeInfoCollection[0].NumberOfPorts);
                                }

                                if (String.IsNullOrEmpty(NodeConnectionInfo.DeviceDescriptor.Product))
                                {
                                    if (!String.IsNullOrEmpty(NodeInfoCollection[0].Name))
                                    {   // 产品名称
                                        PortNode.Name = String.Concat(PortNode.Name, ": ", NodeInfoCollection[0].Name);
                                    }
                                }
                            }

                            ConnectedHubs++;    // 连接的外部Hub数目
                        }
                    }

                    PortNodeCollection.Add(PortNode);
                }

                return PortNodeCollection;
            }

            return null;
        }
    }
}
