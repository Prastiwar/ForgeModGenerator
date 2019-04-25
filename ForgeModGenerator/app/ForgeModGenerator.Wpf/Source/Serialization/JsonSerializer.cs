using Newtonsoft.Json;
using System;

namespace ForgeModGenerator.Serialization
{
    public sealed class JsonSerializer : ISerializer
    {
        public T DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value);
        public object DeserializeObject(string value) => JsonConvert.DeserializeObject(value);
        public object DeserializeObject(string value, Type type) => JsonConvert.DeserializeObject(value, type);

        public string SerializeObject(object value, Type type, bool prettyPrint) => JsonConvert.SerializeObject(value, type, prettyPrint ? Formatting.Indented : Formatting.None, new JsonSerializerSettings());
        public string SerializeObject(object value, Type type) => JsonConvert.SerializeObject(value, type, new JsonSerializerSettings());
        public string SerializeObject(object value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None);
        public string SerializeObject(object value) => JsonConvert.SerializeObject(value);
    }

    public sealed class JsonSerializer<T> : ISerializer<T>
    {
        public T Deserialize(string value) => JsonConvert.DeserializeObject<T>(value);

        public string Serialize(T value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None);
        public string Serialize(T value) => JsonConvert.SerializeObject(value);

        TObject ISerializer.DeserializeObject<TObject>(string value) => JsonConvert.DeserializeObject<TObject>(value);
        object ISerializer.DeserializeObject(string value, Type type) => JsonConvert.DeserializeObject(value, type);
        object ISerializer.DeserializeObject(string value) => JsonConvert.DeserializeObject(value);
        string ISerializer.SerializeObject(object value, Type type, bool prettyPrint) => JsonConvert.SerializeObject(value, type, prettyPrint ? Formatting.Indented : Formatting.None, new JsonSerializerSettings());
        string ISerializer.SerializeObject(object value, Type type) => JsonConvert.SerializeObject(value, type, new JsonSerializerSettings());
        string ISerializer.SerializeObject(object value, bool prettyPrint) => JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None);
        string ISerializer.SerializeObject(object value) => JsonConvert.SerializeObject(value);
    }
}
