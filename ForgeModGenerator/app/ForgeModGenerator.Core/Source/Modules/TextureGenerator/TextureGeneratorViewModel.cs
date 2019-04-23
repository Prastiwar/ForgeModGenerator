using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System.Threading.Tasks;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<FileObject>, FileObject>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext, IFoldersExplorerFactory<ObservableFolder<FileObject>, FileObject> explorerFactory) : base(sessionContext, explorerFactory)
        {
            Explorer.OpenFileDialog.Filter = "Image (*.png) | *.png";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            Explorer.AllowedFileExtensions.Add(".png");
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                Explorer.Folders.Clear();
                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersRootPath, true).ConfigureAwait(true)).ConfigureAwait(false);
                Explorer.FileSynchronizer.RootPath = FoldersRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                IsLoading = false;
                return true;
            }
            return false;
        }
    }
}
