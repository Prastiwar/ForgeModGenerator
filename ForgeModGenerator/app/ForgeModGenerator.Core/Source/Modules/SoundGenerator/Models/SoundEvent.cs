﻿//using ForgeModGenerator.Validation;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;

//namespace ForgeModGenerator.SoundGenerator.Models
//{
//    /// <summary> Collection of Sound files. Minecrafts SoundEvent representation. </summary>
//    public sealed class SoundEvent : ObservableFolder<Sound>, IDataErrorInfo, IValidable
//    {
//        /// <summary> IMPORTANT: Prefer other ctor, this is used for serialization purposes </summary>
//        private SoundEvent() { }

//        /// <summary> IMPORTANT: Prefer to use ctor, this is used for serialization purposes </summary>
//        public static SoundEvent CreateEmpty(IEnumerable<Sound> files = null)
//        {
//            SoundEvent soundEvent = new SoundEvent();
//            if (files != null)
//            {
//                soundEvent.Files = new ObservableRangeCollection<Sound>(files);
//                foreach (Sound file in soundEvent.Files)
//                {
//                    file.PropertyChanged += soundEvent.OnFilePropertyChanged;
//                }
//            }
//            return soundEvent;
//        }

//        public SoundEvent(string path) : base(path) => Init();

//        public SoundEvent(IEnumerable<string> filePaths) : base(filePaths) => Init();
//        public SoundEvent(IEnumerable<Sound> files) : base(files) => Init();

//        public SoundEvent(string path, IEnumerable<Sound> files) : base(path, files) => Init();
//        public SoundEvent(string path, IEnumerable<string> filePaths) : base(path, filePaths) => Init();

//        private string eventName;
//        public string EventName {
//            get => eventName;
//            set => DirtSetProperty(ref eventName, value);
//        }

//        private bool replace = false;
//        public bool Replace {
//            get => replace;
//            set => DirtSetProperty(ref replace, value);
//        }

//        private string subtitle;
//        public string Subtitle {
//            get => subtitle;
//            set => DirtSetProperty(ref subtitle, value);
//        }

//        public override object DeepClone()
//        {
//            ObservableFolder<Sound> baseClone = (ObservableFolder<Sound>)base.DeepClone();
//            SoundEvent clone = new SoundEvent() {
//                Replace = Replace,
//                Subtitle = Subtitle,
//                Files = baseClone.Files
//            };
//            clone.SetInfo(baseClone.Info.FullName);
//            clone.EventName = EventName;
//            clone.IsDirty = false;
//            return clone;
//        }

//        public override bool CopyValues(object fromCopy)
//        {
//            if (fromCopy is SoundEvent item)
//            {
//                EventName = item.EventName;
//                Replace = item.Replace;
//                Subtitle = item.Subtitle;
//                base.CopyValues(fromCopy);
//                return true;
//            }
//            return false;
//        }

//        public ValidateResult Validate()
//        {
//            string errorString = OnValidate(nameof(EventName));
//            return new ValidateResult(string.IsNullOrEmpty(errorString), errorString);
//        }

//        public event PropertyValidationEventHandler ValidateProperty;

//        string IDataErrorInfo.Error => null;
//        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);

//        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);

//        private void Init()
//        {
//            if (Count == 1)
//            {
//                EventName = FormatDottedSoundNameFromFullPath(Files[0].Info.FullName);
//            }
//            else
//            {
//                EventName = FormatDottedSoundNameFromFullPath(Info.FullName);
//            }
//            IsDirty = false;
//        }

//        // Get formatted sound from full path, "shorten.path.toFile"
//        public static string FormatDottedSoundNameFromFullPath(string path)
//        {
//            string dotterPath = Sound.FormatSoundNameFromFullPath(null, path).Remove(0, 1).Replace("/", ".");
//            return string.IsNullOrEmpty(dotterPath) ? "sounds" : dotterPath;
//        }

//        // Get formatted sound from sound path modid:shorten/path/toFile to "shorten.path.toFile"
//        public static string FormatDottedSoundNameFromSoundName(string name) => Sound.GetRelativePathFromSoundName(name).Replace("/", ".");
//    }
//}
