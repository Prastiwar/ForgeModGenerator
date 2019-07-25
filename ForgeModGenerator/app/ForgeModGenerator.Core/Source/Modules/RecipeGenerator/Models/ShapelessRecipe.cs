using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapelessRecipe : Recipe
    {
        protected ShapelessRecipe() { }
        public ShapelessRecipe(string filePath) : base(filePath)
        {
            Ingredients = new ObservableCollection<Ingredient>();
            Result = new RecipeResult();
        }

        public override string Type => "crafting_shapeless";

        private ObservableCollection<Ingredient> ingredients;
        public ObservableCollection<Ingredient> Ingredients {
            get => ingredients;
            set => SetProperty(ref ingredients, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => SetProperty(ref result, value);
        }

        public override object DeepClone()
        {
            ShapelessRecipe recipe = new ShapelessRecipe() {
                Name = Name,
                Group = Group,
                Ingredients =  Ingredients.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>(),
                Result = new RecipeResult {
                    Count = Result.Count,
                    Item = Result.Item
                }
            };
            recipe.SetInfo(Info.FullName);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ShapelessRecipe recipe)
            {
                Ingredients = recipe.Ingredients;
                Result = recipe.Result;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
