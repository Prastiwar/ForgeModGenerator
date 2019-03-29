using ForgeModGenerator.Models;
using System.IO;

namespace ForgeModGenerator.Persistence
{
    public abstract class PreferenceData : ObservableDirtyObject
    {
        public virtual string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");

        public virtual void SavePreferences()
        {
            string jsonContent = GetJsonContent();
            File.WriteAllText(PreferenceLocation, jsonContent);
            IsDirty = false;
        }

        protected abstract string GetJsonContent();
    }
}
