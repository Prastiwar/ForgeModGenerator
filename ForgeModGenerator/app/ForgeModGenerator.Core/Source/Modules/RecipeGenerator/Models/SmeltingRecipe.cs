using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class SmeltingRecipe : Recipe
    {
        protected SmeltingRecipe() { }
        public SmeltingRecipe(string filePath) : base(filePath) { }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => SetProperty(ref result, value);
        }

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
            SmeltingRecipe recipe = new SmeltingRecipe() {
                CookingTime = CookingTime,
                Experience = Experience,
                Ingredients = Ingredients.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>(),
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
            if (fromCopy is SmeltingRecipe recipe)
            {
                Ingredients = recipe.Ingredients;
                Result = recipe.Result;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
