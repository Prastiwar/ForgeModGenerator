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

        private ICommand openDashboardCommand;
        public ICommand OpenDashboardCommand => openDashboardCommand ?? (openDashboardCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.Dashboard); }));

        private ICommand openModGeneratorCommand;
        public ICommand OpenModGeneratorCommand => openModGeneratorCommand ?? (openModGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.ModGenerator); }));

        private ICommand openBuildConfigurationCommand;
        public ICommand OpenBuildConfigurationCommand => openBuildConfigurationCommand ?? (openBuildConfigurationCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.BuildConfiguration); }));

        private ICommand openTextureGeneratorCommand;
        public ICommand OpenTextureGeneratorCommand => openTextureGeneratorCommand ?? (openTextureGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.TextureGenerator); }));

        private ICommand openBlockGeneratorCommand;
        public ICommand OpenBlockGeneratorCommand => openBlockGeneratorCommand ?? (openBlockGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.BlockGenerator); }));

        private ICommand openItemGeneratorCommand;
        public ICommand OpenItemGeneratorCommand => openItemGeneratorCommand ?? (openItemGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.ItemGenerator); }));

        private ICommand openSoundGeneratorCommand;
        public ICommand OpenSoundGeneratorCommand => openSoundGeneratorCommand ?? (openSoundGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.SoundGenerator); }));

        private ICommand openCommandGeneratorCommand;
        public ICommand OpenCommandGeneratorCommand => openCommandGeneratorCommand ?? (openCommandGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.CommandGenerator); }));

        private ICommand openAchievementGeneratorCommand;
        public ICommand OpenAchievementGeneratorCommand => openAchievementGeneratorCommand ?? (openAchievementGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.AchievementGenerator); }));

        private ICommand openRecipeGeneratorCommand;
        public ICommand OpenRecipeGeneratorCommand => openRecipeGeneratorCommand ?? (openRecipeGeneratorCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.RecipeGenerator); }));
        
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
