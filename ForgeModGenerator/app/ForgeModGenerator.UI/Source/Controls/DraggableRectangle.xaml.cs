using ForgeModGenerator.UI.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.UI.Controls
{
    public partial class DraggableRectangle : UserControl
    {
        public DraggableRectangle()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ActiveWindow.DragMove();
        }
    }
}
