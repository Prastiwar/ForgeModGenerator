using ForgeModGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
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

        public static readonly DependencyProperty IngredientsLengthLimitProperty =
            DependencyProperty.Register("IngredientsLengthLimit", typeof(int), typeof(RecipeEditForm), new PropertyMetadata(int.MaxValue));
        public int IngredientsLengthLimit {
            get => (int)GetValue(IngredientsLengthLimitProperty);
            set => SetValue(IngredientsLengthLimitProperty, value);
        }

        private readonly List<Ingredient> cachedIngredients = new List<Ingredient>();

        public void SetDataContext(object context) => DataContext = context;

        private void RecipeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type newType = (Type)e.AddedItems[0];
            bool shouldCollapseSmelting = newType != typeof(SmeltingRecipe);
            SetSmeltingVisibility(shouldCollapseSmelting);

            bool shouldCollapseShaped = newType != typeof(ShapedRecipe);
            SetShapedVisibility(shouldCollapseShaped);

            bool shouldCollapseShapeless = newType != typeof(ShapelessRecipe);

            IngredientsTextBlock.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;
            IngredientsListBox.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;

            int lastIngredientsLengthLimit = IngredientsLengthLimit;
            IngredientsLengthLimit = !shouldCollapseSmelting ? int.MaxValue : 9;
            if (lastIngredientsLengthLimit != IngredientsLengthLimit)
            {
                if (IngredientsListBox.ItemsSource == null)
                {
                    return;
                }
                HandleIngredientLimitChanged(shouldCollapseSmelting);
            }
        }

        private void HandleIngredientLimitChanged(bool isSmeltingCollapsed)
        {
            if (isSmeltingCollapsed)
            {
                if (IngredientsListBox.ItemsSource.Count > IngredientsLengthLimit)
                {
                    // Fit ItemsSource to length limit. Cache removed items
                    for (int i = IngredientsListBox.ItemsSource.Count - 1; i >= IngredientsLengthLimit; i--)
                    {
                        cachedIngredients.Add(IngredientsListBox.ItemsSource[i]);
                        IngredientsListBox.ItemsSource.RemoveAt(i);
                    }
                }
            }
            else
            {
                // Add cached ingredients back to ItemsSource
                for (int i = cachedIngredients.Count - 1; i >= 0; i--)
                {
                    IngredientsListBox.ItemsSource.Add(cachedIngredients[i]);
                    cachedIngredients.RemoveAt(i);
                }
            }
        }

        private void SetShapedVisibility(bool collapse)
        {
            Visibility visibility = collapse ? Visibility.Collapsed : Visibility.Visible;
            PatternTextBlock.Visibility = visibility;
            PatternGrid.Visibility = visibility;
        }

        private void SetSmeltingVisibility(bool collapse)
        {
            Visibility visibility = collapse ? Visibility.Collapsed : Visibility.Visible;
            ExperienceTextBlock.Visibility = visibility;
            ExperienceNumeric.Visibility = visibility;
            CookingTimeTextBlock.Visibility = visibility;
            CookingTimeNumeric.Visibility = visibility;
        }

        private void FirstComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            RecipeKey selectedItem = (RecipeKey)e.AddedItems[0];
            RecipeCreator recipe = (RecipeCreator)box.DataContext;
            int patternIndex = (int)char.GetNumericValue((string)box.Tag, 0);
            int charIndex = (int)char.GetNumericValue((string)box.Tag, 1);
            char[] chars = recipe.Pattern[patternIndex].ToCharArray();
            chars[charIndex] = selectedItem.Key;
            recipe.Pattern[patternIndex] = new string(chars);
        }

        private async void SlotResult_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ItemListForm form = new ItemListForm();
            bool changed = await StaticCommands.ShowMCItemList(button, "RecipeHost", form).ConfigureAwait(true);
            if (changed)
            {
                RecipeCreator recipe = (RecipeCreator)button.DataContext;
                recipe.Result.Item = form.SelectedLocator.Name;
            }
        }

        private async void Slot_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ItemListForm form = new ItemListForm();
            bool changed = await StaticCommands.ShowMCItemList(button, "RecipeHost", form).ConfigureAwait(true);
            if (changed)
            {
                UpdatePattern((RecipeCreator)button.DataContext, (string)button.Tag, form.SelectedLocator.Name);
            }
        }

        private void UpdatePattern(RecipeCreator recipe, string tag, string locatorName)
        {
            int patternIndex = (int)char.GetNumericValue(tag, 0);
            int charIndex = (int)char.GetNumericValue(tag, 1);
            char[] chars = recipe.Pattern[patternIndex].ToCharArray();
            chars[charIndex] = FindOrCreateKey(recipe, locatorName);
            recipe.Pattern[patternIndex] = new string(chars);
        }

        private char FindOrCreateKey(RecipeCreator recipe, string name)
        {
            foreach (RecipeKey key in recipe.Keys)
            {
                if (key.Item == name)
                {
                    return key.Key;
                }
            }
            return recipe.Keys.AddNew(name).Key;
        }
    }
}
