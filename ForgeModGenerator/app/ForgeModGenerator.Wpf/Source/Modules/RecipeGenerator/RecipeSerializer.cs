using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Converters;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.RecipeGenerator.Serialization
{
    public sealed class RecipeSerializer : ISerializer<Recipe>
    {
        private static readonly JsonSerializerSettings settings = GetSettings();

        private static JsonSerializerSettings GetSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            PropertyContractResolver resolver = new PropertyContractResolver();
            resolver.IgnoreProperty(typeof(ObservableDirtyObject), nameof(Recipe.IsDirty));
            resolver.SetAllLetterCase(Lettercase.Lowercase);
            resolver.SetNullStringEmpty(true);
            settings.ContractResolver = resolver;
            settings.Converters.Add(new RecipeJsonConverter());
            return settings;
        }

        public Recipe Deserialize(string value) => JsonConvert.DeserializeObject<Recipe>(value, settings);
        public string Serialize(Recipe value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, settings);
        public string Serialize(Recipe value) => JsonConvert.SerializeObject(value, settings);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((Recipe)value, prettyPrint);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((Recipe)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((Recipe)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((Recipe)value);
    }
}
