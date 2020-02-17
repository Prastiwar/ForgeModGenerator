using ForgeModGenerator.Utility;
using System;

namespace ForgeModGenerator.RecipeGenerator.Models
{
    public class ShapedRecipe : Recipe
    {
        public ShapedRecipe() : base()
        {
            Keys = new RecipeKeyCollection();
            Pattern = new ShapedPattern();
        }

        public override string Type => "crafting_shaped";

        public ShapedPattern Pattern { get; }

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
            recipe.Pattern.Set(Pattern);
            recipe.IsDirty = false;
            return recipe;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ShapedRecipe recipe)
            {
                Pattern.Set(recipe.Pattern);
                Keys = recipe.Keys;
                return base.CopyValues(fromCopy);
            }
            return false;
        }
    }
}
