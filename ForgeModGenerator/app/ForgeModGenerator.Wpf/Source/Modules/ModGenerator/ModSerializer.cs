using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.ModGenerator.Serialization
{
    public class ModSerializer : ISerializer<Mod>
    {
        private static readonly JsonSerializerSettings settings = GetSettings();

        private static JsonSerializerSettings GetSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            return settings;
        }

        public Mod Deserialize(string value) => JsonConvert.DeserializeObject<Mod>(value, settings);
        public string Serialize(Mod value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, settings);
        public string Serialize(Mod value) => JsonConvert.SerializeObject(value, settings);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((Mod)value, prettyPrint);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((Mod)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((Mod)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((Mod)value);
    }
}
