using ForgeModGenerator.Validation;
using System;
using System.ComponentModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class Recipe : FileObject, IDataErrorInfo, IValidable<Recipe>
    {
        protected Recipe() { }
        public Recipe(string filePath) : base(filePath) { }

        public virtual string Type { get; } = "";

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
            Recipe recipe = (Recipe)Activator.CreateInstance(GetType(), true);
            recipe.Name = Name;
            recipe.Group = Group;
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

        public virtual ValidateResult Validate()
        {
            string error = OnValidate(nameof(Type));
            if (!string.IsNullOrEmpty(error))
            {
                return new ValidateResult(false, error);
            }
            error = OnValidate(nameof(Name));
            if (!string.IsNullOrEmpty(error))
            {
                return new ValidateResult(false, error);
            }
            error = OnValidate(nameof(Group));
            if (!string.IsNullOrEmpty(error))
            {
                return new ValidateResult(false, error);
            }
            return ValidateResult.Valid;
        }

        public event PropertyValidationEventHandler<Recipe> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
