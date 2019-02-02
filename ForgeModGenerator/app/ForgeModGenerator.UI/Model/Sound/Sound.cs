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
                Name = FormatSoundPath(modid, name);
                SetInfo(name);
            }
            else
            {
                Name = name;
            }
            IsDirty = false;
        }

        [JsonIgnore]
        public string ShortPath {
            get => Name.Remove(0, Name.IndexOf(':') + 1);
            set => Name = FormatSoundShortPath(Mod.GetModidFromPath(Name), value);
        }

        private string name;
        [JsonProperty(PropertyName = "name")]
        public string Name {
            get => name;
            set => DirtSet(ref name, value);
        }

        private float volume = 1.0f;
        [JsonProperty(PropertyName = "volume")]
        public float Volume {
            get => volume;
            set => DirtSet(ref volume, Math.Clamp(value));
        }

        private float pitch = 1.0f;
        [JsonProperty(PropertyName = "pitch")]
        public float Pitch {
            get => pitch;
            set => DirtSet(ref pitch, value);
        }

        private int weight = 1;
        [JsonProperty(PropertyName = "weight")]
        public int Weight {
            get => weight;
            set => DirtSet(ref weight, value);
        }

        private bool stream = false;
        [JsonProperty(PropertyName = "stream")]
        public bool Stream {
            get => stream;
            set => DirtSet(ref stream, value);
        }

        private int attenuationDistance;
        [JsonProperty(PropertyName = "attenuation_distance")]
        public int AttenuationDistance {
            get => attenuationDistance;
            set => DirtSet(ref attenuationDistance, value);
        }

        private bool preload = false;
        [JsonProperty(PropertyName = "preload")]
        public bool Preload {
            get => preload;
            set => DirtSet(ref preload, value);
        }

        private SoundType type = SoundType.file;
        [JsonProperty(PropertyName = "type")]
        public SoundType Type {
            get => type;
            set => DirtSet(ref type, value);
        }

        public override object DeepClone()
        {
            Sound sound = new Sound() {
                Name = Name,
                Volume = Volume,
                Pitch = Pitch,
                Weight = Weight,
                Stream = Stream,
                AttenuationDistance = AttenuationDistance,
                Preload = Preload,
                Type = Type
            };
            sound.SetInfo(Info.FullName);
            return sound;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Sound sound)
            {
                Name = sound.Name;
                Volume = sound.Volume;
                Pitch = sound.Pitch;
                Weight = sound.Weight;
                Stream = sound.Stream;
                AttenuationDistance = sound.AttenuationDistance;
                Preload = sound.Preload;
                Type = sound.Type;

                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }

        // Get formatted sound from full path, "modid:shorten/path/toFile"
        public static string FormatSoundPath(string modid, string path)
        {
            int startIndex = path.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            string shortPath = Path.ChangeExtension(path.Substring(startIndex, path.Length - startIndex), null);
            return FormatSoundShortPath(modid, shortPath);
        }

        // Get formatted sound from short path, "modid:shorten/path/toFile"
        public static string FormatSoundShortPath(string modid, string shortPath) => $"{modid}:{shortPath}";
    }
}
