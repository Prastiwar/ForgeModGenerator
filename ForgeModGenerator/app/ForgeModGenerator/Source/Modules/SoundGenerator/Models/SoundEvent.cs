using ForgeModGenerator.Converters;
using ForgeModGenerator.Validations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace ForgeModGenerator.SoundGenerator.Models
{
    [JsonConverter(typeof(SoundEventConverter))]
    public class SoundEvent : ObservableFolder<Sound>, IDataErrorInfo
    {
        /// <summary> IMPORTANT: Prefer other ctor, this is used for serialization purposes </summary>
        protected SoundEvent() { }

        /// <summary> IMPORTANT: Prefer to use ctor, this is used for serialization purposes </summary>
        internal static SoundEvent CreateEmpty(IEnumerable<Sound> files = null)
        {
            SoundEvent soundEvent = new SoundEvent();
            if (files != null)
            {
                soundEvent.Files = new ObservableCollection<Sound>(files);
                foreach (Sound file in soundEvent.Files)
                {
                    file.PropertyChanged += soundEvent.File_PropertyChanged;
                }
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

        public ValidationResult IsValid {
            get {
                string errorString = Validate(nameof(EventName));
                return new ValidationResult(string.IsNullOrEmpty(errorString), errorString);
            }
        }

        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => Validate(propertyName);

        private event ValidationEventHandler<SoundEvent> OnValidateHandler;
        public event ValidationEventHandler<SoundEvent> OnValidate {
            add => OnValidateHandler += value;
            remove => OnValidateHandler -= value;
        }

        private string Validate(string propertyName)
        {
            foreach (Delegate handler in OnValidateHandler?.GetInvocationList())
            {
                string error = ((ValidationEventHandler<SoundEvent>)handler).Invoke(this, propertyName);
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
            }
            return null;
        }

        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundNameFromFullPath(string path) => Sound.FormatSoundNameFromFullPath(null, path).Remove(0, 1).Replace("/", ".");

        // Get formatted sound from sound path modid:shorten/path/toFile to "shorten.path.toFile"
        public static string FormatDottedSoundNameFromSoundName(string name) => Sound.GetRelativePathFromSoundName(name).Replace("/", ".");
    }
}
