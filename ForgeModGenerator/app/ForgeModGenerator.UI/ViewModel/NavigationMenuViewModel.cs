using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ForgeModGenerator.ViewModel
{
    public class NavigationMenuViewModel : ViewModelBase
    {
        protected Menu menu;
        protected double slideSpeed = 0.25;
        protected Vector offset = new Vector(170, 0);

        private readonly INavigationService navigationService;
        private readonly ScrollViewer menuScroll;
        private readonly DispatcherTimer timer;

        public NavigationMenuViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            //menu = new Menu(MenuGrid, new MenuSettings(MenuGrid, 0, 2, offset, slideSpeed));
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(75) };
        }

        public void InitializeMenu(Grid menuGrid, int row, int column)
        {
            menu = new Menu(menuGrid, new MenuSettings(menuGrid, column, row, offset, slideSpeed));
        }

        private ICommand toggleMenu;
        public ICommand ToggleMenu { get => toggleMenu ?? (toggleMenu = new RelayCommand(() => { menu.Toggle(); })); }

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

        private ICommand holdScrollUp;
        public ICommand HoldScrollUp {
            get {
                if (holdScrollUp == null)
                {
                    holdScrollUp = new RelayCommand(() =>
                    {
                        timer.Tick += ScrollUp;
                        timer.Start();
                    });
                }
                return holdScrollUp;
            }
        }

        private ICommand holdScrollDown;
        public ICommand HoldScrollDown {
            get {
                if (holdScrollDown == null)
                {
                    holdScrollDown = new RelayCommand(() =>
                    {
                        timer.Tick += ScrollDown;
                        timer.Start();
                    });
                }
                return holdScrollDown;
            }
        }

        private ICommand stopScroll;
        public ICommand StopScroll {
            get {
                if (stopScroll == null)
                {
                    stopScroll = new RelayCommand(() =>
                    {
                        timer.Stop();
                        timer.Tick -= ScrollUp;
                        timer.Tick -= ScrollDown;
                    });
                }
                return stopScroll;
            }
        }

        private void ScrollUp(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                timer.Stop();
                timer.Tick -= ScrollUp;
            }
            menuScroll.LineUp();
        }

        private void ScrollDown(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                timer.Stop();
                timer.Tick -= ScrollDown;
            }
            menuScroll.LineDown();
        }
    }
}
