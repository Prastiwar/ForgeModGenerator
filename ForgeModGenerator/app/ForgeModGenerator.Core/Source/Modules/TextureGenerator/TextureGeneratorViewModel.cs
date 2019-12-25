using ForgeModGenerator.ViewModels;
using System.Threading.Tasks;

namespace ForgeModGenerator.TextureGenerator.ViewModels
{
    /// <summary> TextureGenerator Business ViewModel </summary>
    public class TextureGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<FileObject>, FileObject>
    {
        public TextureGeneratorViewModel(GeneratorContext<FileObject> context,
                                         IFoldersExplorerFactory<ObservableFolder<FileObject>, FileObject> explorerFactory) 
            : base(context, explorerFactory)
        {
            Explorer.OpenFileDialog.Filter = "Image (*.png) | *.png";
            Explorer.AllowFileExtensions(".png");
        }

        public override string DirectoryRootPath => SessionContext.SelectedMod != null ? ModPaths.TexturesFolder(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.ModInfo.Modid) : null;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                Explorer.Folders.Clear();
                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(DirectoryRootPath, true).ConfigureAwait(true)).ConfigureAwait(false);
                Explorer.FileSynchronizer.RootPath = DirectoryRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected override void RegenerateCode() { }
    }
}
