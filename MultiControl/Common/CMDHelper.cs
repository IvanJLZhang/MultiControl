/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/25 16:50:53 
 * 文件名：CMDHelper 
 * 版本：V1.0.0 
 * 文件说明：
 * 
 * 
 * 修改者：           
 * 时间：               
 * 修改说明： 
* ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiControl.Common;

namespace MultiControl.Lib
{
    public class CMDHelper
    {

        public static bool adb_service_start = true;

        private string m_command = String.Empty;
        //Event
        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;
        public event OnProcessExitedHandle Exited;

        public const int WAIT_FOR_MI = 1000 * 10;// 等待cmd命令执行完毕时间（10s）
                                                 // 采用异步处理时不需要此参数

        public string StartFileName { get; set; } = "cmd.exe";// 默认处理cmd命令
        /// <summary>
        /// 异步执行cmd命令， 适用于需要经过一段时间才能得出全部返回结果的操作
        /// </summary>
        /// <param name="command"></param>
        public void CMD_Run(string command)
        {
            m_command = command;
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName;      // 命令  
            //CmdProcess.StartInfo.Arguments = command;           // 参数  

            CmdProcess.StartInfo.CreateNoWindow = true;         // 不创建新窗口  
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;  // 重定向输入  
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出  
            CmdProcess.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
                                                                //CmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  

            CmdProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            CmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);

            CmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件  
            CmdProcess.Exited += new EventHandler(CmdProcess_Exited);   // 注册进程结束事件  

            CmdProcess.Start();

            CmdProcess.StandardInput.WriteLine(command);
            System.Threading.Thread.Sleep(1000);
            CmdProcess.StandardInput.WriteLine("exit");

            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
        }
        /// <summary>
        /// 同步执行cmd命令， 适用于可以快速返回结果的操作
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CMD_RunEx(string command)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.CreateNoWindow = true;
            CmdProcess.StartInfo.FileName = StartFileName;
            //CmdProcess.StartInfo.Arguments = command;

            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardError = true;
            CmdProcess.StartInfo.RedirectStandardInput = true;
            CmdProcess.StartInfo.RedirectStandardOutput = true;
            CmdProcess.Start();

            CmdProcess.StandardInput.WriteLine(command);
            common.m_log.Add_Debug(command);

            CmdProcess.WaitForExit(WAIT_FOR_MI);
            //CmdProcess.WaitForExit();
            CmdProcess.StandardInput.WriteLine("exit");
            //CmdProcess.WaitForExit(WAIT_FOR_MI);
            string outStr = String.Empty;
            outStr = CmdProcess.StandardOutput.ReadToEnd();
            //CmdProcess.StandardOutput.fl
            CmdProcess.Close();
            return common.FilterCommandResult(outStr, command);
        }

        public async Task<string> Execute(string command, int seconds, bool NeedResponseResult = true)
        {
            string output = string.Empty; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                #region 旧的
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                startInfo.RedirectStandardInput = true;//不重定向输入  
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
                        if (NeedResponseResult)
                        {
                            output = await process.StandardOutput.ReadToEndAsync();//读取进程的输出
                            if (string.IsNullOrEmpty(output))
                            {
                                output = await process.StandardError.ReadToEndAsync();
                            }
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
                    common.m_log.Add_Debug(command);
                }
                #endregion
            }
            return output;
        }

        public async Task<string> CMD_RunAsync(string command, int seconds = 0, bool NeedResponseResult = true)
        {
            while (!CMDHelper.adb_service_start)
            {
                await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
            }
            string output = string.Empty; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                #region 旧的
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                startInfo.RedirectStandardInput = true;//不重定向输入  
                startInfo.RedirectStandardOutput = true; //重定向输出
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;//不创建窗口  
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程  
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                        }
                        if (NeedResponseResult)
                        {
                            output = await process.StandardOutput.ReadToEndAsync();//读取进程的输出
                            if (string.IsNullOrEmpty(output))
                            {
                                output = await process.StandardError.ReadToEndAsync();
                            }
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
                    //common.m_log.Add_Debug(command);
                }
                #endregion
            }
            return output;
        }


        public static string Adb_StartServer()
        {
            Process process = new Process();//创建进程对象  
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";//设定需要执行的命令  
            startInfo.Arguments = "/C " + AdbCommand.Adb_start_server;//“/C”表示执行完命令后马上退出  
            startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
            startInfo.RedirectStandardInput = true;//不重定向输入  
            startInfo.RedirectStandardOutput = true; //重定向输出
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;//不创建窗口  
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
            //string output = await process.StandardOutput.ReadToEndAsync();
            process.Close();
            adb_service_start = true;
            common.m_log.Add("Starting adb server.");
            return "";
        }

        public static async Task<string> Adb_KillServer()
        {
            Process process = new Process();//创建进程对象  
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";//设定需要执行的命令  
            startInfo.Arguments = "/C " + AdbCommand.Adb_kill_server;//“/C”表示执行完命令后马上退出  
            startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
            startInfo.RedirectStandardInput = true;//不重定向输入  
            startInfo.RedirectStandardOutput = true; //重定向输出
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;//不创建窗口  
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
            string output = await process.StandardOutput.ReadToEndAsync();
            process.Close();
            adb_service_start = false;
            return output;
        }
        /// <summary>
        /// 通过adb下达获取设备型号的命令检测设备是否连接
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<bool> CheckDeviceConnection(UsbDeviceInfoEx device)
        {
            string command = $"adb -s {device.SerialNumber} shell getprop ro.product.model";
            int count = 0;
            string response = String.Empty;

            while ((String.IsNullOrEmpty(response) || response.Contains("error") || response.Contains("offline"))
                && count <= config_inc.CMD_REPEAT_MAX_TIME)
            {
                common.m_log.Add_Debug(command);
                response = await CMD_RunAsync(command);
                common.m_log.Add_Debug(response);

                await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
                count++;
            }
            if (String.IsNullOrEmpty(response) || response.Contains("error") || response.Contains("offline"))
            {
                common.m_log.Add(response, LogHelper.MessageType.ERROR);
                return false;
            }
            device.ModelName = response.Trim();
            return true;
        }


        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            Exited?.Invoke(sender, new ProcessExitAgs() { Command = m_command });
            m_command = String.Empty;
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            ErrorDataReceived?.Invoke(sender, e);
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputDataReceived?.Invoke(sender, e);
        }
    }
}
