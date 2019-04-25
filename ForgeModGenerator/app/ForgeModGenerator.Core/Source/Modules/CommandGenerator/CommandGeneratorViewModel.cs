using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator.CommandGenerator.ViewModels
{
    /// <summary> CommandGenerator Business ViewModel </summary>
    public class CommandGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<Command>, Command>
    {
        public CommandGeneratorViewModel(ISessionContextService sessionContext, IFoldersExplorerFactory<ObservableFolder<Command>, Command> factory) : base(sessionContext, factory)
        {
            Explorer.OpenFileDialog.Filter = "Java file (*.java) | *.java";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            Explorer.AllowedFileExtensions.Add(".java");
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null
            ? Path.GetDirectoryName(SourceCodeLocator.CustomCommand(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization, "None").FullPath)
            : null;

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
