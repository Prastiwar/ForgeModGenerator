//using ForgeModGenerator.RecipeGenerator.Models;
//using ForgeModGenerator.Utility;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Linq;

//namespace ForgeModGenerator.RecipeGenerator.Converters
//{
//    public class RecipeJsonConverter : JsonConverter<Recipe>
//    {
//        public override Recipe ReadJson(JsonReader reader, Type objectType, Recipe existingValue, bool hasExistingValue, JsonSerializer serializer)
//        {
//            JObject item = JObject.Load(reader);
//            if (item.TryGetValue("type", out JToken type))
//            {
//                JsonConverter actualConverter = GetActualConverter(type.ToObject<string>());
//                if (actualConverter != null)
//                {
//                    return actualConverter.ReadJson(item.CreateReader(), objectType, null, serializer) as Recipe;
//                }
//            }
//            else
//            {
//                Recipe recipe = ReflectionHelper.CreateInstance<Recipe>(true);
//                if (item.TryGetValue("name", out JToken name))
//                {
//                    recipe.Name = name.ToObject<string>();
//                }
//                if (item.TryGetValue("group", out JToken group))
//                {
//                    recipe.Group = group.ToObject<string>();
//                }
//                recipe.IsDirty = false;
//                return recipe;
//            }
//            return null;
//        }

//        public override void WriteJson(JsonWriter writer, Recipe value, JsonSerializer serializer)
//        {
//            JsonConverter actualConverter = GetActualConverter(value.Type);
//            if (actualConverter != null)
//            {
//                actualConverter.WriteJson(writer, value, serializer);
//            }
//        }

//        private JsonConverter GetActualConverter(string recipeType)
//        {
//            foreach (Type recipeClassType in ReflectionHelper.GetSubclassTypes(typeof(Recipe)))
//            {
//                Recipe newRecipe = Activator.CreateInstance(recipeClassType, true) as Recipe;
//                if (recipeType == newRecipe.Type)
//                {
//                    Type genericConverterType = typeof(JsonConverter<>).MakeGenericType(recipeClassType);
//                    Type actualConverterType = System.Reflection.Assembly.GetExecutingAssembly().GetSubclassTypes(genericConverterType).FirstOrDefault();
//                    if (actualConverterType == null)
//                    {
//                        continue;
//                    }
//                    return Activator.CreateInstance(actualConverterType) as JsonConverter;
//                }
//            }
//            return null;
//        }
//    }
//}
