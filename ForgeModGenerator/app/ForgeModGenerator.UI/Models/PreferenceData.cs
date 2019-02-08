using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Models
{
    public abstract class PreferenceData : ObservableDirtyObject
    {
        public virtual string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");

        public virtual void SavePreferences()
        {
            string jsonContent = JsonConvert.SerializeObject(this);
            File.WriteAllText(PreferenceLocation, jsonContent);
            IsDirty = false;
        }
    }
}
