using System;
using System.IO;
using System.Windows;
using LibUsbDotNet.DeviceNotify;
using Splash.IO.PORTS;
using Splash.WPF;
using System.Management;
using System.Diagnostics;

namespace WPFUsbView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// USB插拔事件通知接口
        /// </summary>
        IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        public MainWindow()
        {
            InitializeComponent();
        }

        // 枚举设备信息并输出到XML文档
        private void buttonOpenXML_Click(object sender, RoutedEventArgs e)
        {
            String xmlFile = "UsbEnums.xml";
            try
            {   // 检测当前目录下是否可以创建文件
                using (StreamWriter sw = new StreamWriter(xmlFile))
                {
                    sw.Close();
                }
            }
            catch (Exception)
            {   // 当前目录无法创建文件，改到我的文档目录下
                xmlFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + xmlFile;
            }

            if (USB.EnumUsbToXML(xmlFile))
            {   // 判断文件是否存在
                if (System.IO.File.Exists(xmlFile))
                {   // 打开文件
                    Splash.Diagnostics.Extensions.ShellExecute(xmlFile);
                    return;
                }
            }

            MessageBox.Show("Failed!");
            return;
        }

        // 更新设备枚举信息
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            // 枚举USB设备信息
            treeView1.ItemsSource = TreeViewUsbItem.AllUsbDevices;

            // 展开所有分支
            treeView1.ExpandAll();

            // 设备连接数
            textBlockUsbDevice.Text = TreeViewUsbItem.ConnectedDevices.ToString();

            // 外部Hub连接数
            textBlockUsbHub.Text = TreeViewUsbItem.ConnectedHubs.ToString();
        }

        // 显示软件版本信息
        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            About AboutWindow = new About();
            AboutWindow.Owner = this;
            AboutWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;
            // 显示USB设备枚举信息
            buttonRefresh.PerformClick();
        }
        private void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                buttonRefresh_Click(null, null);
            }, null);
        }

        // 更新布局，调整各控件大小
        private void GridSplitter_LayoutUpdated(object sender, EventArgs e)
        {
            // 设置TreeView的宽度和高度
            treeView1.Width = gridDetail.ColumnDefinitions[0].ActualWidth;
            treeView1.Height = gridDetail.ActualHeight;
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewUsbItem Node = e.NewValue as TreeViewUsbItem;
            if (Node != null)
            {
                listView1.ItemsSource = ListViewUsbItem.UsbDetail(Node.Data);

                //Splash.IO.PORTS.UsbNodeConnectionInformation nodeData = (Splash.IO.PORTS.UsbNodeConnectionInformation)Node.Data;

                //Debug.WriteLine(nodeData.DevicePath);
            }
        }
    }
}
