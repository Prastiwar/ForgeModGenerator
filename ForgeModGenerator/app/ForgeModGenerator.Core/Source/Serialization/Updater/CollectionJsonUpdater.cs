using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public class CollectionJsonUpdater<TCollection, TItem> : JsonUpdater<TCollection>, IJsonUpdater<TCollection, TItem>
        where TCollection : IEnumerable<TItem>
    {
        public CollectionJsonUpdater(ISerializer<TCollection, TItem> serializer, TCollection target, string jsonPath) : base(serializer, target, jsonPath) { }

        protected ISerializer<TCollection, TItem> SerializerCast => Serializer as ISerializer<TCollection, TItem>;

        public virtual bool JsonContains(TItem item)
        {
            string json = GetJsonFromFile();
            string itemJson = SerializerCast.SerializeItem(item, PrettyPrint);
            if (json.Contains(itemJson))
            {
                itemJson = SerializerCast.SerializeItem(item, !PrettyPrint);
            }
            return json.Contains(itemJson);
        }
    }
}
