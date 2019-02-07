using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ForgeModGenerator.Model
{
    public abstract class JsonUpdater<T>
    {
        public JsonUpdater(object target, string jsonPath)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings()) { }
        public JsonUpdater(object target, string jsonPath, JsonSerializerSettings settings)
            : this(target, jsonPath, Formatting.Indented, settings) { }
        public JsonUpdater(object target, string jsonPath, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }
        public JsonUpdater(object target, string jsonPath, Formatting formatting, JsonConverter converter)
            : this(target, jsonPath, Formatting.Indented, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } }) { }

        public JsonUpdater(object target, string jsonPath, Formatting formatting, JsonSerializerSettings settings)
        {
            Target = target;
            Path = jsonPath;
            Formatting = Formatting;
            Settings = settings;
        }

        public Formatting Formatting { get; set; }
        public string Path { get; set; }

        protected object Target { get; set; }
        protected JsonSerializerSettings Settings { get; set; }

        protected string GetJsonFromFile() => File.ReadAllText(Path);

        protected string Serialize() => JsonConvert.SerializeObject(Target, Formatting, Settings);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        public virtual void AddToJson(T item)
        {
            if (!JsonContains(item))
            {
                // TODO: Add
            }
            ForceJsonUpdate(); // temporary solution
        }

        public virtual void RemoveFromJson(T item)
        {
            if (JsonContains(item))
            {
                // TODO: Remove
            }
            ForceJsonUpdate(); // temporary solution
        }

        public virtual void ForceJsonUpdate()
        {
            try
            {
                OverwriteJson(Serialize());
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
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
                Log.Error(ex, Log.UnexpectedErrorMessage);
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
