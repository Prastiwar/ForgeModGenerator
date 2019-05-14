using ForgeModGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using System.Collections.ObjectModel;
using System.Windows;

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
                return new RecipeKey(key, "minecraft:");
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

        protected override Ingredient DefaultItem => new Ingredient("minecraft:", "");

        protected override void DefaultAdd(ObservableCollection<Ingredient> collection)
        {
            if (collection.Count < LengthLimit)
            {
                base.DefaultAdd(collection);
            }
        }
    }
}
