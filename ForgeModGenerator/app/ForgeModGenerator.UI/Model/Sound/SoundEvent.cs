using GalaSoft.MvvmLight;

namespace ForgeModGenerator.Model
{
    public class SoundEvent : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private bool replace = false;
        public bool Replace {
            get => replace;
            set => Set(ref replace, value);
        }

        private string subtitle;
        public string Subtitle {
            get => subtitle;
            set => Set(ref subtitle, value);
        }

        private Sound[] sounds;
        public Sound[] Sounds {
            get => sounds;
            set => Set(ref sounds, value);
        }
    }
}
