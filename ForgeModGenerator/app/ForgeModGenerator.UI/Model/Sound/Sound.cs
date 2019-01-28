using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace ForgeModGenerator.Model
{
    public class Sound : ObservableObject, IFileItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SoundType { sound, @event }

        public Sound(string name)
        {
            if (File.Exists(name))
            {
                Name = ViewModel.SoundGeneratorViewModel.FormatSoundName("temp", name);
                FileName = Path.GetFileName(name);
                FilePath = name;
            }
            else
            {
                Name = name;
            }
        }

        [JsonIgnore]
        public string FileName { get; protected set; }

        [JsonIgnore]
        public string FilePath { get; protected set; }

        public void SetFileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(name);
        }

        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private float volume = 1.0f;
        public float Volume {
            get => volume;
            set => Set(ref volume, Math.Clamp(value));
        }

        private float pitch = 1.0f;
        public float Pitch {
            get => pitch;
            set => Set(ref pitch, value);
        }

        private int weight = 1;
        public int Weight {
            get => weight;
            set => Set(ref weight, value);
        }

        private bool stream = false;
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
        public bool Preload {
            get => preload;
            set => Set(ref preload, value);
        }

        private SoundType type = SoundType.sound;
        public SoundType Type {
            get => type;
            set => Set(ref type, value);
        }
    }
}
