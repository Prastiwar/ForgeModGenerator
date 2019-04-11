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

    public interface ISerializer<T>
    {
        T DeserializeObject(string value);

        string SerializeObject(T value, bool prettyPrint);
        string SerializeObject(T value);
    }
}
