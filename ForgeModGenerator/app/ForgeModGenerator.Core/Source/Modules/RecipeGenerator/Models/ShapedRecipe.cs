using ForgeModGenerator.Utility;
using System;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapedRecipe : Recipe
    {
        public ShapedRecipe()
        {
            Keys = new RecipeKeyCollection();
            Result = new RecipeResult();
        }

        public override string Type => "crafting_shaped";

        public string[] Pattern { get; } = new string[3] { "   ", "   ", "   " };

        private RecipeKeyCollection keys;
        public RecipeKeyCollection Keys {
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
            ShapedRecipe recipe = (ShapedRecipe)base.DeepClone();
            recipe.Keys = Keys.DeepCollectionClone<RecipeKeyCollection, RecipeKey>();
            recipe.Result = new RecipeResult {
                Count = Result.Count,
                Item = Result.Item
            };
            Array.Copy(Pattern, recipe.Pattern, Pattern.Length);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ShapedRecipe recipe)
            {
                Array.Copy(recipe.Pattern, Pattern, Pattern.Length);
                Keys = recipe.Keys;
                Result = recipe.Result;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
