﻿using ForgeModGenerator.Persistence;
using Newtonsoft.Json;

namespace ForgeModGenerator.SoundGenerator.Persistence
{
    public class SoundsGeneratorPreferences : PreferenceData
    {
        public SoundsGeneratorPreferences() => JsonFormatting = Formatting.None;

        private Formatting jsonFormatting;
        public Formatting JsonFormatting {
            get => jsonFormatting;
            set => DirtSetProperty(ref jsonFormatting, value);
        }
    }
}
