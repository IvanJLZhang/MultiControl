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

//using MultiControl.Common;
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
                    lbl_deviceInfo.Text += "Serial Number: " + usbDevice.Info.SerialString + "\r\n";
                    string[] locationPaths = (string[])device.DeviceProperties["LocationPaths"];

                    P_ID p_id = P_ID.NULL;
                    V_ID v_id = V_ID.NULL;
                    Enum.TryParse<P_ID>(device.Pid.ToString(), out p_id);
                    Enum.TryParse<V_ID>(device.Vid.ToString(), out v_id);
                    DeviceManufactory man = new DeviceManufactory();
                    man.p_id = p_id;
                    man.v_id = v_id;
                    man.company_name = device.FullName;
                    lbl_deviceInfo.Text += "USB Port: " + filterUsbPort(locationPaths[0], man) + "\r\n";
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
                    lbl_deviceInfo.Text += "Serial Number: " + e.Device.SerialNumber + "\r\n" + "USB Port: " + locationInfo + "\r\n\r\n";
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
                        P_ID p_id = P_ID.NULL;
                        V_ID v_id = V_ID.NULL;
                        Enum.TryParse<P_ID>(device.Pid.ToString(), out p_id);
                        Enum.TryParse<V_ID>(device.Vid.ToString(), out v_id);

                        DeviceManufactory man = new DeviceManufactory();
                        man.p_id = p_id;
                        man.v_id = v_id;
                        man.company_name = device.FullName;
                        return filterUsbPort(locationPaths[0], man);
                    }
                }
            }
            UsbDevice.Exit();
            return returnStr;
        }

        string filterUsbPort(string locationPath, DeviceManufactory manufactory)
        {
            string usb_port = String.Empty;
            string[] arr = locationPath.Split('#');
            foreach (var node in arr)
            {
                if (node.Contains("USB("))
                {
                    int port = -1;
                    Int32.TryParse(node.Substring(4, 1), out port);
                    if (port > -1)
                    {
                        usb_port += "#" + port.ToString("D4");
                    }
                }
            }
            if (manufactory.p_id == P_ID.SAMSUNG && manufactory.v_id == V_ID.SAMSUNG)
            {
                usb_port = usb_port.Substring(0, usb_port.Length - 5);// 去掉最后一个USB位置
            }
            return usb_port;
        }
    }
    public enum P_ID
    {
        NULL = 0, SAMSUNG = 26720
    }
    public enum V_ID
    {
        NULL = 0, SAMSUNG = 1256
    }
    public class DeviceManufactory
    {
        public P_ID p_id;
        public V_ID v_id;
        public string company_name = String.Empty;
    }

}
