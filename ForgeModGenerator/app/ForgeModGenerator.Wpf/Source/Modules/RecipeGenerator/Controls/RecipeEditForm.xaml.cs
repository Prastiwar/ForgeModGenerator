using ForgeModGenerator.RecipeGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.RecipeGenerator.Controls
{
    public partial class RecipeEditForm : UserControl, IUIElement
    {
        public RecipeEditForm()
        {
            InitializeComponent();
            RecipeTypeComboBox.SelectionChanged += RecipeTypeComboBox_SelectionChanged;
        }

        public static readonly DependencyProperty RecipeTypesProperty =
            DependencyProperty.Register("RecipeTypes", typeof(IEnumerable<Type>), typeof(RecipeEditForm), new PropertyMetadata(Enumerable.Empty<Type>()));
        public IEnumerable<Type> RecipeTypes {
            get => (IEnumerable<Type>)GetValue(RecipeTypesProperty);
            set => SetValue(RecipeTypesProperty, value);
        }

        public static readonly DependencyProperty SelectTypeChangedProperty =
            DependencyProperty.Register("SelectTypeChanged", typeof(ICommand), typeof(RecipeEditForm), new PropertyMetadata(null));
        public ICommand SelectTypeChanged {
            get => (ICommand)GetValue(SelectTypeChangedProperty);
            set => SetValue(SelectTypeChangedProperty, value);
        }

        public void SetDataContext(object context) => DataContext = context;

        private void RecipeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type newType = (Type)e.AddedItems[0];
            bool shouldCollapseSmelting = newType != typeof(SmeltingRecipe);
            Visibility smeltingVisibility = shouldCollapseSmelting ? Visibility.Collapsed : Visibility.Visible;
            ExperienceTextBlock.Visibility = smeltingVisibility;
            ExperienceNumeric.Visibility = smeltingVisibility;
            CookingTimeTextBlock.Visibility = smeltingVisibility;
            CookingTimeNumeric.Visibility = smeltingVisibility;

            bool shouldCollapseShaped = newType != typeof(ShapedRecipe);
            Visibility shapedVisibility = shouldCollapseShaped ? Visibility.Collapsed : Visibility.Visible;
            KeysTextBlock.Visibility = shapedVisibility;
            KeysListBox.Visibility = shapedVisibility;
            PatternTextBlock.Visibility = shapedVisibility;
            PatternGrid.Visibility = shapedVisibility;

            bool shouldCollapseShapeless = newType != typeof(ShapelessRecipe);

            IngredientsTextBlock.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;
            IngredientsListBox.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
