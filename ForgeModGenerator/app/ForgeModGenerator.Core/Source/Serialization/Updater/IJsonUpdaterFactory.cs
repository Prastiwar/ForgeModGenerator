using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public interface IJsonUpdaterFactory<T>
    {
        IJsonUpdater<T> Create();
    }

    public interface IJsonUpdaterFactory<TCollection, TItem>
        where TCollection : IEnumerable<TItem>
    {
        IJsonUpdater<TCollection, TItem> Create();
    }
}
