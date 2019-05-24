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
        public FolderSynchronizer(ISynchronizeInvoke synchronizingObject, IFolderObject<TFolder> foldersToSync, IFoldersFinder<TFolder, TFile> finder, string rootPath = null, string filters = null)
        {
            Finder = finder;
            SyncedFolders = foldersToSync;
            Finder.Filters = filters;
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
            FileWatcher.AlwaysMonitorDirectoryChanges = true;
            FileWatcher.SynchronizingObject = synchronizingObject;
            SyncFilter = NotifyFilter.Directory | NotifyFilter.File;
            FileWatcher.FileCreated += OnFileWatcherCreated;
            FileWatcher.FileDeleted += OnFileWatcherDeleted;
            FileWatcher.FileRenamed += OnFileWatcherRenamed;
            FileWatcher.FileSubPathRenamed += OnFileWatcherSubPathRenamed;
        }

        private NotifyFilter syncFilter;
        public NotifyFilter SyncFilter {
            get => syncFilter;
            set {
                syncFilter = value;
                FileWatcher.AlwaysMonitorDirectoryChanges = SyncFilter.HasFlag(NotifyFilter.Directory);
                if (SyncFilter.HasFlag(NotifyFilter.File))
                {
                    FileWatcher.NotifyFilter |= NotifyFilters.FileName;
                }
                else
                {
                    FileWatcher.NotifyFilter &= ~NotifyFilters.FileName;
                }
            }
        }

        private IFoldersFinder<TFolder, TFile> finder;
        public IFoldersFinder<TFolder, TFile> Finder {
            get => finder;
            set => finder = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IFolderObject<TFolder> syncedFolders;
        public IFolderObject<TFolder> SyncedFolders {
            get => syncedFolders;
            set => syncedFolders = value ?? ObservableFolder<TFolder>.CreateEmpty();
        }

        public ISynchronizeInvoke SynchronizingObject {
            get => FileWatcher.SynchronizingObject;
            set => FileWatcher.SynchronizingObject = value;
        }

        public string RootPath {
            get => FileWatcher.Path;
            set => FileWatcher.Path = value;
        }

        /// <summary> Synchronized Filters (types of files separated with "|") applied Factory and FileWatcher </summary>
        public string Filters {
            get => FileWatcher.Filters;
            set {
                FileWatcher.Filters = value;
                Finder.Filters = value;
            }
        }

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        /// <summary> Is this instance synchronizing files? </summary>
        public bool IsEnabled => FileWatcher.EnableRaisingEvents;

        public void SetEnableSynchronization(bool enabled) => FileWatcher.EnableRaisingEvents = enabled;

        /// <summary> Searches root path for files that are not referenced in Folders collection and adds them </summary>
        public virtual void AddNotReferencedFiles()
        {
            IEnumerable<string> notReferencedFiles = Finder.EnumerateNotReferencedFiles(RootPath, SearchOption.AllDirectories);
            foreach (string filePath in notReferencedFiles)
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                if (!SyncedFolders.TryGetFile(dirPath, out TFolder folder))
                {
                    folder = Finder.Factory.Create(dirPath, null);
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
            SyncedFolders.Add(Finder.Factory.Create(path, null));
            if (includeSubDirectories)
            {
                IEnumerable<TFolder> foundFolders = Finder.FindFoldersFromDirectory(path);
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

        protected void OnFileWatcherCreated(object sender, FileSystemEventArgs e)
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

        protected void OnFileWatcherDeleted(object sender, FileSystemEventArgs e)
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

        protected void OnFileWatcherRenamed(object sender, RenamedEventArgs e)
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

        protected void OnFileWatcherSubPathRenamed(object sender, FileSubPathEventArgs e) => SyncRenameFile(e.OldFullPath, e.FullPath);

        /// <summary> Throws exception if given path is not sub path of RootPath </summary>
        protected void SynchronizationCheck(string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, RootPath))
            {
                throw new InvalidSynchronizationArgumentException(RootPath, actualPath);
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileWatcher.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
