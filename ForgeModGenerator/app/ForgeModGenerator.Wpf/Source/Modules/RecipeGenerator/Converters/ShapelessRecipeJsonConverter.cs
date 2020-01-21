//using ForgeModGenerator.RecipeGenerator.Models;
//using ForgeModGenerator.Utility;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.ObjectModel;

//namespace ForgeModGenerator.RecipeGenerator.Converters
//{
//    public class ShapelessRecipeJsonConverter : JsonConverter<ShapelessRecipe>
//    {
//        public override ShapelessRecipe ReadJson(JsonReader reader, Type objectType, ShapelessRecipe existingValue, bool hasExistingValue, JsonSerializer serializer)
//        {
//            JObject item = JObject.Load(reader);
//            ShapelessRecipe recipe = ReflectionHelper.CreateInstance<ShapelessRecipe>(true);
//            if (item.TryGetValue("name", out JToken name))
//            {
//                recipe.Name = name.ToObject<string>();
//            }
//            if (item.TryGetValue("group", out JToken group))
//            {
//                recipe.Group = group.ToObject<string>();
//            }
//            if (item.TryGetValue("keys", out JToken ingredients))
//            {
//                recipe.Ingredients = ingredients.ToObject<ObservableCollection<Ingredient>>();
//            }
//            if (item.TryGetValue("result", out JToken result))
//            {
//                recipe.Result = result.ToObject<RecipeResult>();
//            }
//            recipe.IsDirty = false;
//            return recipe;
//        }

//        public override void WriteJson(JsonWriter writer, ShapelessRecipe value, JsonSerializer serializer)
//        {
//            if (serializer.Formatting == Formatting.Indented)
//            {
//                writer.WriteRawValue(" ");
//            }
//            JObject jo = new JObject {
//                { nameof(ShapelessRecipe.Type).ToLower(), value.Type },
//                { nameof(ShapelessRecipe.Name).ToLower(), value.Name },
//                { nameof(ShapelessRecipe.Group).ToLower(), value.Group ?? "" },
//                { nameof(ShapelessRecipe.Ingredients).ToLower(), JArray.FromObject(value.Ingredients, serializer) },
//                { nameof(ShapelessRecipe.Result).ToLower(), JObject.FromObject(value.Result, serializer) },
//            };
//            jo.WriteTo(writer);
//        }
//    }
//}
