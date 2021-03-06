﻿using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.BlockGenerator.Controls
{
    public partial class BlockEditForm : UserControl, IUIElement
    {
        public BlockEditForm()
        {
            InitializeComponent();
            DataContextChanged += BlockEditForm_DataContextChanged;
        }

        private void BlockEditForm_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is Block block)
            {
                MCItemLocator[] locators = MCItemLocator.GetAllMinecraftItems();
                MCItemLocator inventoryTextureLocator = locators.FirstOrDefault(x => x.Name == block.InventoryTextureName);
                MCItemLocator textureLocator = locators.FirstOrDefault(x => x.Name == block.TextureName);
                StaticCommands.SetItemButtonImage(InventoryTextureButton, inventoryTextureLocator);
                StaticCommands.SetItemButtonImage(TextureButton, textureLocator);
            }
        }

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

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlockType newType = (BlockType)e.AddedItems[0];
            bool shouldShowDropItem = newType.HasFlag(BlockType.Ore);
            Visibility dropItemVisibility = shouldShowDropItem ? Visibility.Visible : Visibility.Collapsed;
            DropItemComboBox.Visibility = dropItemVisibility;
            DropItemTextBlock.Visibility = dropItemVisibility;
        }

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
