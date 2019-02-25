using ForgeModGenerator.Converters;
using ForgeModGenerator.Persistence;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FolderListViewModelBase<TFolder, TFile> : ViewModelBase
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public FolderListViewModelBase(ISessionContextService sessionContext, IDialogService dialogService)
        {
            SessionContext = sessionContext;
            DialogService = dialogService;
            if (IsInDesignMode)
            {
                return;
            }
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            OpenFolderDialog = new FolderBrowserDialog() { ShowNewFolderButton = true };
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
            FileSynchronizer = new FoldersSynchronizer<TFolder, TFile>(Folders, FoldersRootPath, AllowedFileExtensionsPatterns);
        }

        public IMultiValueConverter FolderFileConverter { get; } = new TupleValueConverter<TFolder, TFile>();

        /// <summary> Path to folder root where are all files localized </summary>
        public abstract string FoldersRootPath { get; }

        /// <summary> Path to json file that can be deserialized to folders </summary>
        public virtual string FoldersJsonFilePath { get; }

        public ISessionContextService SessionContext { get; }

        public FileEditor<TFolder, TFile> FileEditor { get; protected set; }

        private ObservableFolder<TFolder> folders;
        public ObservableFolder<TFolder> Folders {
            get => folders;
            set {
                Set(ref folders, value);
                FileSynchronizer.Folders = folders;
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

        private bool isFileUpdateAvailable;
        public bool IsFileUpdateAvailable {
            get => isFileUpdateAvailable;
            set => Set(ref isFileUpdateAvailable, value);
        }

        private ICommand onLoadedCommand;
        public ICommand OnLoadedCommand => onLoadedCommand ?? (onLoadedCommand = new RelayCommand(OnLoaded));

        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(FileEditor.OpenFileEditor));

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new RelayCommand<TFolder>(ShowFileDialogAndCopyToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new RelayCommand<TFolder>(RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new RelayCommand(ShowFolderDialogAndCopyToRoot));

        private ICommand resolveJsonFileCommand;
        public ICommand ResolveJsonFileCommand => resolveJsonFileCommand ?? (resolveJsonFileCommand = new RelayCommand(FileSynchronizer.AddNotReferencedFiles));

        protected JsonUpdater<TFolder> JsonUpdater { get; set; }

        protected FoldersSynchronizer<TFolder, TFile> FileSynchronizer { get; set; }

        protected HashSet<string> AllowedFileExtensions { get; set; }

        protected string AllowedFileExtensionsPatterns => "*" + string.Join("|*", AllowedFileExtensions);

        protected OpenFileDialog OpenFileDialog { get; }

        protected FolderBrowserDialog OpenFolderDialog { get; }

        protected FrameworkElement FileEditForm { get; set; }

        protected IDialogService DialogService { get; }

        protected virtual async void OnLoaded() => await Refresh();

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersJsonFilePath));
        protected virtual async Task<bool> Refresh()
        {
            if (CanRefresh())
            {
                FileSynchronizer.RootPath = FoldersRootPath;
                IsLoading = true;
                Folders = new ObservableFolder<TFolder>(FileSynchronizer.GetFolders(FoldersRootPath ?? FoldersJsonFilePath, true));
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

        /// <summary> Removes folder and if it's empty, sends it to RecycleBin  </summary>
        protected void RemoveFolder(TFolder folder)
        {
            if (Folders.Remove(folder))
            {
                for (int i = folder.Files.Count - 1; i >= 0; i--)
                {
                    if (File.Exists(folder.Files[i].Info.FullName))
                    {
                        IOHelper.DeleteFileToBin(folder.Files[i].Info.FullName);
                    }
                    folder.Remove(folder.Files[i]);
                }
                if (Directory.Exists(folder.Info.FullName) && IOHelper.IsEmpty(folder.Info.FullName))
                {
                    IOHelper.DeleteDirectoryToBin(folder.Info.FullName);
                }
            }
        }

        protected virtual void RemoveFileFromFolder(Tuple<TFolder, TFile> param) => param.Item1.Remove(param.Item2);

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
                        bool overwrite = await DialogService.ShowMessage($"File {newPath} already exists.\nDo you want to overwrite it?", "Existing file conflict", "Yes", "No", null);
                        if (overwrite)
                        {
                            File.Copy(filePath, newPath, true);
                        }
                    }
                    else
                    {
                        folder.Add(newPath);
                    }
                }
                else
                {
                    File.Copy(filePath, newPath);
                }
            }
        }

        /// <summary> Deserialized folders from FoldersJsonFilePath and checks if any file doesn't exists, if so, prompt if should fix this </summary>
        protected async void CheckJsonFileMismatch()
        {
            IEnumerable<TFolder> deserializedFolders = FileSynchronizer.FindFoldersFromFile(FoldersJsonFilePath, false);
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
        }

        protected virtual void ForceJsonFileUpdate() => JsonUpdater.ForceJsonUpdate();

        protected void CheckForUpdate() => IsFileUpdateAvailable = !IsJsonUpdated();

        protected virtual bool IsJsonUpdated()
        {
            if (SessionContext.SelectedMod != null)
            {
                return FileSynchronizer.EnumerateFilteredFiles(FoldersRootPath, SearchOption.AllDirectories).All(filePath => FileSystemInfoReference.IsReferenced(filePath));
            }
            return true;
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
