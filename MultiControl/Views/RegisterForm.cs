using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;
using System.IO;

namespace MultiControl
{
    public partial class RegisterForm : Form
    {

        public static bool m_bLicensed = false;
        public RegisterForm()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string UserId = this.textBoxId.Text;
            string LicenseKey = this.textBoxKey.Text;
            if (Validate(UserId, LicenseKey) == 0)
            {
                //save key file
                byte[] data = Encoding.Unicode.GetBytes(LicenseKey);
                File.WriteAllBytes("license.dat", data);
                MainWindow.m_bLicensed = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please Input Correct Register Code!");
            }
        }
        private void RegisterForm_Load(object sender, EventArgs e)
        {
            ManagementClass hddObject = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection hddInfo = hddObject.GetInstances();
            string user_id = String.Empty;
            foreach (ManagementObject mo in hddInfo)
            {
                user_id = mo.Properties["SerialNumber"].Value.ToString().Trim();
                break;
            }
            this.textBoxId.Text = user_id.Trim();
            if (File.Exists("license.dat"))
            {
                string license_key = File.ReadAllText("license.dat", Encoding.Unicode).Trim();
                this.textBoxKey.Text = license_key;
                if (!string.IsNullOrEmpty(license_key))
                {
                    if (Validate(user_id, license_key) == 0)
                    {
                        MainWindow.m_bLicensed = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        [DllImport("KeyGenerate.dll", EntryPoint = "fnGenerateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Generate(string user, StringBuilder sbr);

        [DllImport("KeyGenerate.dll", EntryPoint = "fnValidateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Validate(string user, string key);

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
