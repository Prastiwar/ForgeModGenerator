using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    /// <summary>
    /// Interaction logic for ItemContextMenu.xaml
    /// </summary>
    public partial class ModelContextMenu : ContextMenu
    {
        static ModelContextMenu() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ModelContextMenu), new FrameworkPropertyMetadata(typeof(ModelContextMenu)));

        public ModelContextMenu() => InitializeComponent();
    }
}
