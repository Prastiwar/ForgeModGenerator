using System;
using System.ComponentModel;

namespace ForgeModGenerator
{
    public interface IFolderSynchronizer<TFolder, TFile> : IDisposable
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFoldersFinder<TFolder, TFile> Finder { get; }
        ISynchronizeInvoke SynchronizingObject { get; set; }
        IFolderObject<TFolder> SyncedFolders { get; set; }

        string RootPath { get; set; }
        string Filters { get; set; }

        void SetEnableSynchronization(bool enabled);
        void AddNotReferencedFiles();
    }
}
