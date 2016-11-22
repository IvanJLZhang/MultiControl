using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet.DeviceNotify;
//using MultiControl.Functions;

namespace CodeTest
{
    public partial class Form1 : Form
    {
        IDeviceNotifier deviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        public Form1()
        {
            InitializeComponent();
            //deviceNotifier.OnDeviceNotify += DeviceNotifier_OnDeviceNotify;
        }
        delegate void UpdateUIStatusDelegate();
        private void DeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            //UsbPortTree.RefreshUsbPort();
            //if (this.InvokeRequired)
            //{
            //this.BeginInvoke(new UpdateUIStatusDelegate(() =>
            //{
            //    label1.Text = "";
            //    UsbPortTree.RefreshUsbPort();
            //    foreach (var device in UsbPortTree.ConnectedAndroidDevices)
            //    {
            //        label1.Text += $"{device.Port_Path}\n{device.Port_Index}\n{device.Product}\n{device.SerialNumber}\n\n";
            //    }
            //}), null);
            //}
            //else
            //{
            //    //label1.Text = "";
            //    //UsbPortTree.RefreshUsbPort();
            //    //foreach (var device in UsbPortTree.ConnectedAndroidDevices)
            //    //{
            //    //    label1.Text += $"{device.Port_Path}\n{device.Port_Index}\n{device.Product}\n{device.SerialNumber}\n\n";
            //    //}
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //label1.Text = "";
            //UsbPortTree.RefreshUsbPort();
            //foreach (var device in UsbPortTree.ConnectedAndroidDevices)
            //{
            //    label1.Text += $"{device.Port_Path}\n{device.Port_Index}\n{device.Product}\n{device.SerialNumber}\n\n";
            //}
        }
    }
}
