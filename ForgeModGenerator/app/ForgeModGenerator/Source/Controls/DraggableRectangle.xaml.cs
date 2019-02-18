using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public partial class DraggableRectangle : UserControl
    {
        public DraggableRectangle() => InitializeComponent();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            Window window = Application.Current.MainWindow;
            Point mousePos = Mouse.GetPosition(window);
            if (window.WindowState == WindowState.Maximized
                && ActualHeight < ActualWidth
                && mousePos.Y <= ActualHeight)
            {
                window.WindowState = WindowState.Normal;
                window.Left = mousePos.X - (ActualWidth / 2);
                window.Top = mousePos.Y;
            }
            window.DragMove();
        }
    }
}
