using System.Windows;

namespace ForgeModGenerator.View
{
    /// <summary> MainWindow UI View-ViewModel </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GenMenu.InitializeMenu(ContentGrid, 0, 0);
        }
    }
}
