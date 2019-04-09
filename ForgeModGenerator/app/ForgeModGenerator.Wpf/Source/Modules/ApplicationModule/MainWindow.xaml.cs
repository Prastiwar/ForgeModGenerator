using System.Windows;

namespace ForgeModGenerator.ApplicationModule.Views
{
    /// <summary> MainWindow UI View-ViewModel </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationMenu.InitializeMenu(ContentGrid, 0, 0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ApplicationMenu.CanQuit())
            {
                e.Cancel = true;
            }
        }
    }
}
