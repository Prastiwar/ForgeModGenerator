using ForgeModGenerator.ItemGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ForgeModGenerator.ItemGenerator.Controls
{
    public partial class ItemEditForm : UserControl, IUIElement
    {
        public ItemEditForm() => InitializeComponent();

        public IEnumerable<ItemType> ItemTypes => Enum.GetValues(typeof(ItemType)).Cast<ItemType>();
        public IEnumerable<ArmorType> ArmorTypes => Enum.GetValues(typeof(ArmorType)).Cast<ArmorType>();

        public void SetDataContext(object context) => DataContext = context;
    }
}
