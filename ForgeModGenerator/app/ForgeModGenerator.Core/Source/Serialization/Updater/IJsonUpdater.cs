using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public interface IJsonUpdater
    {
        string Path { get; set; }
        bool PrettyPrint { get; set; }

        string Serialize(bool prettyPrint);

        void SetTarget(object target);
        void ForceJsonUpdate();
        void ForceJsonUpdateAsync();

        bool IsValidToSerialize();
        bool IsUpdateAvailable();
    }

    public interface IJsonUpdater<T> : IJsonUpdater
    {
        void SetTarget(T target);
    }

    public interface IJsonUpdater<TCollection, TItem> : IJsonUpdater<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        bool JsonContains(TItem item);
    }
}
