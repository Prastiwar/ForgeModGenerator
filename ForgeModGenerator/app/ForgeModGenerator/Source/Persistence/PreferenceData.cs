using ForgeModGenerator.Models;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Persistence
{
    public abstract class PreferenceData : ObservableDirtyObject
    {
        [JsonIgnore]
        public virtual string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");

        public virtual void SavePreferences()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            };
            string jsonContent = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(PreferenceLocation, jsonContent);
            IsDirty = false;
        }
    }
}
