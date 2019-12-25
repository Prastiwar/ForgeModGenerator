using ForgeModGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForgeModGenerator.RecipeGenerator.Controls
{
    public class EditableRecipeKeyList : EditableList<RecipeKey>
    {
        public static readonly DependencyProperty LengthLimitProperty =
            DependencyProperty.Register("LengthLimit", typeof(int), typeof(EditableRecipeKeyList), new PropertyMetadata(int.MaxValue));
        public int LengthLimit {
            get => (int)GetValue(LengthLimitProperty);
            set => SetValue(LengthLimitProperty, value);
        }

        protected override RecipeKey DefaultItem {
            get {
                char key = (char)('a' + counter);
                counter++;
                return new RecipeKey(key, "");
            }
        }
        private int counter;

        protected override void DefaultAdd(ObservableCollection<RecipeKey> collection)
        {
            if (collection.Count < LengthLimit)
            {
                base.DefaultAdd(collection);
            }
        }
    }

    public class EditableIngredientList : EditableList<Ingredient>
    {
        public static readonly DependencyProperty LengthLimitProperty =
            DependencyProperty.Register("LengthLimit", typeof(int), typeof(EditableIngredientList), new PropertyMetadata(int.MaxValue));
        public int LengthLimit {
            get => (int)GetValue(LengthLimitProperty);
            set => SetValue(LengthLimitProperty, value);
        }

        private ICommand itemClickCommand;
        public ICommand ItemClickCommand => itemClickCommand ?? (itemClickCommand = new DelegateCommand<Tuple<ContentControl, string, ItemListForm>>(OnItemClick));

        private async void OnItemClick(Tuple<ContentControl, string, ItemListForm> parameter)
        {
            ItemListForm form = parameter.Item3;
            if (form == null)
            {
                form = new ItemListForm();
            }
            bool changed = await StaticCommands.ShowMCItemList(parameter.Item1, parameter.Item2, form).ConfigureAwait(true);
            if (changed)
            {
                if (parameter.Item1.DataContext is Ingredient ingredient)
                {
                    ingredient.Item = form.SelectedLocator.Name;
                }
            }
        }

        protected override Ingredient DefaultItem => new Ingredient();

        protected override void DefaultAdd(ObservableCollection<Ingredient> collection)
        {
            if (collection.Count < LengthLimit)
            {
                base.DefaultAdd(collection);
            }
        }
    }
}
