using ForgeModGenerator.RecipeGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using System;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeCreatorModelFormProvider : ModelFormProvider<RecipeCreator>
    {
        public override IUIElement GetUIElement() => new RecipeEditForm() {
            RecipeTypes = new Type[] { typeof(ShapedRecipe), typeof(ShapelessRecipe), typeof(SmeltingRecipe) }
        };
    }
}
