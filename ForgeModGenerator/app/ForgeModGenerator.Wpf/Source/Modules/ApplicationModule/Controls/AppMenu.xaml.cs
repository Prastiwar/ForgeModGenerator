using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ForgeModGenerator.ApplicationModule.Controls
{
    public partial class AppMenu : UserControl
    {
        public AppMenu()
        {
            InitializeComponent();

            SettingsButton.Width = MinimizeButton.Width = RestoreButton.Width = CloseButton.Width = ItemWidth;
            SettingsButton.Height = MinimizeButton.Height = RestoreButton.Height = CloseButton.Height = ItemHeight;
            SettingsButton.Background = MinimizeButton.Background = RestoreButton.Background = CloseButton.Background = ItemBackground;
            SettingsButton.BorderBrush = MinimizeButton.BorderBrush = RestoreButton.BorderBrush = CloseButton.BorderBrush = ItemBorderBrush;
            SettingsButton.Margin = MinimizeButton.Margin = RestoreButton.Margin = CloseButton.Margin = ItemMargin;

            if (CloseCommand == null)
            {
                CloseCommand = new DelegateCommand(Quit);
            }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(AppMenu), new PropertyMetadata(15.0));
        public double ItemHeight {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(AppMenu), new PropertyMetadata(35.0));
        public double ItemWidth {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public static readonly DependencyProperty ContentOrientationProperty =
            DependencyProperty.Register("ContentOrientation", typeof(Orientation), typeof(AppMenu), new PropertyMetadata(Orientation.Horizontal));
        public Orientation ContentOrientation {
            get => (Orientation)GetValue(ContentOrientationProperty);
            set => SetValue(ContentOrientationProperty, value);
        }

        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(AppMenu), new PropertyMetadata(new BrushConverter().ConvertFrom("#FFCD2929")));
        public Brush ItemBackground {
            get => (Brush)GetValue(ItemBackgroundProperty);
            set => SetValue(ItemBackgroundProperty, value);
        }

        public static readonly DependencyProperty ItemBorderBrushProperty =
            DependencyProperty.Register("ItemBorderBrush", typeof(Brush), typeof(AppMenu), new PropertyMetadata(new BrushConverter().ConvertFrom("#FF3C0000")));
        public Brush ItemBorderBrush {
            get => (Brush)GetValue(ItemBorderBrushProperty);
            set => SetValue(ItemBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(AppMenu), new PropertyMetadata(new Thickness(2.5, 0, 2.5, 0)));
        public Thickness ItemMargin {
            get => (Thickness)GetValue(ItemMarginProperty);
            set => SetValue(ItemMarginProperty, value);
        }

        public static readonly DependencyProperty SettingsIconProperty =
            DependencyProperty.Register("SettingsIcon", typeof(string), typeof(AppMenu), new PropertyMetadata("Settings"));
        public string SettingsIcon {
            get => (string)GetValue(SettingsIconProperty);
            set => SetValue(SettingsIconProperty, value);
        }

        public static readonly DependencyProperty MinimizeIconProperty =
            DependencyProperty.Register("MinimizeIcon", typeof(string), typeof(AppMenu), new PropertyMetadata("WindowMinimize"));
        public string MinimizeIcon {
            get => (string)GetValue(MinimizeIconProperty);
            set => SetValue(MinimizeIconProperty, value);
        }

        public static readonly DependencyProperty RestoreIconProperty =
            DependencyProperty.Register("RestoreIcon", typeof(string), typeof(AppMenu), new PropertyMetadata("WindowMaximize"));
        public string RestoreIcon {
            get => (string)GetValue(RestoreIconProperty);
            set => SetValue(RestoreIconProperty, value);
        }

        public static readonly DependencyProperty CloseIconProperty =
            DependencyProperty.Register("CloseIcon", typeof(string), typeof(AppMenu), new PropertyMetadata("Close"));
        public string CloseIcon {
            get => (string)GetValue(CloseIconProperty);
            set => SetValue(CloseIconProperty, value);
        }

        public static readonly DependencyProperty OpenSettingsCommandProperty =
            DependencyProperty.Register("OpenSettingsCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(null));
        public ICommand OpenSettingsCommand {
            get => (ICommand)GetValue(OpenSettingsCommandProperty);
            set => SetValue(OpenSettingsCommandProperty, value);
        }

        public static readonly DependencyProperty MinimizeCommandProperty =
            DependencyProperty.Register("MinimizeCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(new DelegateCommand(Minimize)));
        public ICommand MinimizeCommand {
            get => (ICommand)GetValue(MinimizeCommandProperty);
            set => SetValue(MinimizeCommandProperty, value);
        }

        public static readonly DependencyProperty RestoreCommandProperty =
            DependencyProperty.Register("RestoreCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(new DelegateCommand(Restore)));
        public ICommand RestoreCommand {
            get => (ICommand)GetValue(RestoreCommandProperty);
            set => SetValue(RestoreCommandProperty, value);
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(null));
        public ICommand CloseCommand {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly DependencyProperty AskToCloseProperty =
            DependencyProperty.Register("AskToClose", typeof(bool), typeof(AppMenu), new PropertyMetadata(false));
        public bool AskToClose {
            get => (bool)GetValue(AskToCloseProperty);
            set => SetValue(AskToCloseProperty, value);
        }

        private static void Minimize() => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        private static void Restore() => Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private async void Quit()
        {
            if (CanQuit())
            {
                await Task.Delay(150).ConfigureAwait(false); // to show click effect
                Environment.Exit(0);
            }
        }

        public bool CanQuit()
        {
            if (AskToClose)
            {
                MessageBoxResult result = MessageBox.Show("You have unsaved changes, are you sure you want to quit? Changes will be lost.", "Are you sure you want to exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
