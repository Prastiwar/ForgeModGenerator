using ForgeModGenerator.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public MainWindowViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            StartPage = new Uri("DashboardPage.xaml", UriKind.Relative);
        }

        public Uri StartPage { get; protected set; }

        private ObservableCollection<Mod> mods;
        public ObservableCollection<Mod> Mods {
            get => mods;
            set => Set(ref mods, value);
        }

        private Mod selectedMod;
        public Mod SelectedMod {
            get => selectedMod;
            set => Set(ref selectedMod, value);
        }

        private ICommand openSettings;
        public ICommand OpenSettings { get => openSettings ?? (openSettings = new RelayCommand(() => { navigationService.NavigateTo(ViewModelLocator.Pages.Settings); })); }

        private ICommand exitApp;
        public ICommand ExitApp { get => exitApp ?? (exitApp = new RelayCommand(QuitApp)); }

        private ICommand minimize;
        public ICommand Minimize { get => minimize ?? (minimize = new RelayCommand(() => { Application.Current.MainWindow.WindowState = WindowState.Minimized; })); }

        private ICommand restore;
        public ICommand Restore {
            get => restore ?? (restore = new RelayCommand(() =>
            {
                Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }));
        }

        private async void QuitApp()
        {
            await Task.Delay(150); // wait for click animation
            Environment.Exit(0);
        }

    }
}
