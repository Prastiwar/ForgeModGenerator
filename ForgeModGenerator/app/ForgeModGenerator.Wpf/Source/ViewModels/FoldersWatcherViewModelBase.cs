using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class FoldersWatcherViewModelBase<TFolder, TFile> : BindableBase
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public FoldersWatcherViewModelBase(ISessionContextService sessionContext, IDialogService dialogService, ISnackbarService snackbarService)
        {
            SessionContext = sessionContext;
            DialogService = dialogService;
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            OpenFolderDialog = new FolderBrowserDialog() { ShowNewFolderButton = true };
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
            SnackbarService = snackbarService;
        }

        /// <summary> Path to folder root where are all files localized </summary>
        public abstract string FoldersRootPath { get; }

        /// <summary> Path to json file that can be deserialized to folders </summary>
        public virtual string FoldersJsonFilePath { get; }

        public ISessionContextService SessionContext { get; }

        private ObservableFolder<TFolder> folders;
        public ObservableFolder<TFolder> Folders {
            get => folders;
            set {
                SetProperty(ref folders, value);
                if (folders != null && FileSynchronizer != null)
                {
                    FileSynchronizer.SyncedFolders = folders;
                }
            }
        }

        private TFolder selectedFolder;
        public TFolder SelectedFolder {
            get => selectedFolder;
            set => SetProperty(ref selectedFolder, value);
        }

        private TFile selectedFile;
        public TFile SelectedFile {
            get => selectedFile;
            set => SetProperty(ref selectedFile, value);
        }

        private bool isLoading;
        /// <summary> Determines when folders are loading - used to show loading circle </summary>
        public bool IsLoading {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private bool hasEmptyFolders;
        public bool HasEmptyFolders {
            get => hasEmptyFolders;
            set => SetProperty(ref hasEmptyFolders, value);
        }

        private ICommand onLoadedCommand;
        public ICommand OnLoadedCommand => onLoadedCommand ?? (onLoadedCommand = new DelegateCommand(OnLoaded));

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new DelegateCommand<TFolder>(ShowFileDialogAndCopyToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new DelegateCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new DelegateCommand<TFolder>(RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new DelegateCommand(ShowFolderDialogAndCopyToRoot));

        private ICommand addFileAsFolderCommand;
        public ICommand AddFileAsFolderCommand => addFileAsFolderCommand ?? (addFileAsFolderCommand = new DelegateCommand(ShowFileDialogAndCreateFolder));

        private ICommand removeEmptyFoldersCommand;
        public ICommand RemoveEmptyFoldersCommand => removeEmptyFoldersCommand ?? (removeEmptyFoldersCommand = new DelegateCommand(RemoveEmptyFolders));

        protected FoldersSynchronizer<TFolder, TFile> FileSynchronizer { get; set; }

        protected FoldersFactory<TFolder, TFile> FolderFactory { get; set; }

        protected HashSet<string> AllowedFileExtensions { get; set; }

        protected string AllowedFileExtensionsPatterns => "*" + string.Join("|*", AllowedFileExtensions);

        protected OpenFileDialog OpenFileDialog { get; }

        protected FolderBrowserDialog OpenFolderDialog { get; }

        protected ISnackbarService SnackbarService { get; }

        protected IDialogService DialogService { get; }

        protected virtual async void OnLoaded() => await Refresh();

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersJsonFilePath));
        protected virtual async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                IsLoading = true;
                if (Folders != null)
                {
                    Folders.Clear();
                }
                Folders = new ObservableFolder<TFolder>(FoldersRootPath, Enumerable.Empty<TFolder>());
                FolderFactory = new DefaultFoldersFactory<TFolder, TFile>(AllowedFileExtensionsPatterns);
                FileSynchronizer = new FoldersSynchronizer<TFolder, TFile>(Folders, FolderFactory, FoldersRootPath, AllowedFileExtensionsPatterns);
                Folders.AddRange(await FolderFactory.FindFoldersAsync(FoldersRootPath ?? FoldersJsonFilePath, true));
                IsLoading = false;
                return true;
            }
            return false;
        }

        protected async void ShowFolderDialogAndCopyToRoot()
        {
            DialogResult dialogResult = OpenFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                await CopyFolderToRoot(OpenFolderDialog.SelectedPath);
            }
        }

        /// <summary> Copies directory to root path, if directory with given name exists, add (n) number to its name </summary>
        protected async Task CopyFolderToRoot(string path)
        {
            path = IOHelper.GetDirectoryPath(path);
            string newFolderPath = IOHelper.GetUniqueName(Path.Combine(FoldersRootPath, new DirectoryInfo(path).Name), (name) => !Directory.Exists(name));
            await IOHelper.DirectoryCopyAsync(path, newFolderPath, AllowedFileExtensionsPatterns);
        }

        /// <summary> Removes folder and if it's empty, sends it to RecycleBin </summary>
        protected void RemoveFolder(TFolder folder)
        {
            if (Folders.Remove(folder))
            {
                for (int i = folder.Files.Count - 1; i >= 0; i--)
                {
                    if (FileSystemInfoReference.GetReferenceCount(folder.Files[i].Info.FullName) <= 1 && File.Exists(folder.Files[i].Info.FullName))
                    {
                        IOSafe.DeleteFileRecycle(folder.Files[i].Info.FullName);
                    }
                }
                folder.Clear();
                if (Directory.Exists(folder.Info.FullName) && IOHelper.IsEmpty(folder.Info.FullName))
                {
                    IOSafe.DeleteDirectoryRecycle(folder.Info.FullName);
                }
            }
        }

        protected void RemoveEmptyFolders()
        {
            for (int i = Folders.Files.Count - 1; i >= 0; i--)
            {
                if (Folders.Files[i].Files.Count == 0)
                {
                    Folders.RemoveAt(i);
                }
            }
            HasEmptyFolders = false;
        }

        protected virtual void RemoveFileFromFolder(Tuple<TFolder, TFile> param)
        {
            if (param.Item1.Remove(param.Item2))
            {
                if (!FileSystemInfoReference.IsReferenced(param.Item2.Info.FullName))
                {
                    if (!IOSafe.DeleteFileRecycle(param.Item2.Info.FullName))
                    {
                        DialogService.ShowMessage(IOSafe.GetOperationFailedMessage(param.Item2.Info.FullName), "Deletion failed");
                        param.Item1.Add(param.Item2);
                    }
                }
            }
        }

        protected async void ShowFileDialogAndCreateFolder()
        {
            DialogResult dialogResult = OpenFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string newFolderPath = null;
                string newFolderName = IOHelper.GetUniqueName(Path.GetFileNameWithoutExtension(OpenFileDialog.FileName), name => !Directory.Exists((newFolderPath = Path.Combine(FoldersRootPath, name))));
                TFolder folder = FolderFactory.ConstructFolderInstance(newFolderPath, null);
                await CopyFilesToFolderAsync(folder, OpenFileDialog.FileNames);
            }
        }

        protected virtual async void ShowFileDialogAndCopyToFolder(TFolder folder)
        {
            DialogResult dialogResult = OpenFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                await CopyFilesToFolderAsync(folder, OpenFileDialog.FileNames);
            }
        }

        /// <summary> Copies files to folder path, if file with given name exists, prompt for overwriting </summary>
        protected async Task CopyFilesToFolderAsync(TFolder folder, params string[] fileNames)
        {
            foreach (string filePath in fileNames)
            {
                string newPath = Path.Combine(folder.Info.FullName, Path.GetFileName(filePath));
                if (File.Exists(newPath))
                {
                    if (filePath != newPath)
                    {
                        bool overwrite = await DialogService.ShowMessage($"File {newPath} already exists.{Environment.NewLine}Do you want to overwrite it?", "Existing file conflict", "Yes", "No", null);
                        if (overwrite)
                        {
                            if (!IOSafe.CopyFile(filePath, newPath, true))
                            {
                                await DialogService.ShowMessage(IOSafe.GetOperationFailedMessage(filePath), "Copy failed");
                            }
                        }
                    }
                    else
                    {
                        folder.Add(newPath);
                    }
                }
                else
                {
                    if (!IOSafe.CopyFile(filePath, newPath))
                    {
                        await DialogService.ShowMessage(IOSafe.GetOperationFailedMessage(filePath), "Copy failed");
                    }
                }
            }
        }

        protected virtual async void OnSessionContexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                await Refresh();
            }
        }
    }
}
