using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.RecipeGenerator.Converters
{
    public class SmeltingRecipeJsonConverter : JsonConverter<SmeltingRecipe>
    {
        public override SmeltingRecipe ReadJson(JsonReader reader, Type objectType, SmeltingRecipe existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            SmeltingRecipe recipe = ReflectionHelper.CreateInstance<SmeltingRecipe>(true);
            if (item.TryGetValue("name", out JToken name))
            {
                recipe.Name = name.ToObject<string>();
            }
            if (item.TryGetValue("group", out JToken group))
            {
                recipe.Group = group.ToObject<string>();
            }
            if (item.TryGetValue("ingredient", out JToken ingredients))
            {
                recipe.Ingredients = ingredients.ToObject<ObservableCollection<Ingredient>>();
            }
            if (item.TryGetValue("result", out JToken result))
            {
                recipe.Result = result.ToObject<RecipeResult>();
            }
            if (item.TryGetValue("experience", out JToken experience))
            {
                recipe.Experience = experience.ToObject<float>();
            }
            if (item.TryGetValue("cookingtime", out JToken cookingTime))
            {
                recipe.CookingTime = cookingTime.ToObject<int>();
            }
            recipe.IsDirty = false;
            return recipe;
        }

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
