using ForgeModGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;

namespace ForgeModGenerator.RecipeGenerator.Controls
{
    public class EditableRecipeKeyList : EditableList<RecipeKey>
    {
        protected override RecipeKey DefaultItem {
            get {
                char key = (char)('a' + counter);
                counter++;
                return new RecipeKey(key, "minecraft:");
            }
        }
        private int counter;
    }

    public class EditableIngredientList : EditableList<Ingredient>
    {
        protected override Ingredient DefaultItem => new Ingredient("minecraft:", "");
    }
}
