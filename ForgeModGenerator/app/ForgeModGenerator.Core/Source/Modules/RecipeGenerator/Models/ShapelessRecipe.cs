using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapelessRecipe : Recipe
    {
        public ShapelessRecipe() : base() => Ingredients = new ObservableCollection<Ingredient>();

        public override string Type => "crafting_shapeless";

        private ObservableCollection<Ingredient> ingredients;
        public ObservableCollection<Ingredient> Ingredients {
            get => ingredients;
            set => SetProperty(ref ingredients, value);
        }

        public override object DeepClone()
        {
            ShapelessRecipe recipe = (ShapelessRecipe)base.DeepClone();
            recipe.Ingredients = Ingredients?.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>();
            recipe.Result = new RecipeResult {
                Count = Result.Count,
                Item = Result.Item
            };
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ShapelessRecipe recipe)
            {
                Ingredients = recipe.Ingredients;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
