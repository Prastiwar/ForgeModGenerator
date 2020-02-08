using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ForgeModGenerator.RecipeGenerator.Converters
{
    public class ShapedRecipeJsonConverter : JsonConverter<ShapedRecipe>
    {
        public override ShapedRecipe ReadJson(JsonReader reader, Type objectType, ShapedRecipe existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            ShapedRecipe recipe = ReflectionHelper.CreateInstance<ShapedRecipe>(true);
            if (item.TryGetValue("name", out JToken name))
            {
                recipe.Name = name.ToObject<string>();
            }
            if (item.TryGetValue("group", out JToken group))
            {
                recipe.Group = group.ToObject<string>();
            }
            if (item.TryGetValue("pattern", out JToken pattern))
            {
                string[] patternArray = pattern.ToObject<string[]>();
                Array.Copy(patternArray, recipe.Pattern, patternArray.Length);
            }
            if (item.TryGetValue("keys", out JToken keys))
            {
                recipe.Keys = keys.ToObject<RecipeKeyCollection>();
            }
            if (item.TryGetValue("result", out JToken result))
            {
                recipe.Result = result.ToObject<RecipeResult>();
            }
            recipe.IsDirty = false;
            return recipe;
        }

        public override void WriteJson(JsonWriter writer, ShapedRecipe value, JsonSerializer serializer)
        {
            if (serializer.Formatting == Formatting.Indented)
            {
                writer.WriteRawValue(" ");
            }
            JObject jo = new JObject {
                { nameof(ShapedRecipe.Type).ToLower(), value.Type },
                { nameof(ShapedRecipe.Name).ToLower(), value.Name },
                { nameof(ShapedRecipe.Group).ToLower(), value.Group ?? "" },
                { nameof(ShapedRecipe.Pattern).ToLower(), JArray.FromObject(value.Pattern, serializer) },
                { nameof(ShapedRecipe.Keys).ToLower(), JArray.FromObject(value.Keys, serializer) },
                { nameof(ShapedRecipe.Result).ToLower(), JObject.FromObject(value.Result, serializer) },
            };
            jo.WriteTo(writer);
        }
    }
}
