using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> MainWindow Business ViewModel </summary>
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(INavigationService navigation, ISessionContextService sessionContext, IModBuildService modBuilder)
        {
            this.navigation = navigation;
            this.modBuilder = modBuilder;
            SessionContext = sessionContext;
        }

        private readonly INavigationService navigation;
        private readonly IModBuildService modBuilder;

        public ISessionContextService SessionContext { get; }

        public ICommand GetOpenPageCommand(string page) => new DelegateCommand(() => navigation.NavigateTo(page));

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new DelegateCommand(NavigateToSettings));

        private ICommand refreshCommand;
        public ICommand RefreshCommand => refreshCommand ?? (refreshCommand = new DelegateCommand(ForceRefresh));

        private ICommand runModCommand;
        public ICommand RunModCommand => runModCommand ?? (runModCommand = new DelegateCommand<McMod>(RunSelectedMod));

        private ICommand runModsCommand;
        public ICommand RunModsCommand => runModsCommand ?? (runModsCommand = new DelegateCommand<ObservableCollection<McMod>>(RunSelectedMods));

        private void RunSelectedMods(ObservableCollection<McMod> mods)
        {
            foreach (McMod mcMod in mods)
            {
                modBuilder.Run(mcMod);
            }
        }

        private void RunSelectedMod(McMod mcMod) => modBuilder.Run(mcMod);

        private void ForceRefresh() => SessionContext.Refresh();

        private void NavigateToSettings() => navigation.NavigateTo(Pages.Settings);
    }
}
