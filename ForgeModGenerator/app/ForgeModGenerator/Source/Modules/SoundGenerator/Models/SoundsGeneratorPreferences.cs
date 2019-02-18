using ForgeModGenerator.Models;
using Newtonsoft.Json;

namespace ForgeModGenerator.SoundGenerator.Models
{
    public class SoundsGeneratorPreferences : PreferenceData
    {
        public SoundsGeneratorPreferences()
        {
            JsonFormatting = Formatting.None;
        }

        private Formatting jsonFormatting;
        public Formatting JsonFormatting {
            get => jsonFormatting;
            set => DirtSet(ref jsonFormatting, value);
        }
    }
}
