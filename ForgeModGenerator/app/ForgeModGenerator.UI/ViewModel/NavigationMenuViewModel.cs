using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Views;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> NavigationMenu Business ViewModel </summary>
    public class NavigationMenuViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public NavigationMenuViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private ICommand openDashboard;
        public ICommand OpenDashboard { get => openDashboard ?? (openDashboard = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.Dashboard); })); }

        private ICommand openModGenerator;
        public ICommand OpenModGenerator { get => openModGenerator ?? (openModGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.ModGenerator); })); }

        private ICommand openBlockGenerator;
        public ICommand OpenBlockGenerator { get => openBlockGenerator ?? (openBlockGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.BlockGenerator); })); }

        private ICommand openItemGenerator;
        public ICommand OpenItemGenerator { get => openItemGenerator ?? (openItemGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.ItemGenerator); })); }

        private ICommand openSoundGenerator;
        public ICommand OpenSoundGenerator { get => openSoundGenerator ?? (openSoundGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.SoundGenerator); })); }

        private ICommand openCommandGenerator;
        public ICommand OpenCommandGenerator { get => openCommandGenerator ?? (openCommandGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.CommandGenerator); })); }

        private ICommand openAchievementGenerator;
        public ICommand OpenAchievementGenerator { get => openAchievementGenerator ?? (openAchievementGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.AchievementGenerator); })); }

        private ICommand openRecipeGenerator;
        public ICommand OpenRecipeGenerator { get => openRecipeGenerator ?? (openRecipeGenerator = new RelayCommand(() => { navigationService.NavigateTo(App.Pages.RecipeGenerator); })); }
    }
}
