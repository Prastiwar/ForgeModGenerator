using ForgeModGenerator.Models;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Recipe : ObservableModel
    {
        public Recipe()
        {
            Group = "";
            Result = new RecipeResult();
        }

        public virtual string Type { get; } = "";

        private string group;
        public string Group {
            get => group;
            set => SetProperty(ref group, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => SetProperty(ref result, value);
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe recipe)
            {
                Name = recipe.Name;
                Group = recipe.Group;
                Result = recipe.Result;
                IsDirty = false;
                SetValidateProperty(recipe);
                return true;
            }
            return false;
        }
    }
}
