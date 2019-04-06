namespace ForgeModGenerator
{
    public interface IFoldersExplorerFactory<TFolder, TFile>
            where TFolder : class, IFolderObject<TFile>
            where TFile : class, IFileObject
    {
        IFoldersExplorer<TFolder, TFile> Create();
    }
}
