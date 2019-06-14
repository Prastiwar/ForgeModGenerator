using System;
using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using Newtonsoft.Json;

namespace ForgeModGenerator.Serialization
{
    public sealed class PreferenceDataSerializer : ISerializer<PreferenceData>
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = GetResolver()
        };

        private static PropertyContractResolver GetResolver()
        {
            PropertyContractResolver resolver = new PropertyContractResolver();
            resolver.IgnoreProperty(typeof(PreferenceData), nameof(PreferenceData.PreferenceLocation));
            resolver.IgnoreProperty(typeof(ObservableDirtyObject), nameof(PreferenceData.IsDirty));
            return resolver;
        }

        public PreferenceData Deserialize(string value) => JsonConvert.DeserializeObject(value, settings) as PreferenceData;

        public string Serialize(PreferenceData value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, settings);
        public string Serialize(PreferenceData value) => JsonConvert.SerializeObject(value, settings);
        
        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((PreferenceData)value);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((PreferenceData)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((PreferenceData)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((PreferenceData)value);
    }
}
