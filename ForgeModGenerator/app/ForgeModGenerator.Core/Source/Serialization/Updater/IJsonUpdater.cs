using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public interface IJsonUpdater
    {
        object Target { get; }
        string Path { get; }
        bool PrettyPrint { get; }

        IJsonUpdater SetTarget(object item);
        IJsonUpdater SetPath(string path);
        IJsonUpdater SetPrettyPrint(bool prettyPrint);

        bool IsValidToSerialize();
        bool IsUpdateAvailable();

        string Serialize();
        object DeserializeObject();
        object DeserializeObjectFromContent(string content);

        void ForceJsonUpdate();
        void ForceJsonUpdateAsync();
    }

    public interface IJsonUpdater<T> : IJsonUpdater
    {
        T DeserializeFromContent(string content);
        T Deserialize();
    }

    public interface IJsonUpdater<TCollection, TItem> : IJsonUpdater<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        bool JsonContains(TItem item);
    }
}
