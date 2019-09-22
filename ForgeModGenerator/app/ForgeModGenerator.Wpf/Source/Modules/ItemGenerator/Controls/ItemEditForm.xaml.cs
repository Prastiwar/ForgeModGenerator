using ForgeModGenerator.ItemGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<ItemType> ItemTypes => Enum.GetValues(typeof(ItemType)).Cast<ItemType>();
        public IEnumerable<ArmorType> ArmorTypes => Enum.GetValues(typeof(ArmorType)).Cast<ArmorType>();

        public void SetDataContext(object context) => DataContext = context;

        private void ItemTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemType newType = (ItemType)e.AddedItems[0];
            bool shouldCollapseArmor = newType != ItemType.Armor;
            ArmorTypeComboBox.Visibility = shouldCollapseArmor ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
