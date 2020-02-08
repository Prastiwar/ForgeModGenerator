using ForgeModGenerator.RecipeGenerator.Models;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeKeyCollection : ObservableCollection<RecipeKey>
    {
        private static int counter;

        public RecipeKey AddNew(string item)
        {
            char key = (char)('a' + counter);
            counter++;
            RecipeKey recipeKey = new RecipeKey(key, item);
            Add(recipeKey);
            return recipeKey;
        }

    }
}
