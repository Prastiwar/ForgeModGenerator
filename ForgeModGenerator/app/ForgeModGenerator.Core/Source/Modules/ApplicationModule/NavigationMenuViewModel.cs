using ForgeModGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> NavigationMenu Business ViewModel </summary>
    public class NavigationMenuViewModel : BindableBase
    {
        private readonly INavigationService navigation;

        public NavigationMenuViewModel(INavigationService navigation) => this.navigation = navigation;

        private ICommand openDashboard;
        public ICommand OpenDashboard => openDashboard ?? (openDashboard = new DelegateCommand(() => { navigation.NavigateTo(Pages.Dashboard); }));

        private ICommand openModGenerator;
        public ICommand OpenModGenerator => openModGenerator ?? (openModGenerator = new DelegateCommand(() => { navigation.NavigateTo( Pages.ModGenerator); }));

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

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand => openSettingsCommand ?? (openSettingsCommand = new DelegateCommand(() => { navigation.NavigateTo(Pages.Settings); }));
    }
}
