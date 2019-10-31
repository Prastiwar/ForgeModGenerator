using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    /// <summary>
    /// Interaction logic for ItemContextMenu.xaml
    /// </summary>
    public partial class ItemContextMenu : ContextMenu
    {
        static ItemContextMenu() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemContextMenu), new FrameworkPropertyMetadata(typeof(ItemContextMenu)));

        public ItemContextMenu() => InitializeComponent();
    }
}
