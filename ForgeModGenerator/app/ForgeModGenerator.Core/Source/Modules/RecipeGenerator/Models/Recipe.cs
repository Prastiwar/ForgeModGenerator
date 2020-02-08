using ForgeModGenerator.Models;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Recipe : ObservableModel
    {
        public virtual string Type { get; } = "";
        
        private string group;
        public string Group {
            get => group;
            set => SetProperty(ref group, value);
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe recipe)
            {
                Name = recipe.Name;
                Group = recipe.Group;
                IsDirty = false;
                return true;
            }
            return false;
        }
    }
}
