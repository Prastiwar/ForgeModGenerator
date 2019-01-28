using ForgeModGenerator.Core;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Model
{
    public abstract class PreferenceData : ObservableObject
    {
        public virtual string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");
        public bool IsDirty { get; protected set; }

        public virtual void SavePreferences()
        {
            string jsonContent = JsonConvert.SerializeObject(this);
            File.WriteAllText(PreferenceLocation, jsonContent);
            IsDirty = false;
        }

        protected bool DirtSet<T>(ref T variable, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool set = Set(ref variable, newValue, propertyName);
            if (set)
            {
                IsDirty = true;
            }
            return set;
        }
    }
}
