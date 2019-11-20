using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.BlockGenerator.Controls
{
    public partial class BlockEditForm : UserControl, IUIElement
    {
        public BlockEditForm() => InitializeComponent();

        public IEnumerable<BlockType> BlockTypes => ReflectionHelper.GetEnumValues<BlockType>();
        public IEnumerable<HarvestToolType> HarvestToolTypes => ReflectionHelper.GetEnumValues<HarvestToolType>();

        public static readonly DependencyProperty SoundTypesProperty =
            DependencyProperty.Register("SoundTypes", typeof(ChooseCollection), typeof(BlockEditForm), new PropertyMetadata(null));
        public ChooseCollection SoundTypes {
            get => (ChooseCollection)GetValue(SoundTypesProperty);
            set => SetValue(SoundTypesProperty, value);
        }

        public static readonly DependencyProperty DropItemsProperty =
            DependencyProperty.Register("DropItems", typeof(ChooseCollection), typeof(BlockEditForm), new PropertyMetadata(null));
        public ChooseCollection DropItems {
            get => (ChooseCollection)GetValue(DropItemsProperty);
            set => SetValue(DropItemsProperty, value);
        }

        public static readonly DependencyProperty MaterialTypesProperty =
            DependencyProperty.Register("MaterialTypes", typeof(ChooseCollection), typeof(BlockEditForm), new PropertyMetadata(null));
        public ChooseCollection MaterialTypes {
            get => (ChooseCollection)GetValue(MaterialTypesProperty);
            set => SetValue(MaterialTypesProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;

        private async Task<MCItemLocator> SetTexture(Button button)
        {
            ItemListForm form = new ItemListForm();
            bool changed = await StaticCommands.ShowMCItemList(button, (string)DialogHost.Identifier, form).ConfigureAwait(true);
            return changed ? form.SelectedLocator : default;
        }

        private async void TextureButton_Click(object sender, RoutedEventArgs e)
        {
            MCItemLocator locator = await SetTexture((Button)sender).ConfigureAwait(true);
            if (DataContext is Block block && !string.IsNullOrEmpty(locator.Name))
            {
                block.TextureName = locator.Name;
            }
        }

        private async void InventoryTextureButton_Click(object sender, RoutedEventArgs e)
        {
            MCItemLocator locator = await SetTexture((Button)sender).ConfigureAwait(true);
            if (DataContext is Block block && !string.IsNullOrEmpty(locator.Name))
            {
                block.InventoryTextureName = locator.Name;
            }
        }
    }
}
