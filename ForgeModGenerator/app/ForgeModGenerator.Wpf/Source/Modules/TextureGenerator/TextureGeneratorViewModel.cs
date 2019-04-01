using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<WpfObservableFolder<FileObject>, FileObject>
    {
        public TextureGeneratorViewModel(ISessionContextService sessionContext, IDialogService dialogService, IFileSystem fileSystem) : base(sessionContext, dialogService, fileSystem)
        {
            Explorer.FoldersRootPath = FoldersRootPath;
            Explorer.OpenFileDialog = new FileBrowserDialog {
                Filter = "Image (*.png) | *.png",
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            Explorer.OpenFolderDialog = new FolderBrowserDialog {
                ShowNewFolderButton = true
            };
            Explorer.AllowedFileExtensions.Add(".png");
        }

        public string FoldersRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                if (Explorer.Folders != null)
                {
                    Explorer.Folders.Clear();
                }
                Explorer.Folders = new WpfObservableFolder<WpfObservableFolder<FileObject>>(FoldersRootPath, Enumerable.Empty<WpfObservableFolder<FileObject>>());
                Explorer.FolderFactory = new DefaultFoldersFactory<WpfObservableFolder<FileObject>, FileObject>(Explorer.AllowedFileExtensionsPatterns);
                Explorer.FileSynchronizer = new FoldersSynchronizer<WpfObservableFolder<FileObject>, FileObject>(SyncInvokeObject.Default, Explorer.Folders, Explorer.FolderFactory, FoldersRootPath, Explorer.AllowedFileExtensionsPatterns);
                Explorer.Folders.AddRange(await Explorer.FolderFactory.FindFoldersAsync(FoldersRootPath, true));
                IsLoading = false;
                return true;
            }
            return false;
        }
    }
}
