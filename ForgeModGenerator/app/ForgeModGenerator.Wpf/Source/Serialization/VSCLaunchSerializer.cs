using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.Serialization
{
    public class VSCLaunchSerializer : ISerializer<VSCLaunch>
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() {
            ContractResolver = new LowercasePropertyResolver()
        };

        public VSCLaunch Deserialize(string value) => JsonConvert.DeserializeObject(value, settings) as VSCLaunch;

        public string Serialize(VSCLaunch value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, settings);
        public string Serialize(VSCLaunch value) => JsonConvert.SerializeObject(value, settings);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((VSCLaunch)value);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((VSCLaunch)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((VSCLaunch)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((VSCLaunch)value);
    }
}