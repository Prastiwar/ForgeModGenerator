using System.ComponentModel;

namespace ForgeModGenerator
{
    public class FolderSynchronizerFactory<TFolder, TFile> : IFolderSynchronizerFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FolderSynchronizerFactory(IFoldersFactory<TFolder, TFile> foldersFactory, ISynchronizeInvoke synchronizingObject)
        {
            this.foldersFactory = foldersFactory;
            this.synchronizingObject = synchronizingObject;
        }

        private readonly IFoldersFactory<TFolder, TFile> foldersFactory;
        private readonly ISynchronizeInvoke synchronizingObject;

        public IFolderSynchronizer<TFolder, TFile> Create() => new FolderSynchronizer<TFolder, TFile>(synchronizingObject, null, foldersFactory);

        public IFolderSynchronizer<TFolder, TFile> Create(IFolderObject<TFolder> foldersToSync, string rootPath = null, string filters = null) => 
            new FolderSynchronizer<TFolder, TFile>(synchronizingObject, foldersToSync, foldersFactory, rootPath, filters);
    }
}
