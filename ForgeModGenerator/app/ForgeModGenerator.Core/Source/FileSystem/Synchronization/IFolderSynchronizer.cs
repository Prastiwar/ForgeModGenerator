using System;
using System.ComponentModel;

namespace ForgeModGenerator
{
    public delegate void FileChangeHandler(bool isDirectory, string path);
    public delegate void RenameHandler(bool isDirectory, string oldPath, string newPath);

    public interface IFolderSynchronizer : IDisposable
    {
        ISynchronizeInvoke SynchronizingObject { get; set; }

        event FileChangeHandler SyncChangedFile;
        event FileChangeHandler SyncCreatedFile;
        event FileChangeHandler SyncRemovedFile;
        event RenameHandler SyncRenamedFile;

        string RootPath { get; set; }
        string Filters { get; set; }
        bool IsEnabled { get; }
        bool DisableSyncWhileSyncing { get; set; }

        NotifyFilter SyncFilter { get; set; }

        void SetEnableSynchronization(bool enabled);
    }
}
