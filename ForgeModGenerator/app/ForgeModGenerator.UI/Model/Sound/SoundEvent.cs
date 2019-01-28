using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Model
{
    public class SoundEvent : ObservableObject, IFileItem
    {
        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundName(string path)
        {
            int startIndex = path.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            return Path.ChangeExtension(path.Substring(startIndex, path.Length - startIndex), null);
        }

        public SoundEvent(string name) : this(name, new List<Sound>()) { }

        [JsonConstructor]
        public SoundEvent(string name, IEnumerable<Sound> sounds)
        {
            Sounds = new ObservableCollection<Sound>(sounds);
            if (File.Exists(name))
            {
                EventName = FormatDottedSoundName(name);
                SetFileItem(name);
            }
            else
            {
                EventName = name;
            }
        }

        public SoundEvent(string name, bool replace, string subtitle, Sound[] sounds)
        {
            Replace = replace;
            Subtitle = subtitle;
            Sounds = new ObservableCollection<Sound>(sounds.ToList());
            if (File.Exists(name))
            {
                EventName = FormatDottedSoundName(name);
                SetFileItem(name);
            }
            else
            {
                EventName = name;
            }
        }

        [JsonIgnore]
        public string FileName { get; protected set; }

        [JsonIgnore]
        public string FilePath { get; protected set; }

        public void SetFileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(eventName);
        }

        private string eventName;
        public string EventName {
            get => eventName;
            set => Set(ref eventName, value);
        }

        private bool replace = false;
        [JsonProperty(PropertyName = "replace")]
        public bool Replace {
            get => replace;
            set => Set(ref replace, value);
        }

        private string subtitle;
        [JsonProperty(PropertyName = "subtitle")]
        public string Subtitle {
            get => subtitle;
            set => Set(ref subtitle, value);
        }

        private ObservableCollection<Sound> sounds;
        [JsonProperty(PropertyName = "sounds")]
        public ObservableCollection<Sound> Sounds {
            get => sounds;
            set => Set(ref sounds, value);
        }
    }
}
