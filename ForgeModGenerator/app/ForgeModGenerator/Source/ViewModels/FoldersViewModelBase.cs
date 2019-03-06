using ForgeModGenerator.Persistence;
using ForgeModGenerator.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FoldersViewModelBase<TFolder, TFile> : FoldersWatcherViewModelBase<TFolder, TFile>
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem, IValidable
    {
        public FoldersViewModelBase(ISessionContextService sessionContext, IDialogService dialogService) : base(sessionContext, dialogService) { }

        public EditorForm<TFile> FileEditor { get; protected set; }

        private bool isFileUpdateAvailable;
        public bool IsFileUpdateAvailable {
            get => isFileUpdateAvailable;
            set => Set(ref isFileUpdateAvailable, value);
        }

        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<TFile>(FileEditor.OpenItemEditor));

        private ICommand resolveJsonFileCommand;
        public ICommand ResolveJsonFileCommand => resolveJsonFileCommand ?? (resolveJsonFileCommand = new RelayCommand(ResolveJsonFile));

        protected JsonUpdater<TFolder> JsonUpdater { get; set; }

        protected FrameworkElement FileEditForm { get; set; }

        /// <summary> Deserialized folders from FoldersJsonFilePath and checks if any file doesn't exists, if so, prompt if should fix this </summary>
        protected async void CheckJsonFileMismatch()
        {
            IEnumerable<TFolder> deserializedFolders = FileSynchronizer.GetFoldersFromFile(FoldersJsonFilePath, false);
            bool hasNotExistingFile = deserializedFolders != null ? deserializedFolders.Any(folder => folder.Files.Any(file => !File.Exists(file.Info.FullName))) : false;
            if (hasNotExistingFile)
            {
                string fileName = Path.GetFileName(FoldersJsonFilePath);
                string questionMessage = $"{fileName} file has occurencies that doesn't exist in root folder. Do you want to fix it and overwrite {fileName}? ";
                bool shouldFix = await DialogService.ShowMessage(questionMessage, "Conflict found", "Yes", "No", null);
                if (shouldFix)
                {
                    ForceJsonFileUpdate();
                }
            }
            foreach (TFolder folder in deserializedFolders)
            {
                folder.Clear();
            }
        }

        protected void ResolveJsonFile()
        {
            FileSynchronizer.AddNotReferencedFiles();
            CheckForUpdate();
            ForceJsonFileUpdate();
        }

        protected void CheckForUpdate() => IsFileUpdateAvailable = !IsJsonUpdated();

        protected virtual void ForceJsonFileUpdate() => JsonUpdater.ForceJsonUpdate();

        protected virtual bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                return FileSynchronizer.EnumerateFilteredFiles(FoldersRootPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }
    }
}
