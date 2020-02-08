using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public class CollectionJsonUpdaterFactory<TCollection, TItem> : IJsonUpdaterFactory<TCollection, TItem>
        where TCollection : IEnumerable<TItem>
    {
        public CollectionJsonUpdaterFactory(ISerializer<TCollection, TItem> serializer) => this.serializer = serializer;

        private readonly ISerializer<TCollection, TItem> serializer;

        public IJsonUpdater<TCollection, TItem> Create() => new CollectionJsonUpdater<TCollection, TItem>(serializer);
    }
}
