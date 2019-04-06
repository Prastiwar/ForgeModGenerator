using ForgeModGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsFactory : WpfFoldersFactory<SoundEvent, Sound>
    {
        public SoundEventsFactory(ISerializer serializer) : base(serializer) { }

        protected IFolderObject<SoundEvent> SoundEvents { get; set; }
        protected string Modname { get; set; }
        protected string Modid { get; set; }

        public void Initialize(IFolderObject<SoundEvent> soundEvents, string modname, string modid)
        {
            SoundEvents = soundEvents;
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
            SoundCollectionConverter converter = new SoundCollectionConverter(Modname, Modid);
            return JsonConvert.DeserializeObject<Collection<SoundEvent>>(fileCotent, converter);
        }
        
        /// <inheritdoc/>
        public override SoundEvent Create(string path, IEnumerable<string> filePaths)
        {
            SoundEvent soundEvent = base.Create(path, filePaths);
            soundEvent.EventName = IOHelper.GetUniqueName(soundEvent.EventName, (name) => SoundEvents.Files.All(inFolder => inFolder.EventName != name));
            return soundEvent;
        }
    }
}
