using Newtonsoft.Json;
using System;
using System.IO;

namespace ForgeModGenerator.Model
{
    public abstract class JsonUpdater<T>
    {
        public JsonUpdater(object target)
        {
            Target = target;
        }

        public Formatting Formatting { get; set; }
        public string Path { get; set; }

        protected object Target { get; set; }
        protected JsonConverter Converter { get; set; }

        protected string GetJsonFromFile() => File.ReadAllText(Path);

        protected string GetJsonFromSerialize() => JsonConvert.SerializeObject(Target, Formatting, Converter);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        public void AddToJson(T item)
        {
            if (!JsonContains(item))
            {
                // TODO: Add
            }
            ForceJsonUpdate(); // temporary solution
        }

        public void RemoveFromJson(T item)
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
                OverwriteJson(GetJsonFromSerialize());
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
                string newJson = GetJsonFromSerialize();
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
            string itemJson = GetJsonFromSerialize();
            if (json.Contains(itemJson))
            {
                itemJson = JsonConvert.SerializeObject(Target, Formatting == Formatting.Indented ? Formatting.None : Formatting.Indented);
            }
            return json.Contains(itemJson);
        }
    }
}
