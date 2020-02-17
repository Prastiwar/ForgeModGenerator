using ForgeModGenerator.Utility;
using System;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapedRecipe : Recipe
    {
        public ShapedRecipe() : base() => Keys = new RecipeKeyCollection();

        public override string Type => "crafting_shaped";

        public string[] Pattern { get; } = new string[3] { "   ", "   ", "   " };

        // TODO: Refactor Pattern:
        //protected string[] Pattern { get; } = new string[3] { "   ", "   ", "   " };
        //public char GetPatternKey(int row, int column) => Pattern[row][column];
        //public void SetPattern(int row, int column, char key)
        //{
        //    if (row < 0 || row > 2)
        //    {
        //        throw new IndexOutOfRangeException(nameof(row));
        //    }
        //    if (column < 0 || column > 2)
        //    {
        //        throw new IndexOutOfRangeException(nameof(column));
        //    }
        //    Pattern[row] = Pattern[row].SetCharAt(column, key);
        //}

        private RecipeKeyCollection keys;
        public RecipeKeyCollection Keys {
            get => keys;
            set => SetProperty(ref keys, value);
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
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
