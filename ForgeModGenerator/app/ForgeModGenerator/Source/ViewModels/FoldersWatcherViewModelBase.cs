using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    public abstract class FoldersWatcherViewModelBase<TFolder, TFile> : ViewModelBase
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public FoldersWatcherViewModelBase(ISessionContextService sessionContext, IDialogService dialogService)
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
                Set(ref folders, value);
                if (folders != null && FileSynchronizer != null)
                {
                    FileSynchronizer.SyncedFolders = folders;
                }
            }
        }

        private TFolder selectedFolder;
        public TFolder SelectedFolder {
            get => selectedFolder;
            set => Set(ref selectedFolder, value);
        }

        private TFile selectedFile;
        public TFile SelectedFile {
            get => selectedFile;
            set => Set(ref selectedFile, value);
        }

        private bool isLoading;
        /// <summary> Determines when folders are loading - used to show loading circle </summary>
        public bool IsLoading {
            get => isLoading;
            set => Set(ref isLoading, value);
        }

        private ICommand onLoadedCommand;
        public ICommand OnLoadedCommand => onLoadedCommand ?? (onLoadedCommand = new RelayCommand(OnLoaded));

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new RelayCommand<TFolder>(ShowFileDialogAndCopyToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new RelayCommand<TFolder>(RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new RelayCommand(ShowFolderDialogAndCopyToRoot));

        protected FoldersSynchronizer<TFolder, TFile> FileSynchronizer { get; set; }

        protected FoldersFactory<TFolder, TFile> FolderFactory { get; set; }

        protected HashSet<string> AllowedFileExtensions { get; set; }

        protected string AllowedFileExtensionsPatterns => "*" + string.Join("|*", AllowedFileExtensions);

        protected OpenFileDialog OpenFileDialog { get; }

        protected FolderBrowserDialog OpenFolderDialog { get; }

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
                Folders = new ObservableFolder<TFolder>(FoldersRootPath, System.Linq.Enumerable.Empty<TFolder>());
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

        protected virtual async void RemoveFileFromFolder(Tuple<TFolder, TFile> param)
        {
            if (param.Item1.Remove(param.Item2))
            {
                if (!FileSystemInfoReference.IsReferenced(param.Item2.Info.FullName))
                {
                    if (!IOSafe.DeleteFileRecycle(param.Item2.Info.FullName))
                    {
                        await DialogService.ShowMessage(IOSafe.GetOperationFailedMessage(param.Item2.Info.FullName), "Deletion failed");
                        param.Item1.Add(param.Item2);
                    }
                }
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
