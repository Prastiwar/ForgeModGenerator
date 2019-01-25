using ForgeModGenerator.Core;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Model
{
    public abstract class PreferenceData : ObservableObject
    {
        public virtual string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");

        public virtual void SavePreferences()
        {
            string jsonContent = JsonConvert.SerializeObject(this);
            File.WriteAllText(PreferenceLocation, jsonContent);
        }
    }
}
