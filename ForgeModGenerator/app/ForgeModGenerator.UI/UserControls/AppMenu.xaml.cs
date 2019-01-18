using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ForgeModGenerator.UserControls
{
    public partial class AppMenu : UserControl
    {
        public AppMenu()
        {
            InitializeComponent();
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
            DependencyProperty.Register("MinimizeCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(new RelayCommand(Minimize)));
        public ICommand MinimizeCommand {
            get => (ICommand)GetValue(MinimizeCommandProperty);
            set => SetValue(MinimizeCommandProperty, value);
        }

        public static readonly DependencyProperty RestoreCommandProperty =
            DependencyProperty.Register("RestoreCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(new RelayCommand(Restore)));
        public ICommand RestoreCommand {
            get => (ICommand)GetValue(RestoreCommandProperty);
            set => SetValue(RestoreCommandProperty, value);
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(AppMenu), new PropertyMetadata(new RelayCommand(Quit)));
        public ICommand CloseCommand {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        private static void Minimize() => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        private static void Restore() => Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private static async void Quit()
        {
            await Task.Delay(150); // to show click effect
            Environment.Exit(0);
        }
    }
}
