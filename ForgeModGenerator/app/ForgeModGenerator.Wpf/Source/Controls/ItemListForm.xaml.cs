using ForgeModGenerator.Core;
using System.Linq;
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

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(ItemListForm), new PropertyMetadata("", OnFilterChanged));
        public string FilterText {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string filter = e.NewValue as string;
            ItemListForm form = (ItemListForm)d;
            form.SetFilter(filter);
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

        private MCItemLocator[] notFilteredLocators;
        private bool isFiltered = false;

        public void SetFilter(string filter)
        {
            filter = filter.Trim();
            if (string.IsNullOrEmpty(filter))
            {
                Locators = notFilteredLocators;
                isFiltered = false;
            }
            else
            {
                if (!isFiltered)
                {
                    notFilteredLocators = Locators;
                }
                Locators = notFilteredLocators.Where(locator => locator.Name.Contains(filter)).ToArray();
                isFiltered = true;
            }
        }

        public void SetDataContext(object context) => DataContext = context;
    }
}
