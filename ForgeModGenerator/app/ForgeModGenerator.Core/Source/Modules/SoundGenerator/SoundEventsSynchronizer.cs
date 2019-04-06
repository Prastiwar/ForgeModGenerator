using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.Utility;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEventsSynchronizer : FolderSynchronizer<SoundEvent, Sound>
    {
        public SoundEventsSynchronizer(ISynchronizeInvoke synchronizeObject, IFolderObject<SoundEvent> foldersToSync, IFoldersFactory<SoundEvent, Sound> factory, string rootPath = null, string filters = null)
            : base(synchronizeObject, foldersToSync, factory, rootPath, filters) { }

        /// <inheritdoc/>
        public override void AddNotReferencedFiles()
        {
            string[] filePaths = new string[1];
            foreach (string filePath in Factory.EnumerateNotReferencedFiles(RootPath, SearchOption.AllDirectories))
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                filePaths[0] = filePath;
                SoundEvent folder = Factory.Create(dirPath, filePaths);
                SyncedFolders.Add(folder);
            }
        }

        /// <inheritdoc/>
        protected override bool SyncCreateFile(string path)
        {
            SynchronizationCheck(path);
            SoundEvent soundEvent = Factory.Create(path, new string[] { path });
            return SyncedFolders.Add(soundEvent);
        }

        /// <inheritdoc/>
        protected override bool SyncRemoveFolder(string path)
        {
            SynchronizationCheck(path);
            foreach (SoundEvent soundEvent in SyncedFolders.EnumerateFiles(path).ToList())
            {
                RemoveSyncedFolder(soundEvent);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool SyncRemoveFile(string path)
        {
            SynchronizationCheck(path);
            foreach ((SoundEvent folder, Sound file) in SyncedFolders.EnumerateFolderFilesExtended<SoundEvent, Sound>(path))
            {
                folder.Remove(file);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool SyncRenameFolder(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            oldPath = IOHelper.GetDirectoryPath(oldPath);
            newPath = IOHelper.GetDirectoryPath(newPath);
            foreach (SoundEvent soundEvent in SyncedFolders.EnumerateSubPathFiles(oldPath))
            {
                string oldSubPath = soundEvent.Info.FullName;
                string newSubPath = newPath;
                if (oldSubPath.Length > newPath.Length)
                {
                    newSubPath = Path.Combine(newPath, oldSubPath.Substring(newPath.Length, oldSubPath.Length - newPath.Length));
                }
                RenameInfo(soundEvent, newSubPath);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool SyncRenameFile(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            newPath = newPath.NormalizeFullPath();
            foreach ((SoundEvent folder, Sound file) in SyncedFolders.EnumerateFolderFilesExtended<SoundEvent, Sound>(oldPath))
            {
                string oldFilePath = file.Info.FullName;
                if (RenameInfo(file, newPath))
                {
                    if (string.Compare(folder.EventName, SoundEvent.FormatDottedSoundNameFromFullPath(oldFilePath), System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        folder.EventName = SoundEvent.FormatDottedSoundNameFromFullPath(file.Info.FullName);
                    }
                }
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool SyncCreateFolder(string path, bool includeSubDirectories = true) => false;
    }
}
