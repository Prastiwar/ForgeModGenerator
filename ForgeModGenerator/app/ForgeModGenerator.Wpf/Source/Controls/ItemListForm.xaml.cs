using ForgeModGenerator.Core;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    public partial class ItemListForm : UserControl, IUIElement
    {
        public ItemListForm()
        {
            InitializeComponent();
            if (Locators.Length > 0)
            {
                SelectedLocator = Locators[0];
            }
        }

        public static readonly DependencyProperty LocatorsProperty =
            DependencyProperty.Register("Locators", typeof(MCItemLocator[]), typeof(ItemListForm), new PropertyMetadata(MCItemLocator.GetAllMinecraftItems()));
        public MCItemLocator[] Locators {
            get => (MCItemLocator[])GetValue(LocatorsProperty);
            set => SetValue(LocatorsProperty, value);
        }

        public static readonly DependencyProperty SelectedLocatorProperty =
            DependencyProperty.Register("SelectedLocator", typeof(MCItemLocator), typeof(ItemListForm), new PropertyMetadata(null));
        public MCItemLocator SelectedLocator {
            get => (MCItemLocator)GetValue(SelectedLocatorProperty);
            set => SetValue(SelectedLocatorProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;
    }
}
