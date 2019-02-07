using ForgeModGenerator.Converter;
using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.ValidationRules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ForgeModGenerator.Model
{
    [JsonConverter(typeof(SoundEventConverter))]
    public class SoundEvent : ObservableFolder<Sound>
    {
        public ValidationResult IsValid(IEnumerable<SoundEvent> soundEvents)
        {
            SoundEventValidationDependencyWrapper parameters = new SoundEventValidationDependencyWrapper() {
                SoundEvents = soundEvents,
                SoundEventBeforeChange = this
            };
            return new SoundEventRules(nameof(EventName), parameters).Validate(EventName, null);
        }

        private SoundEvent() { }

        // Create SoundEvent without any sound
        public static SoundEvent CreateEmpty(string folderPath)
        {
            if (!IOExtensions.IsPathValid(folderPath))
            {
                Log.Error(null, $"Called ObservableFolder constructor with invalid path parameter, {nameof(folderPath)}");
                throw new ArgumentException("Invalid Path", nameof(folderPath));
            }
            SoundEvent soundEvent = new SoundEvent {
                Files = new ObservableCollection<Sound>()
            };
            soundEvent.SetInfo(folderPath);
            soundEvent.EventName = soundEvent.Info.Name;
            soundEvent.IsDirty = false;
            return soundEvent;
        }

        // Create SoundEvent from single sound, path must be in sounds folder (will get modid from soundFilePath)
        public SoundEvent(string soundFilePath) : this(Mod.GetModidFromPath(soundFilePath), soundFilePath) { }

        // Create SoundEvent from single sound, path must be in sounds folder
        public SoundEvent(string modid, string soundFilePath) : this(soundFilePath, new List<Sound>() { new Sound(modid, soundFilePath) }) { }

        // Create SoundEvent from Sound collection. SoundEvent will be folder of first sound
        public SoundEvent(IEnumerable<Sound> files) : base(files)
        {
            EventName = FormatDottedSoundName(Info.FullName);
            IsDirty = false;
        }

        public SoundEvent(string folderPath, IEnumerable<Sound> sounds)
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
            if (File.Exists(folderPath))
            {
                EventName = FormatDottedSoundName(folderPath);
                SetInfo(folderPath);
            }
            else
            {
                EventName = folderPath;
            }
            IsDirty = false;
        }

        ~SoundEvent() { ReferenceCounter.RemoveReference(EventName, this); }

        private string eventName;
        public string EventName {
            get => eventName;
            set {
                ReferenceCounter.RemoveReference(EventName, this);
                ReferenceCounter.AddReference(value, this);
                DirtSet(ref eventName, value);
            }
        }

        private bool replace = false;
        public bool Replace {
            get => replace;
            set => DirtSet(ref replace, value);
        }

        private string subtitle;
        public string Subtitle {
            get => subtitle;
            set => DirtSet(ref subtitle, value);
        }

        protected override Sound CreateFileFromPath(string filePath) => new Sound(Mod.GetModidFromPath(filePath), filePath);

        public override bool Remove(Sound item) => Remove(item, false);

        private bool Remove(Sound item, bool ignoreMinValue)
        {
            if (!ignoreMinValue && Files.Count == 1)
            {
                Log.Warning("SoundEvent must have at least 1 sound", true);
                return false;
            }
            return base.Remove(item);
        }

        public override void Delete()
        {
            int length = Files.Count;
            for (int i = 0; i < length; i++)
            {
                Remove(Files[i], true);
            }
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
