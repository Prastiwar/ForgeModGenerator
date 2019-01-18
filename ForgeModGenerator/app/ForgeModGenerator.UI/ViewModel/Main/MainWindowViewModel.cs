using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public ISessionContextService SessionContext { get; }

        public MainWindowViewModel(INavigationService navigationService, ISessionContextService sessionContext)
        {
            this.navigationService = navigationService;
            SessionContext = sessionContext;
        }

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new RelayCommand(() => { navigationService.NavigateTo(ViewModelLocator.Pages.Settings); }));

        private ICommand refresh;
        public ICommand Refresh => refresh ?? (refresh = new RelayCommand(ForceRefresh));

        private void ForceRefresh()
        {
            Mod mod = SessionContext.SelectedMod;
            SessionContext.SelectedMod = null;
            SessionContext.SelectedMod = mod;
        }
    }
}
