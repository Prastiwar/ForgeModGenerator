using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator.Persistence
{
    public abstract class JsonUpdater<T>
    {
        public JsonUpdater(IEnumerable<T> target, string jsonPath)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings()) { }
        public JsonUpdater(IEnumerable<T> target, string jsonPath, JsonSerializerSettings settings)
            : this(target, jsonPath, Formatting.Indented, settings) { }
        public JsonUpdater(IEnumerable<T> target, string jsonPath, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }
        public JsonUpdater(IEnumerable<T> target, string jsonPath, Formatting formatting, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }

        public JsonUpdater(IEnumerable<T> target, string jsonPath, Formatting formatting, JsonSerializerSettings settings)
        {
            Target = target;
            Path = jsonPath;
            Formatting = Formatting;
            Settings = settings;
        }

        public Formatting Formatting { get; set; }
        public string Path { get; set; }

        protected IEnumerable<T> Target { get; set; }
        protected JsonSerializerSettings Settings { get; set; }

        protected string GetJsonFromFile() => File.ReadAllText(Path);

        protected string Serialize() => JsonConvert.SerializeObject(Target, Formatting, Settings);

        protected async Task OverwriteJsonAsync(string json) => await IOHelper.WriteAllTextAsync(Path, json);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        public virtual bool IsValidToSerialize() => true;

        public async void ForceJsonUpdateAsync()
        {
            try
            {
                if (IsValidToSerialize())
                {
                    string serializedContent = Serialize();
                    await OverwriteJsonAsync(serializedContent);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't save JSON", true);
            }
        }

        public void ForceJsonUpdate()
        {
            try
            {
                if (IsValidToSerialize())
                {
                    string serializedContent = Serialize();
                    OverwriteJson(serializedContent);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't save JSON", true);
            }
        }

        public virtual bool IsUpdateAvailable()
        {
            try
            {
                string newJson = Serialize();
                string savedJson = GetJsonFromFile();
                if (newJson == savedJson)
                {
                    newJson = JsonConvert.SerializeObject(Target, Formatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
                }
                return newJson == savedJson;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public virtual bool JsonContains(T item)
        {
            string json = GetJsonFromFile();
            string itemJson = Serialize();
            if (json.Contains(itemJson))
            {
                itemJson = JsonConvert.SerializeObject(Target, Formatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
            }
            return json.Contains(itemJson);
        }
    }
}
