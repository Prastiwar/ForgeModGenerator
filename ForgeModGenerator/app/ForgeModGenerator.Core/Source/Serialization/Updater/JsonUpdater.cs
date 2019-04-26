using ForgeModGenerator.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator.Serialization
{
    /// <summary> Base class to synchronize T target with json file </summary>
    public class JsonUpdater<T> : IJsonUpdater<T>
    {
        public JsonUpdater(ISerializer<T> serializer, T target, string jsonPath)
        {
            Serializer = serializer;
            Target = target;
            Path = jsonPath;
        }

        public string Path { get; set; }

        public bool PrettyPrint { get; set; }

        protected T Target { get; set; }

        protected ISerializer<T> Serializer { get; }

        public void SetTarget(T target) => Target = target;

        void IJsonUpdater.SetTarget(object target) => SetTarget((T)target);

        protected string GetJsonFromFile() => File.ReadAllText(Path);

        protected async Task OverwriteJsonAsync(string json) => await IOHelper.WriteAllTextAsync(Path, json).ConfigureAwait(false);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        public string Serialize(bool prettyPrint) => Serializer.Serialize(Target, prettyPrint);

        public virtual bool IsValidToSerialize() => true;

        public async void ForceJsonUpdateAsync()
        {
            if (IsValidToSerialize())
            {
                string serializedContent = Serialize(PrettyPrint);
                await OverwriteJsonAsync(serializedContent).ConfigureAwait(false);
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
