using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System.Threading.Tasks;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<FileObject>, FileObject>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext, IFoldersExplorerFactory<ObservableFolder<FileObject>, FileObject> explorerFactory) : base(sessionContext)
        {
            explorer = explorerFactory.Create();
            explorer.OpenFileDialog.Filter = "Image (*.png) | *.png";
            explorer.OpenFileDialog.Multiselect = true;
            explorer.OpenFileDialog.CheckFileExists = true;
            explorer.OpenFileDialog.ValidateNames = true;
            explorer.OpenFolderDialog.ShowNewFolderButton = true;
            explorer.AllowedFileExtensions.Add(".png");
        }

        private readonly IFoldersExplorer<ObservableFolder<FileObject>, FileObject> explorer;
        public override IFoldersExplorer<ObservableFolder<FileObject>, FileObject> Explorer => explorer;

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                explorer.Folders.Clear();
                explorer.Folders.AddRange(await explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersRootPath, true));
                explorer.FileSynchronizer.RootPath = FoldersRootPath;
                explorer.FileSynchronizer.SetEnableSynchronization(true);
                IsLoading = false;
                return true;
            }
            return false;
        }
    }
}
