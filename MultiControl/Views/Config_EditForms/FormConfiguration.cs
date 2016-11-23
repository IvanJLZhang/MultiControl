using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiControl
{
    public partial class FormConfiguration : Form
    {
        public string mCfgFolder = string.Empty;
        private IniFile mIni = new IniFile(Application.StartupPath + "\\sysinfo.cfg");

        public FormConfiguration()
        {
            InitializeComponent();

            LoadConfig();
        }

        private void LoadConfig()
        {
            string folder = mIni.IniReadValue("config", "Folder");
            this.textBox1.Text = folder;
            mCfgFolder = folder;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select config file folder";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
                mCfgFolder = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mIni.IniWriteValue("config", "Folder", mCfgFolder);
            /*
            //COPY FOLDER CFG TO CURRENT FOLDER
            string src = mCfgFolder + "\\audio_loopback.cfg";
            string dst = Application.StartupPath + "\\audio_loopback.cfg";
            System.IO.File.Copy(src, dst, true);
            src = mCfgFolder + "\\monipower.cfg";
            dst = Application.StartupPath + "\\monipower.cfg";
            System.IO.File.Copy(src, dst, true);
            src = mCfgFolder + "\\sysinfo.cfg";
            dst = Application.StartupPath + "\\sysinfo.cfg";
            System.IO.File.Copy(src, dst, true);
            src = mCfgFolder + "\\wifi.cfg";
            dst = Application.StartupPath + "\\wifi.cfg";
            System.IO.File.Copy(src, dst, true);
            src = mCfgFolder + "\\pqaa.cfg";
            dst = Application.StartupPath + "\\pqaa.cfg";
            System.IO.File.Copy(src, dst, true);
             * */
            Close();
        }
    }
}
