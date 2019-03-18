using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.Validations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;

namespace ForgeModGenerator.SoundGenerator.Models
{
    [JsonConverter(typeof(SoundEventConverter))]
    public class SoundEvent : ObservableFolder<Sound>, IDataErrorInfo, IValidable
    {
        /// <summary> IMPORTANT: Prefer other ctor, this is used for serialization purposes </summary>
        protected SoundEvent() { }

        /// <summary> IMPORTANT: Prefer to use ctor, this is used for serialization purposes </summary>
        internal static SoundEvent CreateEmpty(IEnumerable<Sound> files = null)
        {
            SoundEvent soundEvent = new SoundEvent();
            if (files != null)
            {
                soundEvent.Files = new WpfObservableRangeCollection<Sound>(files);
                foreach (Sound file in soundEvent.Files)
                {
                    file.PropertyChanged += soundEvent.File_PropertyChanged;
                }
            }
            return soundEvent;
        }

        public SoundEvent(string path) : base(path) => Init();

        public SoundEvent(IEnumerable<string> filePaths) : base(filePaths) => Init();
        public SoundEvent(IEnumerable<Sound> files) : base(files) => Init();

        public SoundEvent(string path, IEnumerable<Sound> files) : base(path, files) => Init();
        public SoundEvent(string path, IEnumerable<string> filePaths) : base(path, filePaths) => Init();

        public SoundEvent(string path, SearchOption searchOption) : base(path, searchOption) => Init();
        public SoundEvent(string path, string fileSearchPatterns) : base(path, fileSearchPatterns) => Init();
        public SoundEvent(string path, string fileSearchPatterns, SearchOption searchOption) : base(path, fileSearchPatterns, searchOption) => Init();

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

        private void Init()
        {
            if (Count == 1)
            {
                EventName = FormatDottedSoundNameFromFullPath(Files[0].Info.FullName);
            }
            else
            {
                EventName = FormatDottedSoundNameFromFullPath(Info.FullName);
            }
            IsDirty = false;
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
                string errorString = OnValidate(nameof(EventName));
                return new ValidationResult(string.IsNullOrEmpty(errorString), errorString);
            }
        }

        public event ValidationEventHandler<SoundEvent> Validate;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(Validate, this, propertyName);

        // Get formatted sound from full path, "shorten.path.toFile"
        public static string FormatDottedSoundNameFromFullPath(string path)
        {
            string dotterPath = Sound.FormatSoundNameFromFullPath(null, path).Remove(0, 1).Replace("/", ".");
            return string.IsNullOrEmpty(dotterPath) ? "sounds" : dotterPath;
        }

        // Get formatted sound from sound path modid:shorten/path/toFile to "shorten.path.toFile"
        public static string FormatDottedSoundNameFromSoundName(string name) => Sound.GetRelativePathFromSoundName(name).Replace("/", ".");
    }
}
