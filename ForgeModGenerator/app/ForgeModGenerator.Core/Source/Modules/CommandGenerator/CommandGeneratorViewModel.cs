//using ForgeModGenerator.CodeGeneration;
//using ForgeModGenerator.CommandGenerator.Models;
//using ForgeModGenerator.Models;
//using ForgeModGenerator.ViewModels;
//using Prism.Commands;
//using System.IO;
//using System.Threading.Tasks;
//using System.Windows.Input;

//namespace ForgeModGenerator.CommandGenerator.ViewModels
//{
//    /// <summary> CommandGenerator Business ViewModel </summary>
//    public class CommandGeneratorViewModel : FoldersWatcherViewModelBase<ObservableFolder<Command>, Command>
//    {
//        public CommandGeneratorViewModel(GeneratorContext<Command> context,
//                                        IFoldersExplorerFactory<ObservableFolder<Command>, Command> factory)
//            : base(context, factory)
//        {
//            Explorer.OpenFileDialog.Filter = "Java file (*.java) | *.java";
//            Explorer.AllowFileExtensions(".java");
//            Explorer.FileSynchronizer.SyncFilter = NotifyFilter.File;
//        }

//        public override string DirectoryRootPath => SessionContext.SelectedMod != null
//            ? Path.GetDirectoryName(SourceCodeLocator.CustomCommand(SessionContext.SelectedMod.ModInfo.Name, SessionContext.SelectedMod.Organization, "None").FullPath)
//            : null;

//        private ICommand openCommandEditor;
//        public ICommand OpenCommandEditor => openCommandEditor ?? (openCommandEditor = new DelegateCommand<ObservableFolder<Command>>(CreateNewCommand));

//        private string tempFilePath;

//        public override async Task<bool> Refresh()
//        {
//            if (CanRefresh())
//            {
//                IsLoading = true;
//                Explorer.Folders.Clear();
//                await InitializeFoldersAsync(await Explorer.FileSynchronizer.Finder.FindFoldersAsync(DirectoryRootPath, true).ConfigureAwait(true)).ConfigureAwait(false);
//                Explorer.FileSynchronizer.RootPath = DirectoryRootPath;
//                Explorer.FileSynchronizer.SetEnableSynchronization(true);
//                SubscribeFolderEvents(Explorer.Folders, new FileChangedEventArgs<ObservableFolder<Command>>(Explorer.Folders.Files, FileChange.Add));
//                RegenerateCode();
//                IsLoading = false;
//                return true;
//            }
//            return false;
//        }

//        private void CreateNewCommand(ObservableFolder<Command> folder)
//        {
//            tempFilePath = Path.GetTempFileName();
//            Command newCommand = new Command(tempFilePath) {
//                ClassName = "NewCommand",
//                Name = "newcommand",
//                Usage = "/newcommand",
//                Permission = $"{SessionContext.SelectedMod.ModInfo.Name.ToLower()}.newcommand",
//                PermissionLevel = 4
//            };
//            newCommand.IsDirty = false;
//            newCommand.ValidateProperty += (sender, propertyName) => OnValidate(sender, folder.Files, propertyName);
//            Context.Validator.SetDefaultRepository(folder.Files);
//            EditorForm.OpenItemEditor(newCommand);
//        }

//        protected override void OnItemEdited(object sender, ItemEditedEventArgs<Command> e)
//        {
//            if (e.Result)
//            {
//                McMod mod = SessionContext.SelectedMod;
//                Context.CodeGenerationService.CreateCustomScript(mod, e.ActualItem);
//            }
//            new FileInfo(tempFilePath).Delete();
//        }

//        protected void SubscribeFolderEvents(object sender, FileChangedEventArgs<ObservableFolder<Command>> e)
//        {
//            if (e.Change == FileChange.Add)
//            {
//                foreach (ObservableFolder<Command> folder in e.Files)
//                {
//                    folder.FilesChanged += OnFilesChanged;
//                }
//            }
//            else if (e.Change == FileChange.Remove)
//            {
//                foreach (ObservableFolder<Command> folder in e.Files)
//                {
//                    folder.FilesChanged -= OnFilesChanged;
//                }
//            }
//        }

//        protected virtual void OnFilesChanged(object sender, FileChangedEventArgs<Command> e) => RegenerateCode();

//        protected override void RegenerateCode()
//        {
//            McMod mod = SessionContext.SelectedMod;
//            Context.CodeGenerationService.RegenerateInitScript(SourceCodeLocator.Commands(mod.ModInfo.Name, mod.Organization).ClassName, mod, Explorer.Folders.Files[0].Files);
//        }
//    }
//}
