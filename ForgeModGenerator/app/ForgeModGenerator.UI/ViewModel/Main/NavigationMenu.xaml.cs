using ForgeModGenerator.Components;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> NavigationMenu UI View-ViewModel </summary>
    public partial class NavigationMenu : UserControl
    {
        protected MenuComponent menuComponent;
        protected double slideSpeed = 0.25;
        protected Vector offset = new Vector(170, 0);
        private readonly DispatcherTimer timer;

        public NavigationMenu()
        {
            InitializeComponent();
            menuComponent = new MenuComponent(MenuGrid, new MenuSettings(MenuGrid, 0, 2, offset, slideSpeed));
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(45) };
        }

        public void InitializeMenu(Grid menuGrid, int row, int column)
        {
            menuComponent = new MenuComponent(menuGrid, new MenuSettings(menuGrid, column, row, offset, slideSpeed));
        }

        private ICommand toggleMenu;
        public ICommand ToggleMenu { get => toggleMenu ?? (toggleMenu = new RelayCommand(() => { menuComponent.Toggle(); })); }

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
            MenuScroll.LineUp();
        }

        private void ScrollDown(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                timer.Stop();
                timer.Tick -= ScrollDown;
            }
            MenuScroll.LineDown();
        }
    }
}
