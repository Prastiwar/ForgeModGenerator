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

        public IEnumerable<SoundEvent> EnumerateSoundEvents(string path)
        {
            string folderPath = IOHelper.GetDirectoryPath(path);
            return Folders.Files.Where(folder => folder.Info.FullName.ComparePath(folderPath));
        }

        public IEnumerable<SoundEvent> EnumerateSubSoundEvents(string path)
        {
            string folderPath = IOHelper.GetDirectoryPath(path);
            return Folders.Files.Where(folder => IOHelper.IsSubPathOf(folder.Info.FullName, folderPath));
        }

        public IEnumerable<Sound> EnumerateSounds(string path)
        {
            foreach (SoundEvent soundEvent in Folders.Files)
            {
                Sound sound = soundEvent.Files.Find(x => x.Info.FullName.ComparePath(path));
                if (sound != null)
                {
                    yield return sound;
                }
            }
        }

        public IEnumerable<(SoundEvent folder, Sound file)> EnumerateSoundsWithSoundEvent(string path)
        {
            foreach (SoundEvent soundEvent in Folders.Files)
            {
                Sound sound = soundEvent.Files.Find(x => x.Info.FullName.ComparePath(path));
                if (sound != null)
                {
                    yield return (folder: soundEvent, file: sound);
                }
            }
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
            return Folders.Add(soundEvent);
        }

        /// <summary> Called when FileSystemWatcher detects folder deletion. Finds folder from path, if removes from Folders - clear it </summary>
        protected bool SyncRemoveSoundEvent(string path)
        {
            SynchronizationCheck(path);
            foreach (SoundEvent soundEvent in EnumerateSoundEvents(path).ToList())
            {
                RemoveFolder(soundEvent);
            }
            return true;
        }

        /// <summary> Called when FileSystemWatcher detects file deletion. Finds File from path and removes it from folder it belongs to </summary>
        protected bool SyncRemoveSound(string path)
        {
            SynchronizationCheck(path);
            foreach ((SoundEvent folder, Sound file) in EnumerateSoundsWithSoundEvent(path))
            {
                folder.Remove(file);
            }
            return true;
        }

        /// <summary> Called when FileSystemWatcher detects folder rename </summary>
        protected bool SyncRenameSoundEvent(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            oldPath = IOHelper.GetDirectoryPath(oldPath);
            newPath = IOHelper.GetDirectoryPath(newPath);
            foreach (SoundEvent soundEvent in EnumerateSubSoundEvents(oldPath))
            {
                string oldSubPath = soundEvent.Info.FullName;
                string newSubPath = newPath;
                if (oldSubPath.Length > newPath.Length)
                {
                    newSubPath = Path.Combine(newPath, oldSubPath.Substring(newPath.Length, oldSubPath.Length - newPath.Length));
                }
                Rename(soundEvent, newSubPath);
            }
            return true;
        }

        /// <summary> Called when FileSystemWatcher detects file rename </summary>
        protected bool SyncRenameSound(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            newPath = newPath.NormalizeFullPath();
            foreach ((SoundEvent folder, Sound file) in EnumerateSoundsWithSoundEvent(oldPath))
            {
                string oldFilePath = file.Info.FullName;
                if (Rename(file, newPath))
                {
                    if (string.Compare(folder.EventName, SoundEvent.FormatDottedSoundNameFromFullPath(oldFilePath), System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        folder.EventName = SoundEvent.FormatDottedSoundNameFromFullPath(file.Info.FullName);
                    }
                }
            }
            return true;
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
                Application.Current.Dispatcher.Invoke(() => { SyncRemoveSoundEvent(e.FullPath); });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRemoveSound(e.FullPath); });
            }
        }

        protected override void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRenameSoundEvent(e.OldFullPath, e.FullPath); });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRenameSound(e.OldFullPath, e.FullPath); });
            }
        }

        protected override void FileWatcher_SubPathRenamed(object sender, FileSubPathEventArgs e) => Application.Current.Dispatcher.Invoke(() => { SyncRenameSound(e.OldFullPath, e.FullPath); });
    }
}
