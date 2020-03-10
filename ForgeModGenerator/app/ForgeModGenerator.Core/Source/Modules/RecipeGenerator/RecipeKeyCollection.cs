using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeKeyCollection : ObservableCollection<RecipeKey>
    {
        private static int counter;

        public RecipeKey AddNew(string item)
        {
            char key = GetCurrentKey();
            while (this.Find(x => x.Key == key) != default)
            {
                counter++;
                key = GetCurrentKey();
            }
            RecipeKey recipeKey = new RecipeKey(key, item);
            Add(recipeKey);
            return recipeKey;
        }

        private char GetCurrentKey() => (char)('a' + counter);

    }
}
