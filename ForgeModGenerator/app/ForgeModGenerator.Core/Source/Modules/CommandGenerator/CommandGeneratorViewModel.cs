using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
using Prism.Commands;
using System.Collections.Generic;
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
                                        IUniqueValidator<Command> validator,
                                        ICodeGenerationService codeGenerationService)
            : base(sessionContext, factory)
        {
            CommandValidator = validator;
            CodeGenerationService = codeGenerationService;
            EditorForm = editorFormFactory.Create();
            EditorForm.ItemEdited += CreateCommand;
            EditorForm.Validator = validator;

            Explorer.OpenFileDialog.Filter = "Java file (*.java) | *.java";
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
            Explorer.AllowFileExtensions(".java");

            Explorer.FileSynchronizer.SyncFilter = NotifyFilter.File;
        }

        public override string FoldersRootPath => SessionContext.SelectedMod != null
            ? Path.GetDirectoryName(SourceCodeLocator.CustomCommand(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization, "None").FullPath)
            : null;

        protected ICodeGenerationService CodeGenerationService { get; }
        protected IEditorForm<Command> EditorForm { get; }
        protected IUniqueValidator<Command> CommandValidator { get; }

        private ICommand openCommandEditor;
        public ICommand OpenCommandEditor => openCommandEditor ?? (openCommandEditor = new DelegateCommand<ObservableFolder<Command>>(CreateNewCommand));

        private string tempFilePath;

        public override async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                Explorer.Folders.Clear();
                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(FoldersRootPath, true).ConfigureAwait(true)).ConfigureAwait(false);
                Explorer.FileSynchronizer.RootPath = FoldersRootPath;
                Explorer.FileSynchronizer.SetEnableSynchronization(true);
                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<ObservableFolder<Command>>(Explorer.Folders.Files, FileChange.Add));
                RegenerateCommands();
                IsLoading = false;
                return true;
            }
            return false;
        }

        private void CreateNewCommand(ObservableFolder<Command> folder)
        {
            tempFilePath = Path.GetTempFileName();
            Command newCommand = new Command(tempFilePath) {
                ClassName = "NewCommand",
                Name = "newcommand",
                Usage = "/newcommand",
                Permission = $"{SessionContext.SelectedMod.ModInfo.Name.ToLower()}.newcommand",
                PermissionLevel = 4
            };
            newCommand.IsDirty = false;
            newCommand.ValidateProperty += (sender, propertyName) => ValidateCommand(sender, folder.Files, propertyName);
            CommandValidator.SetDefaultRepository(folder.Files);
            EditorForm.OpenItemEditor(newCommand);
        }

        protected string ValidateCommand(Command sender, IEnumerable<Command> instances, string propertyName) => CommandValidator.Validate(sender, instances, propertyName).Error;

        protected void CreateCommand(object sender, ItemEditedEventArgs<Command> e)
        {
            if (e.Result)
            {
                McMod mod = SessionContext.SelectedMod;
                CodeGenerationService.CreateCustomScript(mod, e.ActualItem);
            }
            new FileInfo(tempFilePath).Delete();
        }

        protected void SubscribeFolderEvents(object sender, FileChangedEventArgs<ObservableFolder<Command>> e)
        {
            if (e.Change == FileChange.Add)
            {
                foreach (ObservableFolder<Command> folder in e.Files)
                {
                    folder.FilesChanged += OnFilesChanged;
                }
            }
            else if (e.Change == FileChange.Remove)
            {
                foreach (ObservableFolder<Command> folder in e.Files)
                {
                    folder.FilesChanged -= OnFilesChanged;
                }
            }
        }

        protected void RegenerateCommands()
        {
            McMod mod = SessionContext.SelectedMod;
            CodeGenerationService.RegenerateInitScript(SourceCodeLocator.Commands(mod.ModInfo.Name, mod.Organization).ClassName, mod, Explorer.Folders.Files[0].Files);
        }

        protected virtual void OnFilesChanged(object sender, FileChangedEventArgs<Command> e) => RegenerateCommands();
    }
}
