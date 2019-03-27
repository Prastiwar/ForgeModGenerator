using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IModBuildService modBuilder;

        public ISessionContextService SessionContext { get; }

        public MainWindowViewModel(IRegionManager regionManager, ISessionContextService sessionContext, IModBuildService modBuilder)
        {
            this.regionManager = regionManager;
            this.modBuilder = modBuilder;
            SessionContext = sessionContext;
        }

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new DelegateCommand(() => { regionManager.RequestNavigate("Page", Pages.Settings); }));

        private ICommand refreshCommand;
        public ICommand RefreshCommand => refreshCommand ?? (refreshCommand = new DelegateCommand(ForceRefresh));

        private ICommand runModCommand;
        public ICommand RunModCommand => runModCommand ?? (runModCommand = new DelegateCommand<Mod>(RunSelectedMod));

        private ICommand runModsCommand;
        public ICommand RunModsCommand => runModsCommand ?? (runModsCommand = new DelegateCommand<ObservableCollection<Mod>>(RunSelectedMods));

        private void RunSelectedMods(ObservableCollection<Mod> mods)
        {
            foreach (Mod mod in mods)
            {
                modBuilder.Run(mod);
            }
        }

        private void RunSelectedMod(Mod mod) => modBuilder.Run(mod);

        private void ForceRefresh()
        {
            Mod mod = SessionContext.SelectedMod;
            SessionContext.SelectedMod = null;
            SessionContext.SelectedMod = mod;
            SessionContext.Refresh();
        }
    }
}
