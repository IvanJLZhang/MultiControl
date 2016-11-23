using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiControl
{
    public partial class Logger : Form
    {
        public string mLogFolder = string.Empty;
        private IniFile mIni = new IniFile(Application.StartupPath + "\\cfg.ini");

        public Logger()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            string folder = mIni.IniReadValue("config", "Logger");
            this.textBox1.Text = folder;
            mLogFolder = folder;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select log file folder";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
                mLogFolder = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mIni.IniWriteValue("config", "Logger", mLogFolder);
            Close();
        }

        private void Logger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Directory.Exists(mLogFolder))
                e.Cancel = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            mLogFolder = this.textBox1.Text;
        }
    }
}
