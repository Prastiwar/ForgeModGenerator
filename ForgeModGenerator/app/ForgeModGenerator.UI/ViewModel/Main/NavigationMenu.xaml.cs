using ForgeModGenerator.Components;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> NavigationMenu UI View-ViewModel </summary>
    public partial class NavigationMenu : UserControl
    {
        protected MenuComponent menuComponent;
        protected double slideSpeed = 0.25;
        protected Vector offset = new Vector(170, 0);

        public NavigationMenu()
        {
            InitializeComponent();
            menuComponent = new MenuComponent(MenuGrid, new MenuSettings(MenuGrid, 0, 2, offset, slideSpeed));
        }

        private ICommand toggleMenu;
        public ICommand ToggleMenu { get => toggleMenu ?? (toggleMenu = new RelayCommand(() => { menuComponent.Toggle(); })); }

        public void InitializeMenu(Grid menuGrid, int row, int column)
        {
            menuComponent = new MenuComponent(menuGrid, new MenuSettings(menuGrid, column, row, offset, slideSpeed));
        }
    }
}
