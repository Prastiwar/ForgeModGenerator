using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Model
{
    public class SoundEvent : ObservableFolder<Sound>
    {
        private SoundEvent() { }

        public SoundEvent(string path) : this(new List<Sound>() { new Sound(Mod.GetModidFromPath(path), path) }) { }

        public SoundEvent(string modid, string name) : this(name, new List<Sound>() { new Sound(modid, name) }) { }

        public SoundEvent(IEnumerable<Sound> files) : base(files)
        {
            EventName = FormatDottedSoundName(Info.FullName);
            IsDirty = false;
        }

        //[JsonConstructor]
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

            Files = new ObservableCollection<Sound>(sounds);
            if (File.Exists(name))
            {
                EventName = FormatDottedSoundName(name);
                SetInfo(name);
            }
            else
            {
                EventName = name;
            }
            IsDirty = false;
        }

        ~SoundEvent() { ReferenceCounter.RemoveReference(EventName, this); }

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
        
        public override object DeepClone()
        {
            ObservableFolder<Sound> baseClone = (ObservableFolder<Sound>)base.DeepClone();
            SoundEvent clone = new SoundEvent() {
                Replace = Replace,
                Subtitle = Subtitle,
                Files = baseClone.Files
            };
            clone.SetInfo(baseClone.Info.FullName);
            clone.EventName = EventName;
            clone.IsDirty = false;
            return clone;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is SoundEvent item)
            {
                EventName = item.EventName;
                Replace = item.Replace;
                Subtitle = item.Subtitle;
                base.CopyValues(fromCopy);
                return true;
            }
            return false;
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
    }
}
