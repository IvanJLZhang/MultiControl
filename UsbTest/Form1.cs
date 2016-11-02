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

namespace UsbTest
{
    public partial class Form1 : Form
    {
        #region properties
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        #endregion
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //usb.EventArrived += Usb_EventArrived;
            ////usb.EventDeletion += Usb_EventDeletion;
            //usb.StartUsbInsertWatcher(TimeSpan.FromSeconds(1));
        }
        override protected void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    var value = m.WParam.ToInt32();
                    switch (m.WParam.ToInt32())
                    {
                        // USB插上  
                        case DBT_DEVICEARRIVAL:
                            Debug.WriteLine("Device arrival");
                            break;
                        // USB移除  
                        case DBT_DEVICEREMOVECOMPLETE:
                            Debug.WriteLine("Device removed");

                            //IsCopy = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            base.WndProc(ref m);
        }
        private void Usb_EventDeletion(object sender, System.Management.EventArrivedEventArgs e)
        {
            Debug.WriteLine(e.NewEvent.ClassPath);
        }
        int cnt = 0;
        private void Usb_EventArrived(object sender, System.Management.EventArrivedEventArgs e)
        {
            Debug.WriteLine(e.NewEvent.ClassPath + "" + (++cnt));
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
