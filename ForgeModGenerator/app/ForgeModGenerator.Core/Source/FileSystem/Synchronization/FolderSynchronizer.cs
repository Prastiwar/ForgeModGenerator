using ForgeModGenerator.Utility;
using System;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator
{
    public class FolderSynchronizer : IFolderSynchronizer
    {
        public FolderSynchronizer(ISynchronizeInvoke synchronizingObject, string rootPath = null, string filters = null)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                FileWatcher = new FileSystemWatcherExtended() {
                    Filters = filters
                };
            }
            else
            {
                FileWatcher = new FileSystemWatcherExtended(rootPath, filters);
            }
            FileWatcher.IncludeSubdirectories = true;
            FileWatcher.AlwaysMonitorDirectoryChanges = true;
            FileWatcher.SynchronizingObject = synchronizingObject;
            SyncFilter = NotifyFilter.Directory | NotifyFilter.File;

            FileWatcher.FileCreated += OnFileWatcherCreated;
            FileWatcher.FileDeleted += OnFileWatcherDeleted;
            FileWatcher.FileRenamed += OnFileWatcherRenamed;
            FileWatcher.FileSubPathRenamed += OnFileWatcherSubPathRenamed;
            FileWatcher.FileChanged += OnFileWatcherChanged;
        }

        private event FileChangeHandler syncChangedFile;
        public event FileChangeHandler SyncChangedFile {
            add => syncChangedFile += value;
            remove => syncChangedFile -= value;
        }

        private event FileChangeHandler syncCreatedFile;
        public event FileChangeHandler SyncCreatedFile {
            add => syncCreatedFile += value;
            remove => syncCreatedFile -= value;
        }

        private event FileChangeHandler syncRemovedFile;
        public event FileChangeHandler SyncRemovedFile {
            add => syncRemovedFile += value;
            remove => syncRemovedFile -= value;
        }

        private event RenameHandler syncRenamedFile;
        public event RenameHandler SyncRenamedFile {
            add => syncRenamedFile += value;
            remove => syncRenamedFile -= value;
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
            set => FileWatcher.Filters = value;
        }

        /// <summary> Should SetEnableSynchronization(false) while sync event is raising? </summary>
        public bool DisableSyncWhileSyncing { get; set; }

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        /// <summary> Is this instance synchronizing files? </summary>
        public bool IsEnabled => FileWatcher.EnableRaisingEvents;

        public void SetEnableSynchronization(bool enabled) => FileWatcher.EnableRaisingEvents = enabled;

        /// <summary> Called when FileSystemWatcher detects file changes </summary>
        protected void OnFileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            SynchronizationCheck(e.FullPath);
            bool wasEnabled = IsEnabled;
            if (DisableSyncWhileSyncing)
            {
                SetEnableSynchronization(false);
            }
            syncChangedFile?.Invoke(false, e.FullPath);
            if (wasEnabled != IsEnabled)
            {
                SetEnableSynchronization(true);
            }
        }

        /// <summary> Called when FileSystemWatcher detects file creation </summary>
        protected void OnFileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            SynchronizationCheck(e.FullPath);
            bool wasEnabled = IsEnabled;
            if (DisableSyncWhileSyncing)
            {
                SetEnableSynchronization(false);
            }
            bool isDirectory = IOHelper.IsDirectoryPath(e.FullPath);
            syncCreatedFile?.Invoke(isDirectory, e.FullPath);
            if (wasEnabled != IsEnabled)
            {
                SetEnableSynchronization(true);
            }
        }

        /// <summary> Called when FileSystemWatcher detects file deletion </summary>
        protected void OnFileWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            SynchronizationCheck(e.FullPath);

            bool wasEnabled = IsEnabled;
            if (DisableSyncWhileSyncing)
            {
                SetEnableSynchronization(false);
            }
            bool isDirectory = IOHelper.IsDirectoryPath(e.FullPath);
            syncRemovedFile?.Invoke(isDirectory, e.FullPath);
            if (wasEnabled != IsEnabled)
            {
                SetEnableSynchronization(true);
            }
        }

        /// <summary> Called when FileSystemWatcher detects file rename </summary>
        protected void OnFileWatcherRenamed(object sender, RenamedEventArgs e)
        {
            SynchronizationCheck(e.FullPath);
            bool wasEnabled = IsEnabled;
            if (DisableSyncWhileSyncing)
            {
                SetEnableSynchronization(false);
            }
            bool isDirectory = IOHelper.IsDirectoryPath(e.FullPath);
            syncRenamedFile?.Invoke(isDirectory, e.OldFullPath, e.FullPath);
            if (wasEnabled != IsEnabled)
            {
                SetEnableSynchronization(true);
            }
        }

        protected void OnFileWatcherSubPathRenamed(object sender, FileSubPathEventArgs e)
        {
            SynchronizationCheck(e.FullPath);
            bool wasEnabled = IsEnabled;
            if (DisableSyncWhileSyncing)
            {
                SetEnableSynchronization(false);
            }
            syncRenamedFile(false, e.OldFullPath, e.FullPath);
            if (wasEnabled != IsEnabled)
            {
                SetEnableSynchronization(true);
            }
        }

        /// <summary> Throws exception if given path is not sub path of RootPath </summary>
        protected void SynchronizationCheck(string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, RootPath))
            {
                throw new ArgumentException($"Invalid synchronization argument: {actualPath} is not subpath of {RootPath}");
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileWatcher.FileCreated -= OnFileWatcherCreated;
                    FileWatcher.FileDeleted -= OnFileWatcherDeleted;
                    FileWatcher.FileRenamed -= OnFileWatcherRenamed;
                    FileWatcher.FileSubPathRenamed -= OnFileWatcherSubPathRenamed;
                    FileWatcher.FileChanged -= OnFileWatcherChanged;
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
