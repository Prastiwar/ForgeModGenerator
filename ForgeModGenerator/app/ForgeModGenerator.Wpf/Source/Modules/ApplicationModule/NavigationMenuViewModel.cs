using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> NavigationMenu Business ViewModel </summary>
    public class NavigationMenuViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public NavigationMenuViewModel(IRegionManager regionManager) => this.regionManager = regionManager;

        private ICommand openDashboard;
        public ICommand OpenDashboard => openDashboard ?? (openDashboard = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.Dashboard); }));

        private ICommand openModGenerator;
        public ICommand OpenModGenerator => openModGenerator ?? (openModGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.ModGenerator); }));

        private ICommand openBuildConfiguration;
        public ICommand OpenBuildConfiguration => openBuildConfiguration ?? (openBuildConfiguration = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.BuildConfiguration); }));

        private ICommand openTextureGenerator;
        public ICommand OpenTextureGenerator => openTextureGenerator ?? (openTextureGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.TextureGenerator); }));

        private ICommand openBlockGenerator;
        public ICommand OpenBlockGenerator => openBlockGenerator ?? (openBlockGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.BlockGenerator); }));

        private ICommand openItemGenerator;
        public ICommand OpenItemGenerator => openItemGenerator ?? (openItemGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.ItemGenerator); }));

        private ICommand openSoundGenerator;
        public ICommand OpenSoundGenerator => openSoundGenerator ?? (openSoundGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.SoundGenerator); }));

        private ICommand openCommandGenerator;
        public ICommand OpenCommandGenerator => openCommandGenerator ?? (openCommandGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.CommandGenerator); }));

        private ICommand openAchievementGenerator;
        public ICommand OpenAchievementGenerator => openAchievementGenerator ?? (openAchievementGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.AchievementGenerator); }));

        private ICommand openRecipeGenerator;
        public ICommand OpenRecipeGenerator => openRecipeGenerator ?? (openRecipeGenerator = new DelegateCommand(() => { regionManager.RequestNavigate(Pages.RegionName, Pages.RecipeGenerator); }));
    }
}
