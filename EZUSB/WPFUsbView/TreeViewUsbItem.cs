/* ----------------------------------------------------------
文件名称：TreeViewUsbItem.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：    
    V1.0	2011年10月26日
			为USB设备枚举信息生成TreeView数据源
------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Splash.IO.PORTS;

namespace WPFUsbView
{
    /// <summary>
    /// TreeView节点对象
    /// </summary>
    internal class TreeViewUsbItem
    {
        /// <summary>
        /// 节点图标
        /// </summary>
        public ImageSource Icon { get; set; }
        
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
        public List<TreeViewUsbItem> Children { get; set; }        

        /// <summary>
        /// 计算机图标
        /// </summary>
        public static ImageSource ImageComputer
        {
            get
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Properties.Resources.notebook.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
        }     

        /// <summary>
        /// 主控制器图标
        /// </summary>
        public static ImageSource ImageHostController
        {
            get
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Properties.Resources.usb.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
        }

        /// <summary>
        /// Hub图标
        /// </summary>
        public static ImageSource ImageHub
        {
            get
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Properties.Resources.hub.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
        }

        /// <summary>
        /// USB设备图标
        /// </summary>
        public static ImageSource ImageDevice
        {
            get
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Properties.Resources.port.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
        }

        /// <summary>
        /// 连接的外部Hub数目
        /// </summary>
        public static Int32 ConnectedHubs = 0;

        /// <summary>
        /// 连接的USB设备数目
        /// </summary>
        public static Int32 ConnectedDevices = 0;

        /// <summary>
        /// 静态根节点
        /// </summary>
        public static List<TreeViewUsbItem> AllUsbDevices
        {
            get
            {
                // 初始化
                ConnectedHubs = 0;      // 连接的外部Hub数目
                ConnectedDevices = 0;   // 连接的USB设备数目

                // 创建根节点
                TreeViewUsbItem Root = new TreeViewUsbItem();
                Root.Icon = ImageComputer;
                Root.Name = "Computer";
                Root.Data = "Machine Name:" + System.Environment.MachineName;
                
                // 子节点列表
                // 深度遍历主控制器
                HostControllerInfo[] HostControllersCollection = USB.AllHostControllers;
                if (HostControllersCollection != null)
                {
                    List<TreeViewUsbItem> HCNodeCollection = new List<TreeViewUsbItem>(HostControllersCollection.Length);
                    foreach (HostControllerInfo item in HostControllersCollection)
                    {   // 创建主控制器节点
                        TreeViewUsbItem HCNode = new TreeViewUsbItem();

                        HCNode.Icon = ImageHostController;
                        HCNode.Name = item.Name;
                        HCNode.Data = item;

                        // 创建根集线器节点
                        String RootHubPath = USB.GetUsbRootHubPath(item.PNPDeviceID);
                        HCNode.Children = AddHubNode(RootHubPath, "RootHub");

                        HCNodeCollection.Add(HCNode);                       
                    }

                    Root.Children = HCNodeCollection;
                }

                return new List<TreeViewUsbItem>(1) { Root };
            }
        }

        /// <summary>
        /// Hub节点
        /// </summary>
        /// <param name="HubPath">Hub路径</param>
        /// <param name="HubNodeName">节点显示名称</param>
        /// <returns>Hub节点集合</returns>
        private static List<TreeViewUsbItem> AddHubNode(String HubPath, String HubNodeName)
        {
            UsbNodeInformation[] NodeInfoCollection = USB.GetUsbNodeInformation(HubPath);
            if (NodeInfoCollection != null)
            {
                TreeViewUsbItem HubNode = new TreeViewUsbItem();
                HubNode.Icon = ImageHub;
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

                return new List<TreeViewUsbItem>(1) { HubNode };
            }
            
            return null;
        }

        /// <summary>
        /// Port节点
        /// </summary>
        /// <param name="HubPath">Hub路径</param>
        /// <param name="NumberOfPorts">端口数</param>
        /// <returns>Port节点集合</returns>
        private static List<TreeViewUsbItem> AddPortNode(String HubPath, Int32 NumberOfPorts)
        {
            // 深度遍历端口
            UsbNodeConnectionInformation[] NodeConnectionInfoCollection = USB.GetUsbNodeConnectionInformation(HubPath, NumberOfPorts);
            if (NodeConnectionInfoCollection != null)
            {
                List<TreeViewUsbItem> PortNodeCollection = new List<TreeViewUsbItem>(NumberOfPorts);
                foreach (UsbNodeConnectionInformation NodeConnectionInfo in NodeConnectionInfoCollection)
                {   // 增加端口节点
                    TreeViewUsbItem PortNode = new TreeViewUsbItem();

                    PortNode.Icon = ImageDevice;
                    PortNode.Name = "[Port" + NodeConnectionInfo.ConnectionIndex + "]" + NodeConnectionInfo.ConnectionStatus;
                    PortNode.Data = NodeConnectionInfo;
                    PortNode.Children = null;
                    if (NodeConnectionInfo.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                    {
                        // 设备连接
                        ConnectedDevices++; // 连接的USB设备数目
                        if (!String.IsNullOrEmpty(NodeConnectionInfo.DeviceDescriptor.Product))
                        {   // 产品名称
                            PortNode.Name = String.Concat(PortNode.Name, ": ", NodeConnectionInfo.DeviceDescriptor.Product);
                        }

                        if (NodeConnectionInfo.DeviceIsHub)
                        {
                            // 获取外部Hub设备路径
                            String ExternalHubPath = USB.GetExternalHubPath(NodeConnectionInfo.DevicePath, NodeConnectionInfo.ConnectionIndex);
                            UsbNodeInformation[] NodeInfoCollection = USB.GetUsbNodeInformation(HubPath);
                            if (NodeInfoCollection != null)
                            {
                                PortNode.Icon = ImageHub;
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
