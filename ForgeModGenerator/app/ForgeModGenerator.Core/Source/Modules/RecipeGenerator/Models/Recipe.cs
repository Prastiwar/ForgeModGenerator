using ForgeModGenerator.Validation;
using System;
using System.ComponentModel;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public enum RecipeType
    {
        NoShape,
        Shaped
    }

    public sealed class Recipe : FileObject, IDataErrorInfo, IValidable<Recipe>
    {
        private Recipe() { }

        public Recipe(string filePath) : base(filePath)
        {
        }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private RecipeType type;
        public RecipeType Type {
            get => type;
            set => SetProperty(ref type, value);
        }

        private char[] pattern;
        public char[] Pattern {
            get => pattern;
            set => SetProperty(ref pattern, value);
        }

        private RecipeKey[] keys;
        public RecipeKey[] Keys {
            get => keys;
            set => SetProperty(ref keys, value);
        }

        private RecipeResult result;
        public RecipeResult Result {
            get => result;
            set => SetProperty(ref result, value);
        }

        public override object DeepClone()
        {
            Recipe recipe = new Recipe() {
                Type = Type,
                Keys = new RecipeKey[Keys.Length],
                Pattern = new char[Pattern.Length],
                Result = new RecipeResult {
                    Count = Result.Count,
                    Data = Result.Data,
                    Item = Result.Item
                }
            };
            Array.Copy(Keys, recipe.Keys, Keys.Length);
            Array.Copy(Pattern, recipe.Pattern, Pattern.Length);
            recipe.SetInfo(Info.FullName);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Recipe recipe)
            {
                Type = recipe.Type;
                Pattern = recipe.Pattern;
                Keys = recipe.Keys;
                Result = recipe.Result;

                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }

        public ValidateResult Validate() => ValidateResult.Valid;

        public event PropertyValidationEventHandler<Recipe> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
