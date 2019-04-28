using ForgeModGenerator.Validation;
using System.ComponentModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Recipe : FileObject, IDataErrorInfo, IValidable<Recipe>
    {
        protected Recipe() { }
        public Recipe(string filePath) : base(filePath) { }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string group;
        public string Group {
            get => group;
            set => SetProperty(ref group, value);
        }

        public override object DeepClone()
        {
            Recipe recipe = new Recipe() {
                Name = Name,
                Group = Group
            };
            recipe.SetInfo(Info.FullName);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe recipe)
            {
                Name = recipe.Name;
                Group = recipe.Group;

                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }

        public virtual ValidateResult Validate() => ValidateResult.Valid;

        public event PropertyValidationEventHandler<Recipe> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
