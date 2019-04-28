using System;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapedRecipe : Recipe
    {
        protected ShapedRecipe() { }
        public ShapedRecipe(string filePath) : base(filePath) { }

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
            ShapedRecipe recipe = new ShapedRecipe() {
                Keys = new RecipeKey[Keys.Length],
                Pattern = new char[Pattern.Length],
                Result = new RecipeResult {
                    Count = Result.Count,
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
            if (fromCopy is ShapedRecipe recipe)
            {
                Pattern = recipe.Pattern;
                Keys = recipe.Keys;
                Result = recipe.Result;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
