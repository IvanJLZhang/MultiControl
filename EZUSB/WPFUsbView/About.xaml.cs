using System.Windows;
using System.Windows.Documents;

namespace WPFUsbView
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            if (hl != null)
            {
                Splash.Diagnostics.Extensions.ShellExecute(hl.NavigateUri.AbsoluteUri);
            }
        }
    }
}
