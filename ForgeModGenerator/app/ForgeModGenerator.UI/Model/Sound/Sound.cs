using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ForgeModGenerator.Model
{
    public class Sound : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private float volume;
        public float Volume {
            get => volume;
            set => Set(ref volume, value);
        }

        private float pitch;
        public float Pitch {
            get => pitch;
            set => Set(ref pitch, value);
        }

        private int weight;
        public int Weight {
            get => weight;
            set => Set(ref weight, value);
        }

        private bool stream;
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

        private bool preload;
        public bool Preload {
            get => preload;
            set => Set(ref preload, value);
        }

        private string type;
        public string Type {
            get => type;
            set => Set(ref type, value);
        }
    }
}
