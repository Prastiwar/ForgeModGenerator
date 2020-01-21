//using ForgeModGenerator.Models;
//using ForgeModGenerator.Validation;
//using System.ComponentModel;
//using System.IO;

//namespace ForgeModGenerator.SoundGenerator.Models
//{
//    public class Sound : FileObject
//    {
//        public enum SoundType { file, @event }

//        protected Sound() => PropertyChanged += OnSoundPropertyChanged;

//        public Sound(string filePath) : this(McMod.GetModidFromPath(filePath), filePath) { }

//        public Sound(string modid, string filePath) : this()
//        {
//            this.modid = modid ?? throw new System.ArgumentNullException(nameof(modid));

//            if (Utility.IOHelper.IsPathValid(filePath))
//            {
//                Name = FormatSoundNameFromFullPath(modid, filePath);
//                SetInfo(filePath);
//            }
//            else
//            {
//                Name = filePath;
//            }
//            IsDirty = false;
//        }

//        private readonly string modid;

//        private string name;
//        public string Name {
//            get => name;
//            private set => DirtSetProperty(ref name, value);
//        }

//        private float volume = 1.0f;
//        public float Volume {
//            get => volume;
//            set => DirtSetProperty(ref volume, Math.Clamp(value));
//        }

//        private float pitch = 1.0f;
//        public float Pitch {
//            get => pitch;
//            set => DirtSetProperty(ref pitch, value);
//        }

//        private int weight = 1;
//        public int Weight {
//            get => weight;
//            set => DirtSetProperty(ref weight, value);
//        }

//        private bool stream = false;
//        public bool Stream {
//            get => stream;
//            set => DirtSetProperty(ref stream, value);
//        }

//        private int attenuationDistance;
//        public int AttenuationDistance {
//            get => attenuationDistance;
//            set => DirtSetProperty(ref attenuationDistance, value);
//        }

//        private bool preload = false;
//        public bool Preload {
//            get => preload;
//            set => DirtSetProperty(ref preload, value);
//        }

//        private SoundType type = SoundType.file;        
//        public SoundType Type {
//            get => type;
//            set => DirtSetProperty(ref type, value);
//        }

//        internal string GetSoundsFolder() => ModPaths.SoundsFolder(McMod.GetModnameFromPath(Info.FullName), modid);

//        public void FormatName() => Name = FormatSoundNameFromFullPath(modid, Info.FullName);

//        protected override void OnInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == nameof(FileSystemInfoReference.FullName))
//            {
//                FileSystemInfoReference infoRef = (FileSystemInfoReference)sender;
//                if (infoRef.IsValidReference)
//                {
//                    FormatName();
//                    IsDirty = false;
//                }
//            }
//        }

//        protected virtual void OnSoundPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == nameof(Info))
//            {
//                Sound sound = (Sound)sender;
//                if (sound.Info.IsValidReference)
//                {
//                    FormatName();
//                    IsDirty = false;
//                }
//            }
//        }

//        public override object DeepClone()
//        {
//            Sound sound = new Sound(modid, name) {
//                Volume = Volume,
//                Pitch = Pitch,
//                Weight = Weight,
//                Stream = Stream,
//                AttenuationDistance = AttenuationDistance,
//                Preload = Preload,
//                Type = Type
//            };
//            sound.SetInfo(Info.FullName);
//            sound.IsDirty = false;
//            return sound;
//        }

//        public override bool CopyValues(object fromCopy)
//        {
//            if (fromCopy is Sound sound)
//            {
//                Name = sound.Name;
//                Volume = sound.Volume;
//                Pitch = sound.Pitch;
//                Weight = sound.Weight;
//                Stream = sound.Stream;
//                AttenuationDistance = sound.AttenuationDistance;
//                Preload = sound.Preload;
//                Type = sound.Type;
//                base.CopyValues(fromCopy);
//                IsDirty = false;
//                return true;
//            }
//            return false;
//        }

//        /// <summary> Get modid from sound name "modid:shortPath"  </summary>
//        public static string GetModidFromSoundName(string name) => name.Substring(0, name.IndexOf(":"));

//        /// <summary> Get "shortpath" if path is formatted as "modid:shortpath" </summary>
//        public static string GetRelativePathFromSoundName(string name) => name.Remove(0, name.IndexOf(":") + 1);

//        /// <summary> Get formatted sound from short path, "modid:shortPath" </summary>
//        public static string FormatSoundName(string modid, string shortPath) => $"{modid}:{shortPath}";

//        /// <summary> Get formatted sound from full path, "modid:shorten/path/toFile" </summary>
//        public static string FormatSoundNameFromFullPath(string modid, string path)
//        {
//            string shortPath = FormatSoundRelativePath(path);
//            return $"{modid}:{shortPath}";
//        }

//        /// <summary> Get formatted sound from full path, "shorten/path/toFile" </summary>
//        public static string FormatSoundRelativePath(string fullPath)
//        {
//            int startIndex = fullPath.IndexOf("sounds") + 7;
//            if (startIndex == -1 || startIndex >= fullPath.Length)
//            {
//                return null;
//            }
//            return Path.ChangeExtension(fullPath.Substring(startIndex, fullPath.Length - startIndex), null).Replace("\\", "/");
//        }
//    }
//}
