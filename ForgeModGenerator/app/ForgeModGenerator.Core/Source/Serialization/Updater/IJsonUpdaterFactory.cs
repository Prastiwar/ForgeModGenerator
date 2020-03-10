using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public interface IJsonUpdaterFactory<T> : ISimpleFactory<IJsonUpdater<T>> { }

    public interface IJsonUpdaterFactory<TCollection, TItem> : ISimpleFactory<IJsonUpdater<TCollection, TItem>>
        where TCollection : IEnumerable<TItem>
    { }
}
