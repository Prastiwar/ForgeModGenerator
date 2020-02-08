using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator.Serialization
{
    public class CollectionJsonUpdater<TCollection, TItem> : JsonUpdater<TCollection>, IJsonUpdater<TCollection, TItem>
        where TCollection : IEnumerable<TItem>
    {
        public CollectionJsonUpdater(ISerializer<TCollection, TItem> serializer)
            : base(serializer) { }

        public CollectionJsonUpdater(ISerializer<TCollection, TItem> serializer, TCollection target, string jsonPath)
            : base(serializer, target, jsonPath) { }

        protected ISerializer<TCollection, TItem> SerializerCasted => Serializer as ISerializer<TCollection, TItem>;

        public virtual bool JsonContains(TItem item)
        {
            try
            {
                string json = GetJsonFromFile().RemoveAllSpaces();
                string itemJson = SerializerCasted.SerializeItem(item, !PrettyPrint).RemoveAllSpaces();
                return json.Contains(itemJson);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
