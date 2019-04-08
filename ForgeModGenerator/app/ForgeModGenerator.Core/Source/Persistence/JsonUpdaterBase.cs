using ForgeModGenerator.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator.Persistence
{
    /// <summary> Base class to synchronize T target with json file </summary>
    public abstract class JsonUpdaterBase<T> : IJsonUpdater<T>
    {
        public JsonUpdaterBase(T target, string jsonPath)
        {
            Target = target;
            Path = jsonPath;
        }

        public string Path { get; set; }

        public T Target { get; set; }

        protected bool PrettyPrint;

        protected string GetJsonFromFile() => File.ReadAllText(Path);

        protected async Task OverwriteJsonAsync(string json) => await IOHelper.WriteAllTextAsync(Path, json);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        public abstract string Serialize(bool prettyPrint);

        public virtual bool IsValidToSerialize() => true;

        public async void ForceJsonUpdateAsync()
        {
            if (IsValidToSerialize())
            {
                string serializedContent = Serialize(PrettyPrint);
                await OverwriteJsonAsync(serializedContent);
            }
        }

        public void ForceJsonUpdate()
        {
            if (IsValidToSerialize())
            {
                string serializedContent = Serialize(PrettyPrint);
                OverwriteJson(serializedContent);
            }
        }

        public virtual bool IsUpdateAvailable()
        {
            try
            {
                string newJson = Serialize(PrettyPrint);
                string savedJson = GetJsonFromFile();
                if (newJson == savedJson)
                {
                    newJson = Serialize(!PrettyPrint);
                }
                return newJson == savedJson;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool JsonContains(T item)
        {
            try
            {
                string json = GetJsonFromFile();
                string itemJson = Serialize(PrettyPrint);
                if (json.Contains(itemJson))
                {
                    itemJson = Serialize(!PrettyPrint);
                }
                return json.Contains(itemJson);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
