using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using LibUsbDotNet.WinUsb;
using Microsoft.Win32;
namespace UsbTest
{
    public partial class Form1 : Form
    {
        IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        public Form1()
        {
            InitializeComponent();
            ShowConnectedDevices();
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;
        }

        void ShowConnectedDevices()
        {
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            for (int index = 0; index < allDevices.Count; index++)
            {
                var device = (WinUsbRegistry)allDevices[index];

                UsbDevice usbDevice;
                if (device.Open(out usbDevice))
                {
                    //lbl_deviceInfo.Text += device.DeviceID + "\r\n";
                    lbl_deviceInfo.Text += usbDevice.Info.SerialString + "\r\n";
                    string[] locationPaths = (string[])device.DeviceProperties["LocationPaths"];
                    lbl_deviceInfo.Text += filterUsbPort(locationPaths[0]) + "\r\n";
                    lbl_deviceInfo.Text += "\r\n";
                }
            }
        }

        private async void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (e.EventType == EventType.DeviceArrival && e.Device != null)
            {
                string locationInfo = String.Empty;
                int count = 0;
                while (String.IsNullOrEmpty(locationInfo) && count <= 10)
                {
                    await Task.Delay(500);
                    locationInfo = findArrivaledDevice(e.Device.SerialNumber);
                    count++;
                }
                if (locationInfo != string.Empty)
                    lbl_deviceInfo.Text += e.Device.SerialNumber + "\r\n" + locationInfo + "\r\n\r\n";
            }
        }

        string findArrivaledDevice(string serialNumber)
        {
            string returnStr = String.Empty;
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            Debug.WriteLine($"find devices {allDevices.Count}");
            for (int index = 0; index < allDevices.Count; index++)
            {
                var device = allDevices[index];
                UsbDevice usbDevice;
                bool result = device.Open(out usbDevice);
                if (result)
                {
                    if (serialNumber == usbDevice.Info.SerialString)
                    {
                        string[] locationPaths = (string[])device.DeviceProperties["LocationPaths"];

                        return filterUsbPort(locationPaths[0]);
                    }
                }
            }
            UsbDevice.Exit();
            return returnStr;
        }

        string filterUsbPort(string locationPath)
        {
            string usb_port = String.Empty;
            string[] arr = locationPath.Split('#');
            int count = 0;
            foreach (var node in arr)
            {
                if (node.Contains("USB("))
                {
                    int port = -1;
                    Int32.TryParse(node.Substring(4, 1), out port);
                    if (port > -1)
                    {
                        count++;
                        usb_port += "#" + port.ToString("D4");
                    }
                    if (count >= 2)
                        break;
                }
            }
            return usb_port;
        }
    }
}
