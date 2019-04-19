using System.Reflection;
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

        private MethodInfo activateFrame;
        private MethodInfo ActivateFrame => activateFrame ?? (activateFrame = FrameTransitioner.GetType().GetMethod("ActivateFrame", BindingFlags.Instance | BindingFlags.NonPublic));

        private readonly object[] navigateBackArgs = new object[] { -1, 0 };
        private readonly object[] navigateForwardArgs = new object[] { 0, 1 };

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ApplicationMenu.CanQuit())
            {
                e.Cancel = true;
            }
        }

        private void PageFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) => ActivateFrame.Invoke(FrameTransitioner, navigateBackArgs);
        private void PageFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e) => ActivateFrame.Invoke(FrameTransitioner, navigateForwardArgs);
    }
}
