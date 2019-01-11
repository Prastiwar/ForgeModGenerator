using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.View.Controls
{
    public partial class DraggableRectangle : UserControl
    {
        public DraggableRectangle()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e) => Application.Current.MainWindow.DragMove();
    }
}
