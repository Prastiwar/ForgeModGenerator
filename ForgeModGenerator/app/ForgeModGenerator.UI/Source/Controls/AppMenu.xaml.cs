using ForgeModGenerator.UI.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.UI.Controls
{
    public partial class AppMenu : UserControl
    {
        public AppMenu()
        {
            InitializeComponent();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            MainWindow.ActiveWindow.WindowState = WindowState.Minimized;
        }

        private void Restore(object sender, RoutedEventArgs e)
        {
            MainWindow.ActiveWindow.WindowState = MainWindow.ActiveWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private async void ExitApp(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200); // wait for click animation
            Environment.Exit(0);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
        }
    }
}
