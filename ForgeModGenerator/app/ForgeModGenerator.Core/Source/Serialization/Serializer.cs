using System;

namespace ForgeModGenerator.Serialization
{
    public interface ISerializer
    {
        T DeserializeObject<T>(string value);
        object DeserializeObject(string value);
        object DeserializeObject(string value, Type type);

        string SerializeObject(object value, Type type, bool prettyPrint);
        string SerializeObject(object value, Type type);
        string SerializeObject(object value, bool prettyPrint);
        string SerializeObject(object value);
    }
}
