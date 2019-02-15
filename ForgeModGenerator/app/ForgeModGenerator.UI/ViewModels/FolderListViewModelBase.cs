using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FolderListViewModelBase<TFolder, TFile> : ViewModelBase
        where TFolder : IFileFolder<TFile>
        where TFile : IFileItem
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

        public FolderListViewModelBase(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            FolderFileConverter = new TupleValueConverter<TFolder, TFile>();
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            OpenFolderDialog = new FolderBrowserDialog() { ShowNewFolderButton = true };
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
        }

        // Path to folder root where are all files localized
        public abstract string FoldersRootPath { get; }

        // Path to file that can be deserialized to folders
        public virtual string FoldersSerializeFilePath { get; }

        public ISessionContextService SessionContext { get; }

        public IMultiValueConverter FolderFileConverter { get; }

        public HashSet<string> AllowedFileExtensions { get; protected set; }

        protected OpenFileDialog OpenFileDialog { get; }

        protected FolderBrowserDialog OpenFolderDialog { get; }

        protected FrameworkElement FileEditForm { get; set; }

        protected string EditFileCacheKey => "EditFileCacheKey";

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
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new RelayCommand<TFolder>(AddNewFileToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new RelayCommand<TFolder>(RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new RelayCommand(AddNewFolder));
        #endregion

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersSerializeFilePath));
        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Folders = FindFolders(FoldersRootPath ?? FoldersSerializeFilePath, true);
                return true;
            }
            return false;
        }

        #region FileEditor
        protected virtual bool CanCloseFileEditor(bool result, FileEditedEventArgs args) => true;
        protected virtual void OnFileEditorClosing(object sender, FileEditorClosingDialogEventArgs eventArgs)
        {
            bool result = (bool)eventArgs.Parameter;
            if (!CanCloseFileEditor(result, new FileEditedEventArgs(eventArgs.Folder, (TFile)MemoryCache.Default.Get(EditFileCacheKey), eventArgs.File)))
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

        protected virtual void AddNewFolder()
        {
            DialogResult dialogResult = OpenFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (IOExtensions.IsSubPathOf(OpenFolderDialog.SelectedPath, FoldersRootPath))
                {
                    TFolder newFolder = WPFExtensions.CreateInstance<TFolder>(OpenFolderDialog.SelectedPath);
                    Folders.Add(newFolder);
                }
                else
                {
                    DirectoryInfo selectedPath = new DirectoryInfo(OpenFolderDialog.SelectedPath);
                    string newFolderPath = Path.Combine(FoldersRootPath, selectedPath.Name);

                    string fileSearchPattern = "*" + string.Join("|*", AllowedFileExtensions);
                    IOExtensions.DirectoryCopy(OpenFolderDialog.SelectedPath, newFolderPath, fileSearchPattern);

                    foreach (TFolder newFolder in FindFoldersFromDirectory(newFolderPath))
                    {
                        Folders.Add(newFolder);
                    }
                }
            }
        }

        protected virtual void RemoveFolder(TFolder folder)
        {
            if (Folders.Remove(folder))
            {
                folder.Delete();
            }
        }

        protected virtual void RemoveFileFromFolder(Tuple<TFolder, TFile> param) => param.Item1.Remove(param.Item2);

        protected virtual void AddNewFileToFolder(TFolder folder)
        {
            DialogResult dialogResult = OpenFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                foreach (string filePath in OpenFileDialog.FileNames)
                {
                    folder.Add(filePath);
                }
            }
        }

        #region Folders Initializers
        protected virtual ObservableCollection<TFolder> FindFolders(string path, bool createRootIfEmpty = false)
        {
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
                found = CreateEmptyFoldersRoot(path);
            }
            return found;
        }

        protected ObservableCollection<TFolder> CreateEmptyFoldersRoot(string folderPath)
        {
            TFolder root = WPFExtensions.CreateInstance<TFolder>(folderPath);
            SubscribeFolderEvents(root);
            return new ObservableCollection<TFolder>() { root };
        }

        protected ObservableCollection<TFolder> FindFoldersFromFile(string path, bool loadOnlyExisting = true)
        {
            string json = File.ReadAllText(path);
            ObservableCollection<TFolder> deserializedFolders = null;
            try
            {
                deserializedFolders = DeserializeFolders(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt load {typeof(ObservableCollection<TFolder>)} from {json}", true);
                return null;
            }
            if (loadOnlyExisting)
            {
                deserializedFolders = FilterExistingFiles(deserializedFolders);
            }
            foreach (TFolder folder in deserializedFolders)
            {
                SubscribeFolderEvents(folder);
            }
            return deserializedFolders;
        }

        protected ObservableCollection<TFolder> FilterExistingFiles(ObservableCollection<TFolder> deserializedFolders, bool sendWarning = true)
        {
            ObservableCollection<TFolder> folders = new ObservableCollection<TFolder>();
            foreach (TFolder folder in deserializedFolders)
            {
                for (int i = folder.Files.Count - 1; i >= 0; i--)
                {
                    if (!File.Exists(folder.Files[i].Info.FullName))
                    {
                        Log.Warning($"File was not loaded correctly { folder.Files[i].Info.FullName}", sendWarning);
                        folder.Files.RemoveAt(i);
                    }
                }
                if (folder.Files.Count > 0)
                {
                    folders.Add(folder);
                }
            }
            return folders;
        }

        protected ObservableCollection<TFolder> FindFoldersFromDirectory(string path)
        {
            ObservableCollection<TFolder> foundFolders = new ObservableCollection<TFolder>();

            AddFilesToCollection(path);
            foreach (string directory in IOExtensions.EnumerateAllDirectories(path))
            {
                AddFilesToCollection(directory);
            }

            void AddFilesToCollection(string directoryPath)
            {
                IEnumerable<string> files = EnumerateFilteredFiles(directoryPath);
                if (files.Any())
                {
                    TFolder folder = WPFExtensions.CreateInstance<TFolder>(directoryPath);
                    foreach (string filePath in files)
                    {
                        folder.Add(filePath);
                    }
                    SubscribeFolderEvents(folder);
                    foundFolders.Add(folder);
                }
            }
            return foundFolders;
        }

        // Returns file that use extension from AllowedFileExtensions
        protected IEnumerable<string> EnumerateFilteredFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            Directory.EnumerateFiles(directoryPath, "*", searchOption).Where(filePath => AllowedFileExtensions.Contains(Path.GetExtension(filePath)));

        // Used on any folder initialization
        protected virtual void SubscribeFolderEvents(TFolder folder) { }

        protected virtual ObservableCollection<TFolder> DeserializeFolders(string fileCotent) => JsonConvert.DeserializeObject<ObservableCollection<TFolder>>(fileCotent);
        #endregion

        protected virtual void OnSessionContexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                Refresh();
            }
        }
    }
}
