using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.MaterialGenerator.Models;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.MaterialGenerator.Controls
{
    public partial class MaterialEditForm : UserControl, IUIElement
    {
        public MaterialEditForm()
        {
            InitializeComponent();
            TypeComboBox.SelectionChanged += TypeComboBox_SelectionChanged;
        }

        public IEnumerable<PushReaction> PushReactions => Enum.GetValues(typeof(PushReaction)).Cast<PushReaction>();

        public static readonly DependencyProperty SoundEventsProperty =
            DependencyProperty.Register("SoundEvents", typeof(IEnumerable<string>), typeof(MaterialEditForm), new PropertyMetadata(Enumerable.Empty<string>()));
        public IEnumerable<string> SoundEvents {
            get => (IEnumerable<string>)GetValue(SoundEventsProperty);
            set => SetValue(SoundEventsProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;

        private async void TextureButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ItemListForm form = new ItemListForm();
            bool changed = await StaticCommands.ShowMCItemList(button, (string)DialogHost.Identifier, form).ConfigureAwait(true);
            if (changed)
            {
                MCItemLocator locator = form.SelectedLocator;
                if (DataContext is ArmorMaterial armorMaterial)
                {
                    armorMaterial.TextureName = locator.Name;
                }
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type newType = (Type)e.AddedItems[0];

            bool shouldCollapseArmor = newType != typeof(ArmorMaterial);
            SetArmorVisibility(shouldCollapseArmor);

            bool shouldCollapseBlock = newType != typeof(BlockMaterial);
            SetBlockVisibility(shouldCollapseBlock);

            bool shouldCollapseTool = newType != typeof(ToolMaterial);
            SetToolVisibility(shouldCollapseTool);

            bool shouldCollapseItem = newType != typeof(ItemMaterial);
            EfficiencyNumeric.Visibility = shouldCollapseItem && shouldCollapseTool ? Visibility.Collapsed : Visibility.Visible;
            EfficiencyTextBlock.Visibility = shouldCollapseItem && shouldCollapseTool ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetArmorVisibility(bool collapse)
        {
            Visibility visibility = collapse ? Visibility.Collapsed : Visibility.Visible;
            DurabilityNumeric.Visibility = visibility;
            DurabilityTextBlock.Visibility = visibility;

            HelmetDamageReductionNumeric.Visibility = visibility;
            HelmetDamageReductionTextBlock.Visibility = visibility;

            PlateDamageReductionNumeric.Visibility = visibility;
            PlateDamageReductionTextBlock.Visibility = visibility;

            LegsDamageReductionNumeric.Visibility = visibility;
            LegsDamageReductionTextBlock.Visibility = visibility;

            BootsDamageReductionNumeric.Visibility = visibility;
            BootsDamageReductionTextBlock.Visibility = visibility;

            ToughnessNumeric.Visibility = visibility;
            ToughnessTextBlock.Visibility = visibility;

            TextureButton.Visibility = visibility;
            TextureNameTextBlock.Visibility = visibility;

        }

        private void SetBlockVisibility(bool collapse)
        {
            Visibility visibility = collapse ? Visibility.Collapsed : Visibility.Visible;

            IsLiquidCheckBox.Visibility = visibility;
            IsLiquidTextBlock.Visibility = visibility;

            IsSolidCheckBox.Visibility = visibility;
            IsSolidTextBlock.Visibility = visibility;

            BlocksLightCheckBox.Visibility = visibility;
            BlocksLightTextBlock.Visibility = visibility;

            IsTranslucentCheckBox.Visibility = visibility;
            IsTranslucentTextBlock.Visibility = visibility;

            RequiresNoToolCheckBox.Visibility = visibility;
            RequiresNoToolTextBlock.Visibility = visibility;

            CanBurnCheckBox.Visibility = visibility;
            CanBurnTextBlock.Visibility = visibility;

            IsReplaceableCheckBox.Visibility = visibility;
            IsReplaceableTextBlock.Visibility = visibility;

            IsAdventureModeExemptCheckBox.Visibility = visibility;
            IsAdventureModeExemptTextBlock.Visibility = visibility;

            MobilityFlagComboBox.Visibility = visibility;
            MobilityFlagTextBlock.Visibility = visibility;
        }

        private void SetToolVisibility(bool collapse)
        {
            Visibility visibility = collapse ? Visibility.Collapsed : Visibility.Visible;

            AttackDamageNumeric.Visibility = visibility;
            AttackDamageTextBlock.Visibility = visibility;

            MaxUsesNumeric.Visibility = visibility;
            MaxUsesTextBlock.Visibility = visibility;

            HarvestLevelNumeric.Visibility = visibility;
            HarvestLevelTextBlock.Visibility = visibility;
        }
    }
}
