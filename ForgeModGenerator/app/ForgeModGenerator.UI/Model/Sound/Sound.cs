using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace ForgeModGenerator.Model
{
    public class Sound : FileItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SoundType { file, @event }

        private Sound() { }

        public Sound(string modid, string name)
        {
            if (modid == null)
            {
                throw new System.ArgumentNullException(nameof(modid));
            }

            if (File.Exists(name))
            {
                Name = FormatSoundName(modid, name);
                SetFileItem(name);
            }
            else
            {
                Name = name;
            }
        }

        private string name;
        [JsonProperty(PropertyName = "name")]
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private float volume = 1.0f;
        [JsonProperty(PropertyName = "volume")]
        public float Volume {
            get => volume;
            set => Set(ref volume, Math.Clamp(value));
        }

        private float pitch = 1.0f;
        [JsonProperty(PropertyName = "pitch")]
        public float Pitch {
            get => pitch;
            set => Set(ref pitch, value);
        }

        private int weight = 1;
        [JsonProperty(PropertyName = "weight")]
        public int Weight {
            get => weight;
            set => Set(ref weight, value);
        }

        private bool stream = false;
        [JsonProperty(PropertyName = "stream")]
        public bool Stream {
            get => stream;
            set => Set(ref stream, value);
        }

        private int attenuationDistance;
        [JsonProperty(PropertyName = "attenuation_distance")]
        public int AttenuationDistance {
            get => attenuationDistance;
            set => Set(ref attenuationDistance, value);
        }

        private bool preload = false;
        [JsonProperty(PropertyName = "preload")]
        public bool Preload {
            get => preload;
            set => Set(ref preload, value);
        }

        private SoundType type = SoundType.file;
        [JsonProperty(PropertyName = "type")]
        public SoundType Type {
            get => type;
            set => Set(ref type, value);
        }

        // Get formatted sound from full path, "modid:shorten/path/toFile"
        public static string FormatSoundName(string modid, string path)
        {
            int startIndex = path.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            string shortPath = Path.ChangeExtension(path.Substring(startIndex, path.Length - startIndex), null);
            return $"{modid}:{shortPath}";
        }

        public override bool ShouldSerializeFilePath() => false;
        public override bool ShouldSerializeFileName() => false;

        public override object DeepClone() => DeepClone(true);

        public override object DeepClone(bool countReference)
        {
            Sound sound = new Sound() {
                FileName = FileName,
                Name = Name,
                Volume = Volume,
                Pitch = Pitch,
                Weight = Weight,
                Stream = Stream,
                AttenuationDistance = AttenuationDistance,
                Preload = Preload,
                Type = Type
            };
            if (countReference)
            {
                sound.FilePath = FilePath;
            }
            else
            {
                sound.filePath = FilePath;
            }
            return sound;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Sound sound)
            {
                FileName = sound.FileName;
                FilePath = sound.FilePath;
                Name = sound.Name;
                Volume = sound.Volume;
                Pitch = sound.Pitch;
                Weight = sound.Weight;
                Stream = sound.Stream;
                AttenuationDistance = sound.AttenuationDistance;
                Preload = sound.Preload;
                Type = sound.Type;
                return true;
            }
            return false;
        }
    }
}
