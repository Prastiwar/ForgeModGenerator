using System;

namespace ForgeModGenerator.Model
{
    public class SoundsGeneratorPreferences : PreferenceData
    {
        public static SoundsGeneratorPreferences Default => new SoundsGeneratorPreferences() { shouldGeneratePrettyJson = true, SoundTemplate = ""};

        private bool shouldGeneratePrettyJson;
        public bool ShouldGeneratePrettyJson {
            get => shouldGeneratePrettyJson;
            set => Set(ref shouldGeneratePrettyJson, value);
        }

        private string soundTemplate;
        public string SoundTemplate {
            get => soundTemplate;
            set => Set(ref soundTemplate, value);
        }
    }
}
