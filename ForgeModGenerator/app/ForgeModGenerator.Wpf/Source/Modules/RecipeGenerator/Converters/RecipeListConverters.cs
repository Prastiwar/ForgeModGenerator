using ForgeModGenerator.Converters;
using ForgeModGenerator.RecipeGenerator.Models;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Converters
{
    public class RecipeKeyListRecipeKeyConverter : TupleValueConverter<ObservableCollection<RecipeKey>, RecipeKey> { }
    public class IngredientListIngredientConverter : TupleValueConverter<ObservableCollection<Ingredient>, Ingredient> { }
}
