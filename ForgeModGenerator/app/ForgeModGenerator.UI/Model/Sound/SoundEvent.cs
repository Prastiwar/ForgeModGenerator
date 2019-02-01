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

        public SoundEvent(string name) : this(new List<Sound>() { new Sound(Mod.GetModidFromPath(name), name) }) { }

        public SoundEvent(string modid, string name) : this(name, new List<Sound>() { new Sound(modid, name) }) { }

        public SoundEvent(IEnumerable<Sound> sounds)
        {
            if (sounds == null)
            {
                Log.Error(null, "Called SoundEvent with null sounds parameter");
                throw new System.ArgumentNullException(nameof(sounds));
            }
            else if (sounds.Count() <= 0)
            {
                Log.Error(null, "Called SoundEvent with sounds count <= 0 parameter");
                throw new System.Exception($"{nameof(sounds)} must have at least one occurency.");
            }

            Sounds = new ObservableCollection<Sound>(sounds);
            Sounds.CollectionChanged += Sounds_CollectionChanged;
            EventName = FormatDottedSoundName(Sounds[0].FilePath);
            SetFileItem(EventName);
            IsDirty = false;
        }

        [JsonConstructor]
        public SoundEvent(string name, IEnumerable<Sound> sounds)
        {
            if (sounds == null)
            {
                Log.Error(null, "Called SoundEvent with null sounds parameter");
                throw new System.ArgumentNullException(nameof(sounds));
            }
            else if (sounds.Count() <= 0)
            {
                Log.Error(null, "Called SoundEvent with sounds count <= 0 parameter");
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
            IsDirty = false;
        }

        ~SoundEvent() { ReferenceCounter.RemoveReference(EventName, this); }

        public delegate void OnSoundChangedEventHandler(Sound soundChanged);
        public event OnSoundChangedEventHandler OnSoundAdded;
        public event OnSoundChangedEventHandler OnSoundRemoved;

        private void Sounds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
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
            set {
                ReferenceCounter.RemoveReference(eventName, this);
                DirtSet(ref eventName, value);
                ReferenceCounter.AddReference(eventName, this);
            }
        }

        private bool replace = false;
        [JsonProperty(PropertyName = "replace")]
        public bool Replace {
            get => replace;
            set => DirtSet(ref replace, value);
        }

        private string subtitle;
        [JsonProperty(PropertyName = "subtitle")]
        public string Subtitle {
            get => subtitle;
            set => DirtSet(ref subtitle, value);
        }

        private ObservableCollection<Sound> sounds;

        [JsonProperty(PropertyName = "sounds")]
        public ObservableCollection<Sound> Sounds {
            get => sounds;
            set => DirtSet(ref sounds, value);
        }

        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundName(string path)
        {
            int soundsIndex = path.IndexOf("sounds");
            if (soundsIndex == -1)
            {
                return null;
            }
            int startIndex = soundsIndex + 7;
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
                Replace = Replace,
                Subtitle = Subtitle,
                Sounds = sounds
            };
            if (countReference)
            {
                soundEvent.FilePath = FilePath;
                soundEvent.EventName = EventName;
            }
            else
            {
                soundEvent.filePath = FilePath;
                soundEvent.eventName = EventName;
            }
            soundEvent.IsDirty = false;
            return soundEvent;
        }
    }
}
