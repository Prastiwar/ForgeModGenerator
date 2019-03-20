using ForgeModGenerator.Components;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.ApplicationModule.Views
{
    /// <summary> NavigationMenu UI View-ViewModel </summary>
    public partial class NavigationMenu : UserControl
    {
        public NavigationMenu()
        {
            InitializeComponent();
            MenuComponent = new MenuComponent(MenuGrid, new MenuSettings(MenuGrid, 0, 2, offset, slideSpeed));
        }

        protected double slideSpeed = 0.25;
        protected Vector offset = new Vector(170, 0);

        public MenuComponent MenuComponent { get; set; }

        private ICommand toggleMenu;
        public ICommand ToggleMenu => toggleMenu ?? (toggleMenu = new DelegateCommand(() => { MenuComponent.Toggle(); }));

        public void InitializeMenu(Grid menuGrid, int row, int column) => MenuComponent = new MenuComponent(menuGrid, new MenuSettings(menuGrid, column, row, offset, slideSpeed));
    }
}
