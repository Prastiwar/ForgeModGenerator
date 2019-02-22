using ForgeModGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizer : FoldersSynchronizer<SoundEvent, Sound>
    {
        public SoundEventsSynchronizer(Collection<SoundEvent> folders, string modname, string modid, string rootPath = null, string filters = null)
            : base(folders, rootPath, filters) => SetModInfo(modname, modid);

        protected string Modname { get; set; }
        protected string Modid { get; set; }

        public void SetModInfo(string modname, string modid)
        {
            Modname = modname;
            Modid = modid;
        }

        public override async Task<ObservableCollection<SoundEvent>> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return new ObservableCollection<SoundEvent>();
            }
            ObservableCollection<SoundEvent> deserializedFolders = await FindFoldersFromFile(path, false);
            bool hasNotExistingFile = deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName)));
            return hasNotExistingFile ? FilterToOnlyExistingFiles(deserializedFolders) : deserializedFolders;
        }

        protected override ObservableCollection<SoundEvent> DeserializeFolders(string fileCotent)
        {
            SoundCollectionConverter converter = new SoundCollectionConverter(Modname, Modid);
            return JsonConvert.DeserializeObject<ObservableCollection<SoundEvent>>(fileCotent, converter);
        }

        protected override SoundEvent ConstructFolderInstance(string path, IEnumerable<string> filePaths)
        {
            SoundEvent soundEvent = base.ConstructFolderInstance(path, filePaths);
            soundEvent.EventName = IOHelper.GetUniqueName(soundEvent.EventName, (name) => Folders.All(inFolder => inFolder.EventName != name));
            return soundEvent;
        }
    }
}
