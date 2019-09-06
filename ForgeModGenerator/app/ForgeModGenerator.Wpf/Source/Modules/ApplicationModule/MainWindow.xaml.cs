using ForgeModGenerator.Components;
using Prism.Commands;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.Views
{
    /// <summary> MainWindow UI View-ViewModel </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu(ContentGrid, 0, 0);
        }

        public MenuComponent MenuComponent { get; protected set; }
        protected double MenuSlideSpeed { get; set; } = 0.25;
        protected Vector MenuSlideOffset { get; set; } = new Vector(170, 0);

        private ICommand toggleMenuCommand;
        public ICommand ToggleMenuCommand => toggleMenuCommand ?? (toggleMenuCommand = new DelegateCommand(MenuComponent.Toggle));

        private MethodInfo activateFrame;
        private MethodInfo ActivateFrame => activateFrame ?? (activateFrame = FrameTransitioner.GetType().GetMethod("ActivateFrame", BindingFlags.Instance | BindingFlags.NonPublic));

        private readonly object[] navigateBackArgs = new object[] { -1, 0 };
        private readonly object[] navigateForwardArgs = new object[] { 0, 1 };

        protected void InitializeMenu(Grid menuGrid, int row, int column) => MenuComponent = new MenuComponent(menuGrid, new MenuSettings(menuGrid, column, row, MenuSlideOffset, MenuSlideSpeed));

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
