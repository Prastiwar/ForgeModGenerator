using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.SoundGenerator.Models
{
    [JsonConverter(typeof(SoundConverter))]
    public class Sound : FileItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SoundType { file, @event }

        protected Sound() => PropertyChanged += Sound_PropertyChanged;

        /// <summary> IMPORTANT: Prefer to use ctor, this is used for serialization purposes </summary>
        internal static Sound CreateEmpty(string name = null, string modid = null) => new Sound() { Name = name, modid = modid };

        public Sound(string filePath) : this(Mod.GetModidFromPath(filePath), filePath) { }

        public Sound(string modid, string filePath) : this()
        {
            this.modid = modid ?? throw new System.ArgumentNullException(nameof(modid));

            if (File.Exists(filePath))
            {
                Name = FormatSoundNameFromFullPath(modid, filePath);
                SetInfo(filePath);
            }
            else
            {
                Name = filePath;
            }
            IsDirty = false;
        }

        private string modid;

        private string name;
        public string Name {
            get => name;
            private set => DirtSet(ref name, value);
        }

        private float volume = 1.0f;
        public float Volume {
            get => volume;
            set => DirtSet(ref volume, Math.Clamp(value));
        }

        private float pitch = 1.0f;
        public float Pitch {
            get => pitch;
            set => DirtSet(ref pitch, value);
        }

        private int weight = 1;
        public int Weight {
            get => weight;
            set => DirtSet(ref weight, value);
        }

        private bool stream = false;
        public bool Stream {
            get => stream;
            set => DirtSet(ref stream, value);
        }

        private int attenuationDistance;
        public int AttenuationDistance {
            get => attenuationDistance;
            set => DirtSet(ref attenuationDistance, value);
        }

        private bool preload = false;
        public bool Preload {
            get => preload;
            set => DirtSet(ref preload, value);
        }

        private SoundType type = SoundType.file;
        public SoundType Type {
            get => type;
            set => DirtSet(ref type, value);
        }

        internal string GetSoundsFolder() => ModPaths.SoundsFolder(Mod.GetModnameFromPath(Info.FullName), modid);

        public void FormatName() => Name = FormatSoundNameFromFullPath(modid, Info.FullName);

        protected override void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileSystemInfoReference.FullName))
            {
                FormatName();
                IsDirty = false;
            }
        }

        protected virtual void Sound_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Info))
            {
                FormatName();
                IsDirty = false;
            }
        }

        public override object DeepClone()
        {
            Sound sound = new Sound() {
                modid = modid,
                Name = Name,
                Volume = Volume,
                Pitch = Pitch,
                Weight = Weight,
                Stream = Stream,
                AttenuationDistance = AttenuationDistance,
                Preload = Preload,
                Type = Type
            };
            sound.SetInfo(Info.FullName);
            return sound;
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Sound sound)
            {
                modid = sound.modid;
                Name = sound.Name;
                Volume = sound.Volume;
                Pitch = sound.Pitch;
                Weight = sound.Weight;
                Stream = sound.Stream;
                AttenuationDistance = sound.AttenuationDistance;
                Preload = sound.Preload;
                Type = sound.Type;

                base.CopyValues(fromCopy);
                IsDirty = false;
                return true;
            }
            return false;
        }

        /// <summary> Get modid from sound name "modid:shortPath"  </summary>
        public static string GetModidFromSoundName(string name) => name.Substring(0, name.IndexOf(":"));

        /// <summary> Get "shortpath" if path is formatted as "modid:shortpath" </summary>
        public static string GetRelativePathFromSoundName(string name) => name.Remove(0, name.IndexOf(":") + 1);

        /// <summary> Get formatted sound from short path, "modid:shortPath" </summary>
        public static string FormatSoundName(string modid, string shortPath) => $"{modid}:{shortPath}";

        /// <summary> Get formatted sound from full path, "modid:shorten/path/toFile" </summary>
        public static string FormatSoundNameFromFullPath(string modid, string path)
        {
            string shortPath = FormatSoundRelativePath(path);
            return $"{modid}:{shortPath}";
        }

        /// <summary> Get formatted sound from full path, "shorten/path/toFile" </summary>
        public static string FormatSoundRelativePath(string fullPath)
        {
            int startIndex = fullPath.IndexOf("sounds") + 7;
            if (startIndex == -1)
            {
                return null;
            }
            return Path.ChangeExtension(fullPath.Substring(startIndex, fullPath.Length - startIndex), null).Replace("\\", "/");
        }
    }
}
