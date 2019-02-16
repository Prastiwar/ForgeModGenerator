using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.ModGenerator.Models;
using ForgeModGenerator.SoundGenerator.Validations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Models
{
    [JsonConverter(typeof(SoundEventConverter))]
    public class SoundEvent : ObservableFolder<Sound>
    {
        /// <summary> IMPORTANT: Prefer other ctor, this is used for serialization purposes </summary>
        protected SoundEvent() { }

        /// <summary> IMPORTANT: Prefer to use ctor, this is used for serialization purposes </summary>
        internal static SoundEvent CreateEmpty(IEnumerable<Sound> files = null)
        {
            SoundEvent soundEvent = new SoundEvent();
            if (files != null)
            {
                soundEvent.Files = new System.Collections.ObjectModel.ObservableCollection<Sound>(files);
            }
            return soundEvent;
        }

        public SoundEvent(string path) : base(path) => EventName = Info.Name;

        public SoundEvent(IEnumerable<string> filePaths) : base(filePaths)
        {
            EventName = FormatDottedSoundNameFromFullPath(Info.FullName);
            IsDirty = false;
        }

        public SoundEvent(IEnumerable<Sound> files) : base(files) { }

        public SoundEvent(string path, IEnumerable<Sound> files) : base(path, files)
        {
            EventName = FormatDottedSoundNameFromFullPath(Info.FullName);
            IsDirty = false;
        }

        public SoundEvent(string path, SearchOption searchOption) : base(path, searchOption) { }
        public SoundEvent(string path, string fileSearchPattern) : base(path, fileSearchPattern) { }
        public SoundEvent(string path, string fileSearchPattern, SearchOption searchOption) : base(path, fileSearchPattern, searchOption) { }

        private string eventName;
        public string EventName {
            get => eventName;
            set => DirtSet(ref eventName, value);
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

        public ValidationResult IsValid(IEnumerable<SoundEvent> soundEvents)
        {
            SoundEventRules soundEventRules = new SoundEventRules();
            SoundEventValidationDependencyWrapper parameters = new SoundEventValidationDependencyWrapper() {
                SoundEvents = soundEvents,
                SoundEventBeforeChange = this
            };
            ValidationResult eventResult = soundEventRules.ValidateEventName(EventName, parameters);
            if (!eventResult.IsValid)
            {
                return eventResult;
            }
            return soundEventRules.ValidateSounds(Files);
        }

        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundNameFromFullPath(string path) => Sound.FormatSoundPathFromFullPath(null, path).Remove(0, 1).Replace("/", ".");

        // Get formatted sound from sound path modid:shorten/path/toFile to "shorten.path.toFile"
        public static string FormatDottedSoundNameFromSoundName(string name) => Sound.GetRelativePathFromSoundPath(name).Replace("/", ".");
    }
}
