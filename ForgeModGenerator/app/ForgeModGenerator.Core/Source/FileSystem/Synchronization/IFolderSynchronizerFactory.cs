namespace ForgeModGenerator
{
    public interface IFolderSynchronizerFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFolderSynchronizer<TFolder, TFile> Create();
    }
}
