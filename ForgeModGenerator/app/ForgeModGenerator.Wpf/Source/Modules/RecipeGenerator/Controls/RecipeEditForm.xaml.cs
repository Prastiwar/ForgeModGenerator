using ForgeModGenerator.Controls;
using ForgeModGenerator.Core;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForgeModGenerator.RecipeGenerator.Controls
{
    public partial class RecipeEditForm : UserControl, IUIElement
    {
        public RecipeEditForm() => InitializeComponent();

        public static readonly DependencyProperty RecipeTypesProperty =
            DependencyProperty.Register("RecipeTypes", typeof(IEnumerable<Type>), typeof(RecipeEditForm), new PropertyMetadata(Enumerable.Empty<Type>()));
        public IEnumerable<Type> RecipeTypes {
            get => (IEnumerable<Type>)GetValue(RecipeTypesProperty);
            set => SetValue(RecipeTypesProperty, value);
        }

        public static readonly DependencyProperty IngredientsLengthLimitProperty =
            DependencyProperty.Register("IngredientsLengthLimit", typeof(int), typeof(RecipeEditForm), new PropertyMetadata(int.MaxValue));
        public int IngredientsLengthLimit {
            get => (int)GetValue(IngredientsLengthLimitProperty);
            set => SetValue(IngredientsLengthLimitProperty, value);
        }

        private readonly List<Ingredient> cachedIngredients = new List<Ingredient>();

        public void SetDataContext(object context)
        {
            DataContext = context;
            UpdateRecipeLayout(DataContext.GetType());
            UpdateSlots();
        }

        private void UpdateRecipeLayout(Type type)
        {
            bool shouldCollapseSmelting = type != typeof(SmeltingRecipe);
            SetSmeltingVisibility(shouldCollapseSmelting);

            bool shouldCollapseShaped = type != typeof(ShapedRecipe);
            SetShapedVisibility(shouldCollapseShaped);

            bool shouldCollapseShapeless = type != typeof(ShapelessRecipe);
            IngredientsTextBlock.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;
            EditableIngredientListBox.Visibility = shouldCollapseSmelting && shouldCollapseShapeless ? Visibility.Collapsed : Visibility.Visible;

            int lastIngredientsLengthLimit = IngredientsLengthLimit;
            IngredientsLengthLimit = !shouldCollapseSmelting ? int.MaxValue : 9;
            if (lastIngredientsLengthLimit != IngredientsLengthLimit)
            {
                if (EditableIngredientListBox.ItemsSource == null)
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
                if (EditableIngredientListBox.ItemsSource.Count > IngredientsLengthLimit)
                {
                    // Fit ItemsSource to length limit. Cache removed items
                    for (int i = EditableIngredientListBox.ItemsSource.Count - 1; i >= IngredientsLengthLimit; i--)
                    {
                        cachedIngredients.Add(EditableIngredientListBox.ItemsSource[i]);
                        EditableIngredientListBox.ItemsSource.RemoveAt(i);
                    }
                }
            }
            else
            {
                // Add cached ingredients back to ItemsSource
                for (int i = cachedIngredients.Count - 1; i >= 0; i--)
                {
                    EditableIngredientListBox.ItemsSource.Add(cachedIngredients[i]);
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

        private async void ResultSlotClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is Recipe recipe)
            {
                ItemListForm form = new ItemListForm();
                bool changed = await StaticCommands.ShowMCItemList(button, (string)DialogHost.Identifier, form).ConfigureAwait(true);
                if (changed)
                {
                    recipe.Result.Item = form.SelectedLocator.Name;
                }
            }
        }

        private async void ShapedSlotClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is ShapedRecipe recipe)
            {
                ItemListForm form = new ItemListForm();
                bool changed = await StaticCommands.ShowMCItemList(button, (string)DialogHost.Identifier, form).ConfigureAwait(true);
                if (changed)
                {
                    UpdatePattern(recipe, (string)button.Tag, form.SelectedLocator.Name);
                }
            }
        }

        private void UpdatePattern(ShapedRecipe recipe, string tag, string locatorName)
        {
            int patternIndex = (int)char.GetNumericValue(tag, 0);
            int charIndex = (int)char.GetNumericValue(tag, 1);
            char nextChar = FindOrCreateKey(recipe, locatorName);
            recipe.Pattern.Set(patternIndex, charIndex, nextChar);
        }

        private char FindOrCreateKey(Recipe recipe, string name)
        {
            dynamic recipeDynamic = recipe;
            foreach (RecipeKey key in recipeDynamic.Keys)
            {
                if (key.Item == name)
                {
                    return key.Key;
                }
            }
            return recipeDynamic.Keys.AddNew(name).Key;
        }

        private ListBox GetListBoxFromIngredientList(EditableIngredientList list)
        {
            FrameworkElement content = list.Template.LoadContent() as FrameworkElement;
            ListBox listBox = content.FindName("IngredientsListBox") as ListBox;
            return listBox;
        }

        private void RefreshIngredientsList(EditableIngredientList list)
        {
            ListBox listBox = GetListBoxFromIngredientList(list);
            if (listBox != null)
            {
                ObservableCollection<Ingredient> ingredients = list.ItemsSource;
                // TODO: Get all possible locators not only defaults
                MCItemLocator[] locators = MCItemLocator.GetAllMinecraftItems();
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    DependencyObject container = listBox.ItemContainerGenerator.ContainerFromIndex(i);
                    ListBoxItem item = container as ListBoxItem;
                    ContentControl button = WPFHelper.GetDescendantFromName(item, "ItemButton") as ContentControl;
                    string itemName = ingredients[i].Item;
                    MCItemLocator locator = locators.FirstOrDefault(x => string.Compare(x.Name, itemName, true) == 0);
                    StaticCommands.SetItemButtonImage(button, locator);
                    i++;
                }
            }
        }

        private void UpdateSlots()
        {
            if (DataContext is Recipe recipe)
            {
                bool shouldUpdateResult = !string.IsNullOrEmpty(recipe.Result.Item);
                if (shouldUpdateResult)
                {
                    // TODO: Get all possible locators not only defaults
                    MCItemLocator[] locators = MCItemLocator.GetAllMinecraftItems();
                    MCItemLocator locator = locators.FirstOrDefault(x => string.Compare(x.Name, recipe.Result.Item, true) == 0);
                    StaticCommands.SetItemButtonImage(ResultItemButton, locator);
                }
            }
            if (DataContext is ShapedRecipe shaped)
            {
                if (shaped.Pattern.IsEmpty)
                {
                    // TODO: Get all possible locators not only defaults
                    MCItemLocator[] locators = MCItemLocator.GetAllMinecraftItems();
                    SetItemButtonImage(FirstSlot, shaped.Pattern.GetKey(0, 0));
                    SetItemButtonImage(SecondSlot, shaped.Pattern.GetKey(0, 1));
                    SetItemButtonImage(ThirdSlot, shaped.Pattern.GetKey(0, 2));
                    SetItemButtonImage(FourthSlot, shaped.Pattern.GetKey(1, 0));
                    SetItemButtonImage(FifthSlot, shaped.Pattern.GetKey(1, 1));
                    SetItemButtonImage(SixthSlot, shaped.Pattern.GetKey(1, 2));
                    SetItemButtonImage(SeventhSlot, shaped.Pattern.GetKey(2, 0));
                    SetItemButtonImage(EightSlot, shaped.Pattern.GetKey(2, 1));
                    SetItemButtonImage(NinethSlot, shaped.Pattern.GetKey(2, 2));
                    void SetItemButtonImage(ContentControl control, char key)
                    {
                        int i = shaped.Keys.FindIndex(x => x.Key == key);
                        if (i > -1)
                        {
                            StaticCommands.SetItemButtonImage(control, locators.FirstOrDefault(x => string.Compare(x.Name, shaped.Keys[i].Item, true) == 0));
                        }
                    }
                }
            }
            else if (DataContext is SmeltingRecipe smelting)
            {
                bool shouldUpdateIngredients = smelting.Ingredients.Any(x => !string.IsNullOrEmpty(x.Item));
                if (shouldUpdateIngredients)
                {
                    RefreshIngredientsList(EditableIngredientListBox);
                }
            }
            else if (DataContext is ShapelessRecipe shapeless)
            {
                bool shouldUpdateIngredients = shapeless.Ingredients.Any(x => !string.IsNullOrEmpty(x.Item));
                if (shouldUpdateIngredients)
                {
                    RefreshIngredientsList(EditableIngredientListBox);
                }
            }
        }
    }
}
