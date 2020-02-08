using ForgeModGenerator.Converters;
using ForgeModGenerator.RecipeGenerator.Models;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Converters
{
    public class RecipeKeyListRecipeKeyConverter : TupleValueConverter<RecipeKeyCollection, RecipeKey> { }
    public class IngredientListIngredientConverter : TupleValueConverter<ObservableCollection<Ingredient>, Ingredient> { }
}
