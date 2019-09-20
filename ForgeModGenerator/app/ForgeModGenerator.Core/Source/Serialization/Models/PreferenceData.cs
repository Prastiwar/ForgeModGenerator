using ForgeModGenerator.Models;
using System.IO;

namespace ForgeModGenerator.Serialization
{
    public abstract class PreferenceData : ObservableDirtyObject
    {
        public string PreferenceLocation => Path.Combine(AppPaths.Preferences, $"{GetType().Name}.json");
    }
}
