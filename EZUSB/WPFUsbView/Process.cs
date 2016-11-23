/* ----------------------------------------------------------
文件名称：Process.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：
    V1.0	2011年06月30日
			实现ShellExecute函数
------------------------------------------------------------ */
using System;

namespace Splash.Diagnostics
{
    public static partial class Extensions
    {   
        /// <summary>
        /// 以命令行方式操作一个文件
        /// </summary>
        /// <param name="CommandLine">命令行</param>
        public static void ShellExecute(String CommandLine)
        {
            // 创建进程
            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            // 分离文件名和路径
            // 定位路径
            int IndexA = CommandLine.LastIndexOf('\\');
            if (IndexA >= 0)
            {   // 设定工作目录
                pro.StartInfo.WorkingDirectory = CommandLine.Substring(0, IndexA);
            }

            // 定位文件名，判断是否带参数
            IndexA++;
            int IndexB = CommandLine.IndexOf(' ', IndexA);
            if (IndexB >= 0)
            {   // 带有参数
                pro.StartInfo.FileName = CommandLine.Substring(IndexA, IndexB - IndexA);
                pro.StartInfo.Arguments = CommandLine.Substring(IndexB + 1);
            }
            else
            {   // 不带参数
                pro.StartInfo.FileName = CommandLine.Substring(IndexA);
            }

            pro.Start();
        }
    }
}
