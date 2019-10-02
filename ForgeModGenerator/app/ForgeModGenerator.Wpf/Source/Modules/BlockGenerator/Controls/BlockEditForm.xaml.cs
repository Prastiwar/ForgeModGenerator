using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.BlockGenerator.Controls
{
    public partial class BlockEditForm : UserControl, IUIElement
    {
        public BlockEditForm() => InitializeComponent();

        public IEnumerable<BlockType> BlockTypes => Enum.GetValues(typeof(BlockType)).Cast<BlockType>();
        public IEnumerable<HarvestToolType> HarvestToolTypes => Enum.GetValues(typeof(HarvestToolType)).Cast<HarvestToolType>();

        public static readonly DependencyProperty SoundTypesProperty =
            DependencyProperty.Register("SoundTypes", typeof(IEnumerable<string>), typeof(BlockEditForm), new PropertyMetadata(Enumerable.Empty<string>()));
        public IEnumerable<string> SoundTypes {
            get => (IEnumerable<string>)GetValue(SoundTypesProperty);
            set => SetValue(SoundTypesProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;

        private async Task<MCItemLocator> SetTexture(Button button)
        {
            button.Command.Execute(button.CommandParameter);
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
