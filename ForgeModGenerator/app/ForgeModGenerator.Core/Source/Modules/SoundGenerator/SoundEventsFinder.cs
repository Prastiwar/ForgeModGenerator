using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsFinder : DefaultFoldersFinder<SoundEvent, Sound>
    {
        public SoundEventsFinder(ISoundEventsSerializer serializer, IFoldersFactory<SoundEvent, Sound> factory) : base(serializer, factory) { }

        protected string Modname { get; set; }
        protected string Modid { get; set; }

        public void Initialize(string modname, string modid)
        {
            Modname = modname;
            Modid = modid;
        }

        /// <inheritdoc/>
        public override IEnumerable<SoundEvent> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return Enumerable.Empty<SoundEvent>();
            }
            IEnumerable<SoundEvent> deserializedFolders = FindFoldersFromFile(path, false);
            bool hasNotExistingFile = deserializedFolders != null ? deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName))) : false;
            return hasNotExistingFile ? FilterToOnlyExistingFiles(deserializedFolders) : deserializedFolders;
        }

        /// <inheritdoc/>
        protected override ICollection<SoundEvent> DeserializeFolders(string fileCotent)
        {
            ((ISoundEventsSerializer)Serializer).SetModInfo(Modname, Modid);
            return Serializer.DeserializeObject<ICollection<SoundEvent>>(fileCotent);
        }
    }
}
