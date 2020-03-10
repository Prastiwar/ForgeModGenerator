using ForgeModGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using System;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeChoiceFormProvider : ModelChoiceFormProvider<Recipe>
    {
        public override IUIChoice GetUIChoice() => new ChooseInstanceForm() {
            Types = new Type[] { typeof(ShapedRecipe), typeof(ShapelessRecipe), typeof(SmeltingRecipe) },
            MaxColumnAmount = 3
        };
    }
}
