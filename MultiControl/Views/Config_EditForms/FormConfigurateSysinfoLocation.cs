using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiControl
{
    public partial class FormConfigurateSysinfoLocation : Form
    {
        public string mLogFolder = string.Empty;
        private IniFile mIni = new IniFile(Application.StartupPath + "\\cfg.ini");
        public FormConfigurateSysinfoLocation()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            string folder = mIni.IniReadValue("config", "Folder");
            this.textBox1.Text = folder;
            mLogFolder = folder;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select sysinfo.cfg  file folder";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
                mLogFolder = fbd.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
              5. 从电脑上发送文件到设备
            adb push <本地路径> <远程路径>
            用push命令可以把本机电脑上的文件或者文件夹复制到设备(手机)
            6. 从设备上下载文件到电脑
            adb pull <远程路径> <本地路径>*/
            //  mIni.IniWriteValue("config", "Folder", mLogFolder);
            string startCmd = "adb -s " + saveMSD.mNumber + " pull " + saveMSD.SD + "/Android/data/com.wistron.get.config.information/files/sysinfo.cfg  " + mLogFolder;
            string x = Execute(startCmd);
            Close();
        }

        public string Execute(string dosCommand)
        {
            return Execute(dosCommand, 0);
        }
        public string Execute(string command, int seconds)
        {
            string output = string.Empty; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = true; //重定向输出
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;//不创建窗口  
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程  
                    {
                        //output = process.StandardOutput.ReadToEnd();//读取进程的输出  
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出
                        if (string.IsNullOrEmpty(output))
                        {
                            output = process.StandardError.ReadToEnd();
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
