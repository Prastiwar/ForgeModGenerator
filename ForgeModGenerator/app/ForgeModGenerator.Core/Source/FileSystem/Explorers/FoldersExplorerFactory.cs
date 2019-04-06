using ForgeModGenerator.Services;

namespace ForgeModGenerator
{
    public class FoldersExplorerFactory<TFolder, TFile> : IFoldersExplorerFactory<TFolder, TFile>
            where TFolder : class, IFolderObject<TFile>
            where TFile : class, IFileObject
    {
        public FoldersExplorerFactory(IDialogService dialogService, IFileSystem fileSystem, IFolderSynchronizerFactory<TFolder, TFile> synchronizer)
        {
            this.dialogService = dialogService;
            this.fileSystem = fileSystem;
            this.synchronizer = synchronizer;
        }

        private readonly IDialogService dialogService;
        private readonly IFileSystem fileSystem;
        private readonly IFolderSynchronizerFactory<TFolder, TFile> synchronizer;

        public IFoldersExplorer<TFolder, TFile> Create() => new FoldersExplorer<TFolder, TFile>(dialogService, fileSystem, synchronizer.Create());
    }
}
