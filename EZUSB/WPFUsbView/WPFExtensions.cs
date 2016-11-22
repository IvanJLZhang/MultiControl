/* ----------------------------------------------------------
文件名称：WPFExtensions.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

博客：http://blog.csdn.net/jhqin

开发环境：
    Visual Studio V2010
    .NET Framework 4 Client Profile

版本历史：
    V1.1    2011年11月07日
            实现扩展方法：WPF中TreeView类的ExpandAll功能

    V1.0	2011年11月03日
			实现扩展方法：WPF中Button类的PerformClick功能
------------------------------------------------------------ */
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Splash.WPF
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 扩展方法：实现WPF中Button类的PerformClick功能
        /// </summary>
        /// <param name="button">Button实例</param>
        /// <remarks>
        /// 需要添加对UIAutomationProvider.dll的引用
        /// 参考网址：http://www.cnblogs.com/zhouyinhui/archive/2010/05/20/1740111.html
        /// </remarks>
        public static void PerformClick(this Button button)
        {
            ButtonAutomationPeer BAP = new ButtonAutomationPeer(button);
            IInvokeProvider IIP = BAP.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            if (IIP != null)
            {
                IIP.Invoke();
            }
        }
     
        /// <summary>
        /// 扩展方法：实现WPF中TreeView类的ExpandAll功能
        /// </summary>
        /// <param name="treeView">要展开的TreeView实例</param>
        /// <remarks>
        /// 参考网址：http://www.cnblogs.com/sayo/archive/2008/07/23/1249804.html        
        /// </remarks>
        public static void ExpandAll(this TreeView treeView)
        {
            ExpandSubItems(treeView as ItemsControl);
        }

        private static void ExpandSubItems(ItemsControl control)
        {
            if (control == null) return;
            foreach (object item in control.Items)
            {
                TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null && treeItem.HasItems)
                {
                    treeItem.IsExpanded = true;
                    ExpandSubItems(treeItem as ItemsControl);
                }               
            }
        }        
    }
}
