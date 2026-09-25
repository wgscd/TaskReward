using System.Windows;
using System.Windows.Threading;

namespace TaskReward
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show("程序遇到一个问题：\n\n" + args.Exception.Message +
                                "\n\n（应用已尽力保留已有数据，请重新启动）",
                                "成长星 · 提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                args.Handled = true;
            };
        }
    }
}
