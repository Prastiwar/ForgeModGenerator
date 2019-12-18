using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.ItemGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.ItemGenerator.Controls
{
    public partial class ItemEditForm : UserControl, IUIElement
    {
        public ItemEditForm()
        {
            InitializeComponent();
            ItemTypeComboBox.SelectionChanged += ItemTypeComboBox_SelectionChanged;
        }

        public IEnumerable<ItemType> ItemTypes => ReflectionHelper.GetEnumValues<ItemType>();
        public IEnumerable<ArmorType> ArmorTypes => ReflectionHelper.GetEnumValues<ArmorType>();

        public static readonly DependencyProperty MaterialsProperty =
            DependencyProperty.Register("Materials", typeof(ChooseCollection), typeof(ItemEditForm), new PropertyMetadata(Enumerable.Empty<string>()));
        public ChooseCollection Materials {
            get => (ChooseCollection)GetValue(MaterialsProperty);
            set => SetValue(MaterialsProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;
        private void ItemTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemType newType = (ItemType)e.AddedItems[0];
            bool shouldCollapseArmor = newType != ItemType.Armor;
            ArmorTypeComboBox.Visibility = shouldCollapseArmor ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Command.Execute(btn.CommandParameter);
            ItemListForm form = new ItemListForm();
            bool changed = await StaticCommands.ShowMCItemList(btn, (string)DialogHost.Identifier, form).ConfigureAwait(true);
            if (changed)
            {
                MCItemLocator locator = form.SelectedLocator;
                if (DataContext is Item item)
                {
                    item.TextureName = locator.Name;
                }
            }
        }
    }
}
