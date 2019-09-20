using System;
using System.Collections.Generic;

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

    public interface ISerializer<T> : ISerializer
    {
        T Deserialize(string value);

        string Serialize(T value, bool prettyPrint);
        string Serialize(T value);
    }

    public interface ISerializer<TCollection, TItem> : ISerializer<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        TItem DeserializeItem(string value);

        string SerializeItem(TItem value, bool prettyPrint);
        string SerializeItem(TItem value);
    }
}
