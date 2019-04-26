using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.CommandGenerator.ViewModels
{
    /// <summary> CommandGenerator Business ViewModel </summary>
    public class CommandGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<Command>, Command>
    {
        public CommandGeneratorViewModel(ISessionContextService sessionContext,
                                        IFoldersExplorerFactory<ObservableFolder<Command>, Command> factory,
                                        IEditorFormFactory<Command> editorFormFactory,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext, factory)
        {
            Explorer.OpenFileDialog.Filter = "Java file (*.java) | *.java";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            Explorer.AllowedFileExtensions.Add(".java");
            EditorForm = editorFormFactory.Create();
            EditorForm.ItemEdited += CreateCommand;
            CodeGenerationService = codeGenerationService;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null
            ? Path.GetDirectoryName(SourceCodeLocator.CustomCommand(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization, "None").FullPath)
            : null;

        protected ICodeGenerationService CodeGenerationService { get; }
        protected IEditorForm<Command> EditorForm { get; }

        private ICommand openCommandEditor;
        public ICommand OpenCommandEditor => openCommandEditor ?? (openCommandEditor = new DelegateCommand<ObservableFolder<Command>>(CreateNewCommand));

        private string tempFilePath;

        private void CreateNewCommand(ObservableFolder<Command> folder)
        {
            tempFilePath = Path.GetTempFileName();
            Command newCommand = new Command(tempFilePath) {
                Name = "NewCommand",
                Usage = "/newcommand",
                Permission = $"{SessionContext.SelectedMod.ModInfo.Name.ToLower()}.newcommand",
                PermissionLevel = 4
            };
            EditorForm.OpenItemEditor(newCommand);
        }

        private void CreateCommand(object sender, ItemEditedEventArgs<Command> e)
        {
            if (e.Result)
            {
                CodeGenerationService.CreateCustomScript(SessionContext.SelectedMod, e.ActualItem);
            }
            new FileInfo(tempFilePath).Delete();
        }

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
