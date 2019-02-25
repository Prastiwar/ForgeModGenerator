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
    public class SoundEventsSynchronizer : FoldersSynchronizer<SoundEvent, Sound>
    {
        public SoundEventsSynchronizer(ObservableFolder<SoundEvent> folders, string modname, string modid, string rootPath = null, string filters = null)
            : base(folders, rootPath, filters) => SetModInfo(modname, modid);

        protected string Modname { get; set; }
        protected string Modid { get; set; }

        public void SetModInfo(string modname, string modid)
        {
            Modname = modname;
            Modid = modid;
        }

        public override IEnumerable<SoundEvent> GetFolders(string path, bool createRootIfEmpty = false)
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

        protected override ICollection<SoundEvent> DeserializeFolders(string fileCotent)
        {
            SoundCollectionConverter converter = new SoundCollectionConverter(Modname, Modid);
            return JsonConvert.DeserializeObject<Collection<SoundEvent>>(fileCotent, converter);
        }

        protected override SoundEvent ConstructFolderInstance(string path, IEnumerable<string> filePaths)
        {
            SoundEvent soundEvent = base.ConstructFolderInstance(path, filePaths);
            soundEvent.EventName = IOHelper.GetUniqueName(soundEvent.EventName, (name) => Folders.Files.All(inFolder => inFolder.EventName != name));
            return soundEvent;
        }
    }
}
