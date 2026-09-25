using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TaskReward.ViewModels;

namespace TaskReward
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void TitleBar_MouseUp(object sender, MouseButtonEventArgs e) { }

        private void ToggleMaximize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                UpdateShadowForMaximize(false);
            }
            else
            {
                WindowState = WindowState.Maximized;
                UpdateShadowForMaximize(true);
            }
        }

        private void UpdateShadowForMaximize(bool max)
        {
            // 最大化时去掉圆角与阴影，避免黑边
            var b = FindName("ShadowRoot") as Border ?? null;
            if (b != null)
            {
                if (max) { b.CornerRadius = new CornerRadius(0); b.Effect = null; }
                else { b.CornerRadius = new CornerRadius(16); b.Effect = new DropShadowEffect
                    { BlurRadius = 40, Opacity = 0.35, Color = Color.FromRgb(0x15,0x17,0x38) }; }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximize_Click(object sender, RoutedEventArgs e) => ToggleMaximize();
        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Close_Hover(object sender, RoutedEventArgs e)
            => (sender as Button).Background = new SolidColorBrush(Color.FromRgb(0xE5,0x48,0x4D));
        private void Close_Leave(object sender, RoutedEventArgs e)
            => (sender as Button).Background = Brushes.Transparent;
    }
}
