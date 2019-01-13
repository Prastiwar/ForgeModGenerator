using ForgeModGenerator.Core;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public MainWindowViewModel(INavigationService navigationService, ISessionContextService sessionContext)
        {
            this.navigationService = navigationService;
            SessionContext = sessionContext;
        }

        public ISessionContextService SessionContext { get; }

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

        protected ObservableCollection<Mod> FindMods()
        {
            ObservableCollection<Mod> foundMods = new ObservableCollection<Mod>();
            string[] modPaths = Directory.GetDirectories(AppPaths.Mods);
            foreach (string modPath in modPaths)
            {
                foundMods.Add(Mod.Import(modPath));
            }
            return foundMods;
        }

    }
}
