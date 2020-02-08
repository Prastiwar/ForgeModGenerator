using ForgeModGenerator.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator.Serialization
{
    /// <summary> Base class to synchronize T target with json file </summary>
    public class JsonUpdater<T> : IJsonUpdater<T>
    {
        public JsonUpdater(ISerializer<T> serializer) => Serializer = serializer;

        public JsonUpdater(ISerializer<T> serializer, T target, string jsonPath) : this(serializer)
        {
            Target = target;
            Path = jsonPath;
        }

        public object Target { get; private set; }

        public string Path { get; private set; }

        public bool PrettyPrint { get; private set; }

        internal ISerializer<T> Serializer { get; }

        public IJsonUpdater<T> SetTarget(T target)
        {
            Target = target;
            return this;
        }

        public IJsonUpdater SetPath(string path)
        {
            Path = path;
            return this;
        }

        public IJsonUpdater SetPrettyPrint(bool prettyPrint)
        {
            PrettyPrint = prettyPrint;
            return this;
        }

        public T Deserialize() => Serializer.Deserialize(GetJsonFromFile());

        public T DeserializeFromContent(string content) => Serializer.Deserialize(content);

        public string Serialize() => Serializer.Serialize((T)Target, PrettyPrint);

        public async void ForceJsonUpdateAsync()
        {
            if (IsValidToSerialize())
            {
                string serializedContent = Serialize();
                await OverwriteJsonAsync(serializedContent).ConfigureAwait(false);
            }
        }

        public void ForceJsonUpdate()
        {
            if (IsValidToSerialize())
            {
                string serializedContent = Serialize();
                OverwriteJson(serializedContent);
            }
        }

        public virtual bool IsUpdateAvailable()
        {
            try
            {
                string newJson = Serialize().RemoveAllSpaces();
                string savedJson = GetJsonFromFile().RemoveAllSpaces();
                return string.Compare(newJson, savedJson, true) != 0;
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
                string json = GetJsonFromFile().RemoveAllSpaces();
                string itemJson = Serializer.Serialize(item, !PrettyPrint).RemoveAllSpaces();
                return json.Contains(itemJson);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool IsValidToSerialize() => true;

        protected string GetJsonFromFile() => File.Exists(Path) ? File.ReadAllText(Path) : "";

        protected async Task OverwriteJsonAsync(string json) => await IOHelper.WriteAllTextAsync(Path, json).ConfigureAwait(false);

        protected void OverwriteJson(string json) => File.WriteAllText(Path, json);

        IJsonUpdater IJsonUpdater.SetTarget(object target) => SetTarget((T)target);
        object IJsonUpdater.DeserializeObject() => Deserialize();
        object IJsonUpdater.DeserializeObjectFromContent(string content) => DeserializeFromContent(content);
    }
}
