using ForgeModGenerator.RecipeGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.RecipeGenerator.Converters
{
    public class SmeltingRecipeJsonConverter : JsonConverter<SmeltingRecipe>
    {
        public override SmeltingRecipe ReadJson(JsonReader reader, Type objectType, SmeltingRecipe existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            base.ReadJson(reader, objectType, existingValue, serializer) as SmeltingRecipe;

        public override void WriteJson(JsonWriter writer, SmeltingRecipe value, JsonSerializer serializer)
        {
            if (serializer.Formatting == Formatting.Indented)
            {
                writer.WriteRawValue(" ");
            }
            JObject jo = new JObject {
                { nameof(SmeltingRecipe.Type).ToLower(), value.Type },
                { nameof(SmeltingRecipe.Name).ToLower(), value.Name },
                { nameof(SmeltingRecipe.Group).ToLower(), value.Group ?? "" },
                { nameof(SmeltingRecipe.Experience).ToLower(), value.Experience },
                { nameof(SmeltingRecipe.CookingTime).ToLower(), value.CookingTime },
                { nameof(SmeltingRecipe.Result).ToLower(), JObject.FromObject(value.Result, serializer) },
                { "ingredient", JArray.FromObject(value.Ingredients, serializer) },
            };
            jo.WriteTo(writer);
        }
    }
}
