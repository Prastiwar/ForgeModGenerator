using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class SmeltingRecipe : Recipe
    {
        protected SmeltingRecipe() { }
        public SmeltingRecipe(string filePath) : base(filePath) { }

        private ObservableCollection<Ingredient> ingredient;
        public ObservableCollection<Ingredient> Ingredient {
            get => ingredient;
            set => SetProperty(ref ingredient, value);
        }

        private string result;
        public string Result {
            get => result;
            set => SetProperty(ref result, value);
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
                Ingredient = Ingredient.DeepCollectionClone<ObservableCollection<Ingredient>, Ingredient>(),
                Result = Result,
                CookingTime = CookingTime,
                Experience = Experience
            };
            recipe.SetInfo(Info.FullName);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is SmeltingRecipe recipe)
            {
                Ingredient = recipe.Ingredient;
                Result = recipe.Result;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
