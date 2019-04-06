using ForgeModGenerator.Exceptions;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator
{
    public class FolderSynchronizer<TFolder, TFile> : IFolderSynchronizer<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FolderSynchronizer(ISynchronizeInvoke synchronizingObject, IFolderObject<TFolder> foldersToSync, IFoldersFactory<TFolder, TFile> factory, string rootPath = null, string filters = null)
        {
            this.rootPath = rootPath;
            this.filters = filters;
            this.synchronizingObject = synchronizingObject;
            Factory = factory;
            SyncedFolders = foldersToSync;

            if (string.IsNullOrEmpty(rootPath))
            {
                FileWatcher = new FileSystemWatcherExtended() {
                    Filters = filters
                };
            }
            else
            {
                FileWatcher = new FileSystemWatcherExtended(RootPath, Filters);
            }
            FileWatcher.IncludeSubdirectories = true;
            FileWatcher.MonitorDirectoryChanges = true;
            FileWatcher.SynchronizingObject = synchronizingObject;
            FileWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
            FileWatcher.FileCreated += FileWatcher_Created;
            FileWatcher.FileDeleted += FileWatcher_Deleted;
            FileWatcher.FileRenamed += FileWatcher_Renamed;
            FileWatcher.FileSubPathRenamed += FileWatcher_SubPathRenamed;
        }

        private IFoldersFactory<TFolder, TFile> factory;
        public IFoldersFactory<TFolder, TFile> Factory {
            get => factory;
            set => factory = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IFolderObject<TFolder> syncedFolders;
        public IFolderObject<TFolder> SyncedFolders {
            get => syncedFolders;
            set => syncedFolders = value ?? ObservableFolder<TFolder>.CreateEmpty();
        }

        private ISynchronizeInvoke synchronizingObject;
        public ISynchronizeInvoke SynchronizingObject{
            get => synchronizingObject;
            set {
                synchronizingObject = value;
                FileWatcher.SynchronizingObject = synchronizingObject;
            }
        }

        private string rootPath;
        public string RootPath {
            get => rootPath;
            set {
                rootPath = value;
                FileWatcher.Path = RootPath;
            }
        }

        private string filters;
        public string Filters {
            get => filters;
            set {
                filters = value;
                FileWatcher.Filters = Filters;
                Factory.Filters = Filters;
            }
        }

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        public bool IsEnabled => FileWatcher.EnableRaisingEvents;
        public void SetEnableSynchronization(bool enabled) => FileWatcher.EnableRaisingEvents = enabled;

        /// <summary> Searches root path for files that are not referenced in Folders collection and adds them </summary>
        public virtual void AddNotReferencedFiles()
        {
            IEnumerable<string> notReferencedFiles = Factory.EnumerateNotReferencedFiles(RootPath, SearchOption.AllDirectories);
            foreach (string filePath in notReferencedFiles)
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                if (!SyncedFolders.TryGetFile(dirPath, out TFolder folder))
                {
                    folder = Factory.Create(dirPath, null);
                    SyncedFolders.Add(folder);
                }
                List<string> notReferencedFilesForFolder = notReferencedFiles.Where(path => IOHelper.GetDirectoryPath(path) == folder.Info.FullName).ToList();
                folder.AddRange(notReferencedFilesForFolder);
                notReferencedFiles = notReferencedFiles.Except(notReferencedFilesForFolder);
            }
        }

        protected bool RenameInfo(IFileSystemObject info, string newPath)
        {
            if (!info.Info.FullName.ComparePath(newPath))
            {
                info.SetInfo(newPath);
                return true;
            }
            return false;
        }

        protected bool RemoveSyncedFolder(TFolder folder)
        {
            if (SyncedFolders.Remove(folder))
            {
                folder.Clear();
                return true;
            }
            return false;
        }

        /// <summary> Called when FileSystemWatcher detects folder creation </summary>
        protected virtual bool SyncCreateFolder(string path, bool includeSubDirectories = true)
        {
            SynchronizationCheck(path);
            SyncedFolders.Add(Factory.Create(path, null));
            if (includeSubDirectories)
            {
                IEnumerable<TFolder> foundFolders = Factory.FindFoldersFromDirectory(path);
                if (foundFolders.Any())
                {
                    SyncedFolders.AddRange(foundFolders);
                }
            }
            return true;
        }

        /// <summary> Called when FileSystemWatcher detects file creation. Finds folder from path and adds path to it </summary>
        protected virtual bool SyncCreateFile(string path)
        {
            SynchronizationCheck(path);
            string folderPath = IOHelper.GetDirectoryPath(path);
            return SyncedFolders.TryGetFile(IOHelper.GetDirectoryPath(folderPath), out TFolder folder) ? folder.Add(path) : false;
        }

        /// <summary> Called when FileSystemWatcher detects folder deletion. Finds folder from path, if removes from Folders - clear it </summary>
        protected virtual bool SyncRemoveFolder(string path)
        {
            SynchronizationCheck(path);
            return SyncedFolders.TryGetFile(IOHelper.GetDirectoryPath(path), out TFolder folder) ? RemoveSyncedFolder(folder) : false;
        }

        /// <summary> Called when FileSystemWatcher detects file deletion. Finds File from path and removes it from folder it belongs to </summary>
        protected virtual bool SyncRemoveFile(string path)
        {
            SynchronizationCheck(path);
            return SyncedFolders.TryGetFolderFile(path, out TFile file, out TFolder folder) ? folder.Remove(file) : false;
        }

        /// <summary> Called when FileSystemWatcher detects folder rename </summary>
        protected virtual bool SyncRenameFolder(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            string oldFolderPath = IOHelper.GetDirectoryPath(oldPath);
            newPath = IOHelper.GetDirectoryPath(newPath).NormalizeFullPath();
            return SyncedFolders.TryGetFile(IOHelper.GetDirectoryPath(oldFolderPath), out TFolder oldFolder) ? RenameInfo(oldFolder, newPath) : false;
        }

        /// <summary> Called when FileSystemWatcher detects file rename </summary>
        protected virtual bool SyncRenameFile(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            return SyncedFolders.TryGetFolderFile(oldPath, out TFile file) ? RenameInfo(file, newPath.NormalizeFullPath()) : false;
        }

        protected void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                SyncCreateFolder(e.FullPath, true);
            }
            else // is file
            {
                SyncCreateFile(e.FullPath);
            }
        }

        protected void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                SyncRemoveFolder(e.FullPath);
            }
            else // is file
            {
                SyncRemoveFile(e.FullPath);
            }
        }

        protected void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                SyncRenameFolder(e.OldFullPath, e.FullPath);
            }
            else // is file
            {
                SyncRenameFile(e.OldFullPath, e.FullPath);
            }
        }

        protected void FileWatcher_SubPathRenamed(object sender, FileSubPathEventArgs e) => SyncRenameFile(e.OldFullPath, e.FullPath);

        /// <summary> Throws exception if given path is not sub path of RootPath </summary>
        protected void SynchronizationCheck(string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, RootPath))
            {
                throw new InvalidSynchronizationArgumentException(RootPath, actualPath);
            }
        }

        public void Dispose() => FileWatcher.Dispose();
    }
}
