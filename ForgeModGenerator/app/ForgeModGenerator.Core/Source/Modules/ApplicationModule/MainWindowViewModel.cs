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

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new DelegateCommand(NavigateToSettings));

        private ICommand openDashboard;
        public ICommand OpenDashboard => openDashboard ?? (openDashboard = new DelegateCommand(() => { navigation.NavigateTo(Pages.Dashboard); }));

        private ICommand openModGenerator;
        public ICommand OpenModGenerator => openModGenerator ?? (openModGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.ModGenerator); }));

        private ICommand openBuildConfiguration;
        public ICommand OpenBuildConfiguration => openBuildConfiguration ?? (openBuildConfiguration = new DelegateCommand(() => { navigation.NavigateTo(Pages.BuildConfiguration); }));

        private ICommand openTextureGenerator;
        public ICommand OpenTextureGenerator => openTextureGenerator ?? (openTextureGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.TextureGenerator); }));

        private ICommand openBlockGenerator;
        public ICommand OpenBlockGenerator => openBlockGenerator ?? (openBlockGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.BlockGenerator); }));

        private ICommand openItemGenerator;
        public ICommand OpenItemGenerator => openItemGenerator ?? (openItemGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.ItemGenerator); }));

        private ICommand openSoundGenerator;
        public ICommand OpenSoundGenerator => openSoundGenerator ?? (openSoundGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.SoundGenerator); }));

        private ICommand openCommandGenerator;
        public ICommand OpenCommandGenerator => openCommandGenerator ?? (openCommandGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.CommandGenerator); }));

        private ICommand openAchievementGenerator;
        public ICommand OpenAchievementGenerator => openAchievementGenerator ?? (openAchievementGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.AchievementGenerator); }));

        private ICommand openRecipeGenerator;
        public ICommand OpenRecipeGenerator => openRecipeGenerator ?? (openRecipeGenerator = new DelegateCommand(() => { navigation.NavigateTo(Pages.RecipeGenerator); }));
        
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

        private void ForceRefresh() => SessionContext.Refresh();

        private void NavigateToSettings() => navigation.NavigateTo(Pages.Settings);
    }
}
