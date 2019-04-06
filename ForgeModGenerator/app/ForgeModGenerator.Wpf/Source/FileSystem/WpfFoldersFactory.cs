using ForgeModGenerator.Serialization;

namespace ForgeModGenerator
{
    public class WpfFoldersFactory<TFolder, TFile> : DefaultFoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public WpfFoldersFactory(ISerializer serializer) : base(serializer) { }
        
        public override IFolderObject<TFolder> CreateFolders() => WpfObservableFolder<TFolder>.CreateEmpty();
    }
}
