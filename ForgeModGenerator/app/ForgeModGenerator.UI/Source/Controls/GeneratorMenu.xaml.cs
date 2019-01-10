using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ForgeModGenerator.UI.Controls
{
    public partial class GeneratorMenu : UserControl
    {
        protected Menu menu;
        protected double slideSpeed = 0.25;
        protected Vector offset = new Vector(170, 0);

        private DispatcherTimer timer;

        public GeneratorMenu()
        {
            InitializeComponent();
            menu = new Menu(MenuGrid, new MenuSettings(MenuGrid, 0, 2, offset, slideSpeed));
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(75) };
        }

        public void InitializeMenu(Grid menuGrid, int row, int column)
        {
            menu = new Menu(menuGrid, new MenuSettings(menuGrid, column, row, offset, slideSpeed));
        }

        private void ToggleLeftMenu(object sender, RoutedEventArgs e)
        {
            menu.Toggle();
        }

        private void HoldScrollUp(object sender, RoutedEventArgs e)
        {
            timer.Tick += ScrollUp;
            timer.Start();
        }

        private void HoldScrollDown(object sender, RoutedEventArgs e)
        {
            timer.Tick += ScrollDown;
            timer.Start();
        }

        private void ScrollUp(object sender, EventArgs e)
        {
            MenuScroll.LineUp();
        }

        private void ScrollDown(object sender, EventArgs e)
        {
            MenuScroll.LineDown();
        }

        private void StopScroll(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Tick -= ScrollUp;
            timer.Tick -= ScrollDown;
        }

        private void OpenDashboard(object sender, RoutedEventArgs e)
        {
        }
        private void OpenModGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenBlockGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenItemGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenSoundGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenCommandGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenAchievementGenerator(object sender, RoutedEventArgs e)
        {
        }
        private void OpenRecipeGenerator(object sender, RoutedEventArgs e)
        {
        }
    }
}
