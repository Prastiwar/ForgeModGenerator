using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class SmeltingRecipe : Recipe
    {
        public SmeltingRecipe() : base() => Ingredients = new ObservableCollection<Ingredient>();

        public override string Type => "smelting";

        private ObservableCollection<Ingredient> ingredients;
        public ObservableCollection<Ingredient> Ingredients {
            get => ingredients;
            set => SetProperty(ref ingredients, value);
        }

        private int cookingTime;
        public int CookingTime {
            get => cookingTime;
            set => SetProperty(ref cookingTime, value);
        }

        private float experience;
        public float Experience {
            get => experience;
            set => SetProperty(ref experience, value);
        }

        public override object DeepClone()
        {
            SmeltingRecipe recipe = (SmeltingRecipe)base.DeepClone();
            CookingTime = recipe.CookingTime;
            Experience = recipe.Experience;
            recipe.Ingredients = Ingredients.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>();
            recipe.Result = new RecipeResult {
                Count = Result.Count,
                Item = Result.Item
            };
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is SmeltingRecipe recipe)
            {
                Ingredients = recipe.Ingredients;
                CookingTime = recipe.CookingTime;
                Experience = recipe.Experience;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
