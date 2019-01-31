using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Model
{
    public class SoundEvent : FileItem
    {
        private SoundEvent() { }

        public SoundEvent(string name) : this(name, new List<Sound>() { new Sound(Mod.GetModidFromPath(name), name) }) { }

        public SoundEvent(string modid, string name) : this(name, new List<Sound>() { new Sound(modid, name) }) { }

        [JsonConstructor]
        public SoundEvent(string name, IEnumerable<Sound> sounds)
        {
            if (sounds == null)
            {
                throw new System.ArgumentNullException(nameof(sounds));
            }
            else if (sounds.Count() <= 0)
            {
                throw new System.Exception($"{nameof(sounds)} must have at least one occurency.");
            }

            Sounds = new ObservableCollection<Sound>(sounds);
            Sounds.CollectionChanged += Sounds_CollectionChanged;
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

        public delegate void OnSoundChangedEventHandler(Sound soundChanged);
        public event OnSoundChangedEventHandler OnSoundAdded;
        public event OnSoundChangedEventHandler OnSoundRemoved;

        private void Sounds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    OnSoundAdded?.Invoke((Sound)item);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    Sound sound = (Sound)item;
                    ReferenceCounter.RemoveReference(sound.FilePath, sound);
                    OnSoundRemoved?.Invoke(sound);
                }
            }
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

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is SoundEvent soundEvent)
            {
                FileName = soundEvent.FileName;
                FilePath = soundEvent.FilePath;
                EventName = soundEvent.EventName;
                Replace = soundEvent.Replace;
                Subtitle = soundEvent.Subtitle;
                Sounds = new ObservableCollection<Sound>(soundEvent.Sounds);
                Sounds.CollectionChanged += Sounds_CollectionChanged;
                return true;
            }
            return false;
        }

        public override bool ShouldSerializeFilePath() => false;
        public override bool ShouldSerializeFileName() => false;

        public override object DeepClone() => DeepClone(true);

        public override object DeepClone(bool countReference)
        {
            ObservableCollection<Sound> sounds = new ObservableCollection<Sound>();
            foreach (Sound sound in Sounds)
            {
                sounds.Add((Sound)sound.DeepClone(countReference));
            }
            SoundEvent soundEvent = new SoundEvent() {
                FileName = FileName,
                EventName = EventName,
                Replace = Replace,
                Subtitle = Subtitle,
                Sounds = sounds
            };
            if (countReference)
            {
                soundEvent.FilePath = FilePath;
            }
            else
            {
                soundEvent.filePath = FilePath;
            }
            return soundEvent;
        }
    }
}
