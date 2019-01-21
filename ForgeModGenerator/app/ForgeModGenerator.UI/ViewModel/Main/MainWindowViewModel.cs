using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private readonly IModBuildService modBuilder;

        public ISessionContextService SessionContext { get; }

        public MainWindowViewModel(INavigationService navigationService, ISessionContextService sessionContext, IModBuildService modBuilder)
        {
            this.navigationService = navigationService;
            this.modBuilder = modBuilder;
            SessionContext = sessionContext;
        }

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new RelayCommand(() => { navigationService.NavigateTo(ViewModelLocator.Pages.Settings); }));

        private ICommand refreshCommand;
        public ICommand RefreshCommand => refreshCommand ?? (refreshCommand = new RelayCommand(ForceRefresh));

        private ICommand runModCommand;
        public ICommand RunModCommand => runModCommand ?? (runModCommand = new RelayCommand<Mod>(RunSelectedMod));

        private ICommand runModsCommand;
        public ICommand RunModsCommand => runModsCommand ?? (runModsCommand = new RelayCommand<ObservableCollection<Mod>>(RunSelectedMods));

        private void RunSelectedMods(ObservableCollection<Mod> mods)
        {
            foreach (Mod mod in mods)
            {
                modBuilder.Run(mod);
            }
        }

        private void RunSelectedMod(Mod mod)
        {
            modBuilder.Run(mod);
        }

        private void ForceRefresh()
        {
            Mod mod = SessionContext.SelectedMod;
            SessionContext.SelectedMod = null;
            SessionContext.SelectedMod = mod;
            Log.Info("Force Refresh called");
        }
    }
}
