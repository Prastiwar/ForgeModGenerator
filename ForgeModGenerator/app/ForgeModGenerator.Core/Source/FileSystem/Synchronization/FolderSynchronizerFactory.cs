using System.ComponentModel;

namespace ForgeModGenerator
{
    public class FolderSynchronizerFactory<TFolder, TFile> : IFolderSynchronizerFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FolderSynchronizerFactory(IFoldersFinder<TFolder, TFile> finder, ISynchronizeInvoke synchronizingObject)
        {
            this.finder = finder;
            this.synchronizingObject = synchronizingObject;
        }

        private readonly IFoldersFinder<TFolder, TFile> finder;
        private readonly ISynchronizeInvoke synchronizingObject;

        public IFolderSynchronizer<TFolder, TFile> Create() => new FolderSynchronizer<TFolder, TFile>(synchronizingObject, null, finder);

        public IFolderSynchronizer<TFolder, TFile> Create(IFolderObject<TFolder> foldersToSync, string rootPath = null, string filters = null) => 
            new FolderSynchronizer<TFolder, TFile>(synchronizingObject, foldersToSync, finder, rootPath, filters);
    }
}
