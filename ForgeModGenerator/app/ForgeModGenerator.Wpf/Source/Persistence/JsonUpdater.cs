using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForgeModGenerator.Persistence
{
    public abstract class CollectionJsonUpdater<T> : JsonUpdaterBase<IEnumerable<T>>
    {
        public CollectionJsonUpdater(IEnumerable<T> target, string jsonPath)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings()) { }
        public CollectionJsonUpdater(IEnumerable<T> target, string jsonPath, JsonSerializerSettings settings)
            : this(target, jsonPath, Formatting.Indented, settings) { }
        public CollectionJsonUpdater(IEnumerable<T> target, string jsonPath, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }
        public CollectionJsonUpdater(IEnumerable<T> target, string jsonPath, Formatting formatting, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }

        public CollectionJsonUpdater(IEnumerable<T> target, string jsonPath, Formatting formatting, JsonSerializerSettings settings) : base(target, jsonPath)
        {
            PrettyPrint = formatting == Formatting.Indented ? true : false;
            Settings = settings;
        }

        protected JsonSerializerSettings Settings { get; set; }

        public override string Serialize(bool prettyPrint) => JsonConvert.SerializeObject(Target, BoolToFormatting(prettyPrint), Settings);

        public virtual bool JsonContains(T item)
        {
            string json = GetJsonFromFile();
            string itemJson = JsonConvert.SerializeObject(item, BoolToFormatting(PrettyPrint), Settings);
            if (json.Contains(itemJson))
            {
                itemJson = JsonConvert.SerializeObject(item, BoolToFormatting(!PrettyPrint), Settings);
            }
            return json.Contains(itemJson);
        }

        protected Formatting BoolToFormatting(bool prettyPrint) => prettyPrint ? Formatting.Indented : Formatting.None;
    }
}
