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
        private string mUserId = string.Empty;
        private string mLicenseKey = string.Empty;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private bool writeData(byte[] pData, string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pData, 0, pData.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pFileStream != null)
                {
                    pFileStream.Close();
                }
            }

            return true;
        }

        private string readData(string fileName)
        {
            string key = string.Empty;
            FileStream pFileStream = null;
            byte[] pData = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(pFileStream);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                pData = reader.ReadBytes((int)reader.BaseStream.Length);
                key = Encoding.Unicode.GetString(pData, 0, pData.Length);
                if (pFileStream != null)
                {
                    pFileStream.Close();
                }
                reader.Close();
                return key;
            }
            catch
            {
                return key;
            }
            
        }

        private void LoadTestingProgram()
        {
            this.Hide();
            Form1 mainFrm = new Form1();
            mainFrm.UpdateLicenseInformation(mLicenseKey, true);
            mainFrm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mUserId = this.textBoxId.Text;
            mLicenseKey = this.textBoxKey.Text;
            //StringBuilder sbKey = new StringBuilder(35);
            //sbKey.Append(mLicenseKey);
            if (Validate(mUserId, mLicenseKey) == 0)
            {
                //save key file
                byte[] data = Encoding.Unicode.GetBytes(mLicenseKey);
                if (writeData(data, "license.dat"))
                {
                    LoadTestingProgram();
                }
            }
            else
            {
                MessageBox.Show("Please Input Correct Register Code!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            ManagementClass hddObject = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection hddInfo = hddObject.GetInstances();
            foreach (ManagementObject mo in hddInfo)
            {
                mUserId = mo.Properties["SerialNumber"].Value.ToString().Trim();
                break;
            }
            this.textBoxId.Text = mUserId.Trim();//.Substring(0,8);
            //StringBuilder sbr = new StringBuilder(35);
            //int result = Generate(mUserId, sbr);
            //this.textBoxKey.Text = sbr.ToString();
            //Validate(mUserId, mLicenseKey);
            //mLicenseKey = "V5QTU-QBLOR-RB8IQ-AMC65-1ID90-K0NUB";//readData("license.dat");
            //WD-WXP1E51XKE48
            //mUserId = "billge";
            mLicenseKey = readData("license.dat");
            if (!string.IsNullOrEmpty(mLicenseKey))
            {
                //StringBuilder sbKey = new StringBuilder(35);
                //sbKey.Append(mLicenseKey);
                if (Validate(mUserId, mLicenseKey) == 0)
                {
                    LoadTestingProgram();
                }
            }
        }

        [DllImport("KeyGenerate.dll", EntryPoint = "fnGenerateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Generate(string user, StringBuilder sbr);

        [DllImport("KeyGenerate.dll", EntryPoint = "fnValidateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Validate(string user, string key);
    }
}
