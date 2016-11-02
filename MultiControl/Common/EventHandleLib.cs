/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/25 17:10:57 
 * 文件名：EventHandleLib 
 * 版本：V1.0.0 
 * 文件说明：
 * 定义事件处理委托
 * 
 * 修改者：           
 * 时间：               
 * 修改说明： 
* ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiControl.Common
{
    public delegate void OnStartUpdateHandle(object sender, ResultEventArgs e);

    public delegate void OnResultUpdateHandle(object sender, ResultEventArgs e);

    public delegate void OnProgressUpdateHandle(object sender, TestProgressEventArgs e);

    public delegate void OnFinishUpdateHandle(object sender, TestProgressEventArgs e);

    public delegate void OnProcessExitedHandle(object sender, ProcessExitAgs e);
}
