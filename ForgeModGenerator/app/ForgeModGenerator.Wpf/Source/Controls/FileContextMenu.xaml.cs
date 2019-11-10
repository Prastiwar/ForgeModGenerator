using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.Controls
{
    /// <summary>
    /// Interaction logic for ItemContextMenu.xaml
    /// </summary>
    public partial class FileContextMenu : ContextMenu
    {
        static FileContextMenu() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FileContextMenu), new FrameworkPropertyMetadata(typeof(FileContextMenu)));

        public FileContextMenu() => InitializeComponent();
    }
}
