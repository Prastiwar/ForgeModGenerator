using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.UI
{
    public partial class MainWindow : Window
    {
        protected Menu leftMenu;

        public MainWindow()
        {
            InitializeComponent();
            leftMenu = new Menu(LeftMenuGrid, new MenuSettings(LeftMenuGrid, 0, 0, new Vector(250, 0), 0.3));
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Restore(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private async void ExitApp(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200); // wait for animation
            Environment.Exit(0);
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleLeftMenu(object sender, RoutedEventArgs e)
        {
            leftMenu.Toggle();
        }

        private void Open(object sender, RoutedEventArgs e)
        {

        }
    }
}
