using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.ModGenerator.Serialization
{
    public class ModInfoSerializer : ISerializer<McModInfo>
    {
        private static readonly JsonConverter converter = new McModInfoJsonConverter();

        public McModInfo Deserialize(string value) => JsonConvert.DeserializeObject<McModInfo>(value, converter);
        public string Serialize(McModInfo value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, converter);
        public string Serialize(McModInfo value) => JsonConvert.SerializeObject(value, converter);

        T ISerializer.DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, converter);
        object ISerializer.DeserializeObject(string value) => Deserialize(value);
        object ISerializer.DeserializeObject(string value, Type type) => Deserialize(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => Serialize((McModInfo)value, prettyPrint);
        string ISerializer.SerializeObject(object value, Type type) => Serialize((McModInfo)value);
        string ISerializer.SerializeObject(object value, bool prettyPrint) => Serialize((McModInfo)value, prettyPrint);
        string ISerializer.SerializeObject(object value) => Serialize((McModInfo)value);
    }
}
