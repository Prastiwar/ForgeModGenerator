using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public class SoundEvent : ObservableObject, IFileItem
    {
        public SoundEvent(string name)
        {
            if (File.Exists(name))
            {
                Name = Path.GetFileName(name); // TODO: Format
                FileName = Path.GetFileName(name);
                FilePath = name;
            }
            else
            {
                Name = name;
            }
        }

        [JsonIgnore]
        public string FileName { get; }

        [JsonIgnore]
        public string FilePath { get; }

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

        private ObservableCollection<Sound> sounds;
        public ObservableCollection<Sound> Sounds {
            get => sounds;
            set => Set(ref sounds, value);
        }
    }
}
