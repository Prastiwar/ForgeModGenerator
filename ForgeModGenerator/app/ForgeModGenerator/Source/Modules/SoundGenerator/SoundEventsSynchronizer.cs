using ForgeModGenerator.SoundGenerator.Converters;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace ForgeModGenerator.SoundGenerator
{
    public class ObservableSoundEvents : ObservableFolder<SoundEvent>
    {
        public ObservableSoundEvents(string path) : base(path) { }
        public ObservableSoundEvents(IEnumerable<string> filePaths) : base(filePaths) { }
        public ObservableSoundEvents(IEnumerable<SoundEvent> files) : base(files) { }
        public ObservableSoundEvents(string path, IEnumerable<string> filePaths) : base(path, filePaths) { }
        public ObservableSoundEvents(string path, IEnumerable<SoundEvent> files) : base(path, files) { }
        public ObservableSoundEvents(string path, SearchOption searchOption) : base(path, searchOption) { }
        public ObservableSoundEvents(string path, string fileSearchPatterns) : base(path, fileSearchPatterns) { }
        public ObservableSoundEvents(string path, string fileSearchPatterns, SearchOption searchOption) : base(path, fileSearchPatterns, searchOption) { }
        protected ObservableSoundEvents() { }

        protected override bool CanAdd(SoundEvent item) => !Files.Exists(x => item.EventName == x.EventName);
        protected override bool CanAdd(string filePath) => true;
    }

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

        public override IEnumerable<SoundEvent> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!File.Exists(path))
            {
                File.AppendAllText(path, "{}");
                return Enumerable.Empty<SoundEvent>();
            }
            IEnumerable<SoundEvent> deserializedFolders = GetFoldersFromFile(path, false);
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

        protected bool SyncCreateSoundFile(string path)
        {
            SynchronizationCheck(path);
            SoundEvent soundEvent = ConstructFolderInstance(path, new string[] { path });
            //soundEvent.Add(path);
            return Folders.Add(soundEvent);
        }

        protected override void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsFilePath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { SyncCreateSoundFile(e.FullPath); });
            }
        }

        protected override void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { while (SyncRemoveFolder(e.FullPath)) { } });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { while (SyncRemoveFile(e.FullPath)) { } });
            }
        }

        protected override void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { while (SyncRenameFolder(e.OldFullPath, e.FullPath)) { } });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { while (SyncRenameFile(e.OldFullPath, e.FullPath)) { } });
            }
        }
    }
}
