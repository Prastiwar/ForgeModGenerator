﻿using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;
using Prism.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Base ViewModel class with json updater </summary>
    public abstract class FoldersViewModelBase<TFolder, TFile> : FoldersWatcherViewModelBase<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject, IValidable
    {
        public FoldersViewModelBase(ISessionContextService sessionContext, IDialogService dialogService, IFoldersExplorerFactory<TFolder, TFile> factory) : base(sessionContext, factory) => DialogService = dialogService;

        /// <summary> Path to json file that can be deserialized to folders </summary>
        public abstract string FoldersJsonFilePath { get; }

        public IEditorForm<TFile> FileEditor { get; protected set; }

        public IDialogService DialogService { get; protected set; }

        private bool isFileUpdateAvailable;
        public bool IsFileUpdateAvailable {
            get => isFileUpdateAvailable;
            set => SetProperty(ref isFileUpdateAvailable, value);
        }

        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new DelegateCommand<TFile>(FileEditor.OpenItemEditor));

        private ICommand resolveJsonFileCommand;
        public ICommand ResolveJsonFileCommand => resolveJsonFileCommand ?? (resolveJsonFileCommand = new DelegateCommand(ResolveJsonFile));

        protected IJsonUpdater<IEnumerable<TFolder>> JsonUpdater { get; set; }

        /// <summary> Deserialized folders from FoldersJsonFilePath and checks if any file doesn't exists, if so, prompt if should fix this </summary>
        protected async void CheckJsonFileMismatch()
        {
            IEnumerable<TFolder> deserializedFolders = Explorer.FileSynchronizer.Finder.FindFoldersFromFile(FoldersJsonFilePath, false);
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
                folder.Clear(); // dereference file paths
            }
        }

        /// <summary> Adds found files that are not referenced in json file </summary>
        protected void ResolveJsonFile()
        {
            Explorer.FileSynchronizer.AddNotReferencedFiles();
            CheckForUpdate();
            ForceJsonFileUpdate();
        }

        protected void CheckForUpdate() => IsFileUpdateAvailable = !IsJsonUpdated();

        protected virtual void ForceJsonFileUpdate() => JsonUpdater.ForceJsonUpdate();

        protected virtual bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                return Explorer.FileSynchronizer.Finder.EnumerateFilteredFiles(FoldersRootPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
        }
    }
}
