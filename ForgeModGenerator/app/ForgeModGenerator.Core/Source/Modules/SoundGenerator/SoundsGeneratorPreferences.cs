using ForgeModGenerator.Serialization;

namespace ForgeModGenerator.SoundGenerator.Persistence
{
    public class SoundsGeneratorPreferences : PreferenceData
    {
        public SoundsGeneratorPreferences() => SoundJsonPrettyPrint = false;

        private bool soundJsonPrettyPrint;
        public  bool SoundJsonPrettyPrint {
            get => soundJsonPrettyPrint;
            set => DirtSetProperty(ref soundJsonPrettyPrint, value);
        }
    }
}
