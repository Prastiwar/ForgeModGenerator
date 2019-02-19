using ForgeModGenerator.Converters;
using ForgeModGenerator.Exceptions;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
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
        #region EventArgs
        public class FileEditedEventArgs : EventArgs
        {
            public FileEditedEventArgs(TFolder folder, TFile cachedFile, TFile actualFile)
            {
                Folder = folder;
                CachedFile = cachedFile;
                ActualFile = actualFile;
            }
            public TFolder Folder { get; }
            public TFile CachedFile { get; }
            public TFile ActualFile { get; }
        }

        public class FileEditorClosingDialogEventArgs : DialogClosingEventArgs
        {
            public FileEditorClosingDialogEventArgs(DialogClosingEventArgs otherArgs, TFolder folder, TFile file)
                : base(otherArgs.Session, otherArgs.Parameter, otherArgs.RoutedEvent, otherArgs.Source)
            {
                Folder = folder;
                File = file;
                OtherArgs = otherArgs;
            }
            public DialogClosingEventArgs OtherArgs { get; }
            public TFolder Folder { get; }
            public TFile File { get; }

            public new void Cancel() => OtherArgs.Cancel();
        }

        public class FileEditorOpeningDialogEventArgs : DialogOpenedEventArgs
        {
            public FileEditorOpeningDialogEventArgs(DialogOpenedEventArgs otherArgs, TFolder folder, TFile file)
                : base(otherArgs.Session, otherArgs.RoutedEvent)
            {
                Folder = folder;
                File = file;
            }
            public TFolder Folder { get; }
            public TFile File { get; }
        }
        #endregion

        public FolderListViewModelBase(ISessionContextService sessionContext, IDialogService dialogService)
        {
            SessionContext = sessionContext;
            DialogService = dialogService;
            FolderFileConverter = new TupleValueConverter<TFolder, TFile>();
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
            FileWatcher = new FileSystemWatcherExtended(FoldersRootPath, AllowedFileExtensionsPatterns) {
                IncludeSubdirectories = true,
                MonitorDirectoryChanges = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName
            };
            FileWatcher.Created += FileWatcher_Created;
            FileWatcher.Deleted += FileWatcher_Deleted;
            FileWatcher.Renamed += FileWatcher_Renamed;
            FileWatcher.Changed += FileWatcher_Changed;
            FileWatcher.Disposed += FileWatcher_Disposed;
            FileWatcher.Error += FileWatcher_Error;
        }

        /// <summary> Path to folder root where are all files localized </summary>
        public abstract string FoldersRootPath { get; }

        /// <summary> Path to file that can be deserialized to folders </summary>
        public virtual string FoldersSerializeFilePath { get; }

        public ISessionContextService SessionContext { get; }

        public IMultiValueConverter FolderFileConverter { get; }

        private ObservableCollection<TFolder> folders;
        public ObservableCollection<TFolder> Folders {
            get => folders;
            set => Set(ref folders, value);
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

        #region Commands
        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(OpenFileEditor));

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new RelayCommand<TFolder>(ShowFileDialogAndCopyToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new RelayCommand<TFolder>(RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new RelayCommand(ShowFolderDialogAndCopyToRoot));
        #endregion

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        protected HashSet<string> AllowedFileExtensions { get; set; }

        protected string AllowedFileExtensionsPatterns => "*" + string.Join("|*", AllowedFileExtensions);

        protected OpenFileDialog OpenFileDialog { get; }

        protected FolderBrowserDialog OpenFolderDialog { get; }

        protected FrameworkElement FileEditForm { get; set; }

        protected IDialogService DialogService { get; }

        protected string EditFileCacheKey => "EditFileCacheKey";

        #region FileWatcherSystem
        protected void SynchronizationCheck(string rootPath, string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, rootPath))
            {
                throw new InvalidSynchronizationArgument(rootPath, actualPath);
            }
        }

        protected void SyncCreateFolder(string path, bool includeSubDirectories = true)
        {
            SynchronizationCheck(FoldersRootPath, path);
            Folders.Add(ConstructFolderInstance(path, null));
            if (includeSubDirectories)
            {
                ObservableCollection<TFolder> foundFolders = FindFoldersFromDirectory(path);
                if (foundFolders.Count > 0)
                {
                    foreach (TFolder folder in foundFolders)
                    {
                        Folders.Add(folder);
                    }
                }
            }
        }

        protected void SyncCreateFile(string path)
        {
            SynchronizationCheck(FoldersRootPath, path);
            string folderPath = IOHelper.GetDirectoryPath(path);
            if (TryGetFolder(folderPath, out TFolder folder))
            {
                folder.Add(path);
            }
        }

        protected void SyncRemoveFolder(string path)
        {
            SynchronizationCheck(FoldersRootPath, path);
            if (TryGetFolder(path, out TFolder folder))
            {
                if (Folders.Remove(folder))
                {
                    folder.Clear();
                }
            }
        }

        protected void SyncRemoveFile(string path)
        {
            SynchronizationCheck(FoldersRootPath, path);
            if (TryGetFile(path, out TFile file, out TFolder folder))
            {
                folder.Remove(file);
            }
        }

        protected void SyncRenameFolder(string oldPath, string newPath)
        {
            SynchronizationCheck(FoldersRootPath, oldPath);
            string oldFolderPath = IOHelper.GetDirectoryPath(oldPath);
            string folderPath = IOHelper.GetDirectoryPath(newPath);
            if (TryGetFolder(oldFolderPath, out TFolder oldFolder))
            {
                oldFolder.SetInfo(folderPath);
            }
        }

        protected void SyncRenameFile(string oldPath, string newPath)
        {
            SynchronizationCheck(FoldersRootPath, oldPath);
            if (TryGetFile(oldPath, out TFile file, out TFolder folder))
            {
                file.SetInfo(newPath);
            }
        }

        protected virtual void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncCreateFolder(e.FullPath, true); });
            }
            else // is file
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncCreateFile(e.FullPath); });
            }
        }

        protected virtual void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncRemoveFolder(e.FullPath); });
            }
            else // is file
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncRemoveFile(e.FullPath); });
            }
        }

        protected virtual void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncRenameFolder(e.OldFullPath, e.FullPath); });
            }
            else // is file
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { SyncRenameFile(e.OldFullPath, e.FullPath); });
            }
        }

        protected virtual void FileWatcher_Changed(object sender, FileSystemEventArgs e) { }
        protected virtual void FileWatcher_Disposed(object sender, EventArgs e) { }
        protected virtual void FileWatcher_Error(object sender, ErrorEventArgs e) { }
        #endregion

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersSerializeFilePath));
        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Folders = FindFolders(FoldersRootPath ?? FoldersSerializeFilePath, true);
                FileWatcher.Path = FoldersRootPath;
                return true;
            }
            return false;
        }

        #region FileEditor
        protected virtual async Task<bool> CanCloseFileEditor(bool result, FileEditedEventArgs args)
        {
            if (!result)
            {
                if (args.ActualFile.IsDirty)
                {
                    return await DialogService.ShowMessage("Are you sure you want to exit form? Changes won't be saved", "Unsaved changes", "Yes", "No", null);
                }
            }
            return true;
        }

        protected virtual async void OnFileEditorClosing(object sender, FileEditorClosingDialogEventArgs eventArgs)
        {
            bool result = (bool)eventArgs.Parameter;
            bool canClose = await CanCloseFileEditor(result, new FileEditedEventArgs(eventArgs.Folder, (TFile)MemoryCache.Default.Get(EditFileCacheKey), eventArgs.File));
            if (!canClose)
            {
                eventArgs.Cancel();
            }
        }

        protected virtual void OnFileEditorOpening(object sender, FileEditorOpeningDialogEventArgs eventArgs) { }
        protected virtual async void OpenFileEditor(Tuple<TFolder, TFile> param)
        {
            SelectedFile = param.Item2;
            if (FileEditForm == null)
            {
                return;
            }
            FileEditForm.DataContext = SelectedFile;
            MemoryCache.Default.Set(EditFileCacheKey, SelectedFile.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration); // cache file state so it can be restored later
            bool result = false;
            try
            {
                result = (bool)await DialogHost.Show(FileEditForm,
                    (sender, args) => { OnFileEditorOpening(sender, new FileEditorOpeningDialogEventArgs(args, param.Item1, param.Item2)); },
                    (sender, args) => { OnFileEditorClosing(sender, new FileEditorClosingDialogEventArgs(args, param.Item1, param.Item2)); });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't open edit form for {param.Item2.Info.Name}", true);
                return;
            }
            OnFileEdited(result, new FileEditedEventArgs(param.Item1, (TFile)MemoryCache.Default.Remove(EditFileCacheKey), param.Item2));
        }

        protected virtual void OnFileEdited(bool result, FileEditedEventArgs args)
        {
            if (!result)
            {
                args.ActualFile.CopyValues(args.CachedFile);
            }
        }
        #endregion

        protected void ShowFolderDialogAndCopyToRoot()
        {
            DialogResult dialogResult = OpenFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                CopyFolderToRoot(OpenFolderDialog.SelectedPath);
            }
        }

        /// <summary> Copies directory to root path, if directory with given name exists, incrementally change its name </summary>
        protected void CopyFolderToRoot(string path)
        {
            path = IOHelper.GetDirectoryPath(path);
            string newFolderPath = IOHelper.GetUniqueName(Path.Combine(FoldersRootPath, new DirectoryInfo(path).Name), (name) => !Directory.Exists(name));
            IOHelper.DirectoryCopy(path, newFolderPath, AllowedFileExtensionsPatterns);
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

        /// <summary> Copies files to folder path, if file with given name exists, prompt for overwriting </summary>
        protected async void CopyFilesToFolderAsync(TFolder folder, params string[] fileNames)
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

        protected virtual void ShowFileDialogAndCopyToFolder(TFolder folder)
        {
            DialogResult dialogResult = OpenFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                CopyFilesToFolderAsync(folder, OpenFileDialog.FileNames);
            }
        }

        #region Folders Initializers
        protected virtual ObservableCollection<TFolder> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!IOHelper.IsPathValid(path))
            {
                throw new InvalidOperationException($"Path is not valid {path}");
            }
            ObservableCollection<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = FindFoldersFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = FindFoldersFromFile(path);
            }

            bool isEmpty = found == null || found.Count <= 0;
            if (isEmpty && createRootIfEmpty)
            {
                found = CreateEmptyFoldersRoot(IOHelper.GetDirectoryPath(path));
            }
            return found;
        }

        /// <summary> Creates TFolder with found TFile's for each subdirectory </summary>
        protected ObservableCollection<TFolder> FindFoldersFromDirectory(string path)
        {
            ObservableCollection<TFolder> foundFolders = new ObservableCollection<TFolder>();

            AddFilesToCollection(path);
            foreach (string directory in IOHelper.EnumerateAllDirectories(path))
            {
                AddFilesToCollection(directory);
            }

            void AddFilesToCollection(string directoryPath)
            {
                IEnumerable<string> filePaths = EnumerateAllowedFiles(directoryPath);
                if (filePaths.Any())
                {
                    TFolder folder = ConstructFolderInstance(directoryPath, filePaths);
                    foundFolders.Add(folder);
                }
            }
            return foundFolders;
        }

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        protected ObservableCollection<TFolder> FindFoldersFromFile(string path, bool loadOnlyExisting = true)
        {
            string content = File.ReadAllText(path);
            ObservableCollection<TFolder> deserializedFolders = null;
            try
            {
                deserializedFolders = DeserializeFolders(content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt load {typeof(ObservableCollection<TFolder>)} from {content}", true);
                return null;
            }
            foreach (TFolder folder in deserializedFolders)
            {
                SubscribeFolderEvents(folder);
            }
            if (loadOnlyExisting)
            {
                deserializedFolders = FilterToOnlyExistingFiles(deserializedFolders);
            }
            return deserializedFolders;
        }

        /// <summary> Returns file that use extension from AllowedFileExtensions </summary>
        protected IEnumerable<string> EnumerateAllowedFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly) => IOHelper.EnumerateFiles(directoryPath, AllowedFileExtensionsPatterns, searchOption);

        protected ObservableCollection<TFolder> CreateEmptyFoldersRoot(string folderPath) => new ObservableCollection<TFolder>() { ConstructFolderInstance(IOHelper.GetDirectoryPath(folderPath), null) };

        protected ObservableCollection<TFolder> FilterToOnlyNotExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => !File.Exists(file.Info.FullName));
        protected ObservableCollection<TFolder> FilterToOnlyExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => File.Exists(file.Info.FullName));

        /// <summary> Returns folders which files matches fileFilter </summary>
        protected ObservableCollection<TFolder> FilterFolderFiles(IEnumerable<TFolder> foldersToFilter, Func<TFile, bool> fileFilter)
        {
            ObservableCollection<TFolder> folders = new ObservableCollection<TFolder>();
            foreach (TFolder folder in foldersToFilter.Where(dir => Directory.Exists(dir.Info.FullName)))
            {
                TFolder copyFolder = (TFolder)folder.DeepClone();
                for (int i = copyFolder.Files.Count - 1; i >= 0; i--)
                {
                    TFile file = copyFolder.Files[i];
                    if (!fileFilter(file))
                    {
                        copyFolder.Remove(file);
                    }
                }
                if (copyFolder.Files.Count > 0)
                {
                    folders.Add(copyFolder);
                }
            }
            return folders;
        }

        protected virtual TFolder ConstructFolderInstance(string path, IEnumerable<string> filePaths)
        {
            TFolder folder = null;
            try
            {
                folder = ReflectionExtensions.CreateInstance<TFolder>(path, filePaths);
            }
            catch (Exception)
            {
                folder = ReflectionExtensions.CreateInstance<TFolder>(path);
                if (filePaths != null)
                {
                    foreach (string filePath in filePaths)
                    {
                        folder.Add(filePath);
                    }
                }
            }
            SubscribeFolderEvents(folder);
            return folder;
        }

        /// <summary> Used on any folder initialization (e.g ConstructFolderInstance(..)) </summary>
        protected virtual void SubscribeFolderEvents(TFolder folder) { }

        /// <summary> Deserializes Json content to ObservableCollection<TFolder> </summary>
        protected virtual ObservableCollection<TFolder> DeserializeFolders(string fileCotent) => JsonConvert.DeserializeObject<ObservableCollection<TFolder>>(fileCotent);
        #endregion

        protected virtual void OnSessionContexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                Refresh();
            }
        }

        protected bool TryGetFolder(string path, out TFolder folder)
        {
            string folderPath = IOHelper.GetDirectoryPath(path);
            folder = Folders.Find(x => x.Info.FullName == folderPath);
            return folder != null;
        }

        protected bool TryGetFile(string path, out TFile file, out TFolder folder)
        {
            if (TryGetFolder(path, out folder))
            {
                file = folder.Files.Find(x => x.Info.FullName == path);
                return file != null;
            }
            file = null;
            return false;
        }
    }
}
