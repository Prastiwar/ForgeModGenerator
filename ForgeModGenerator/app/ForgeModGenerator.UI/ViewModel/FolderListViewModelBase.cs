using ForgeModGenerator.Miscellaneous;
using ForgeModGenerator.Model;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FolderListViewModelBase<TFolder, TFile> : ViewModelBase
        where TFolder : IFileFolder<TFile>
        where TFile : IFileItem
    {
        public FolderListViewModelBase(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
            OpenFileDialog = new OpenFileDialog() {
                Multiselect = true,
                CheckFileExists = true,
                ValidateNames = true
            };
            AllowedExtensions = new string[] { OpenFileDialog.DefaultExt };
            SessionContext.PropertyChanged += OnSessionContexPropertyChanged;
        }

        public delegate void OnFileChangedEventHandler(object itemChanged);
        protected event OnFileChangedEventHandler OnFileAdded;
        protected event OnFileChangedEventHandler OnFileRemoved;

        public ISessionContextService SessionContext { get; }
        protected OpenFileDialog OpenFileDialog { get; }

        public abstract string FoldersRootPath { get; }

        public string[] AllowedExtensions { get; protected set; }

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

        private ICommand editFileCommand;
        public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<TFile>(EditFile));

        private ICommand addCommand;
        public ICommand AddCommand => addCommand ?? (addCommand = new RelayCommand<TFolder>(AddNewFile));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new RelayCommand<Tuple<TFolder, TFile>>(RemoveFile));

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(FoldersRootPath) || File.Exists(FoldersRootPath));

        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Folders = FindCollection(FoldersRootPath, true);
                return true;
            }
            return false;
        }

        protected virtual void OpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs) { }

        protected virtual void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            bool result = (bool)eventArgs.Parameter;
            if (!CanCloseEditForm(result, (TFile)MemoryCache.Default.Get(EditFileCacheKey), SelectedFile))
            {
                eventArgs.Cancel();
            }
        }

        protected virtual bool CanCloseEditForm(bool result, TFile fileBeforeEdit, TFile fileAfterEdit) => true;

        protected virtual async void EditFile(TFile file)
        {
            SelectedFile = file;
            bool result = false;
            try
            {
                MemoryCache.Default.Set(EditFileCacheKey, SelectedFile.DeepClone(), ObjectCache.InfiniteAbsoluteExpiration);
                FileEditForm.DataContext = SelectedFile;
                result = (bool)await DialogHost.Show(FileEditForm, OpenedEventHandler, ClosingEventHandler);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't open edit form for {file.Info.Name}", true);
                return;
            }
            OnEdited(result, file);
        }

        protected virtual void OnEdited(bool result, TFile file)
        {
            if (!result)
            {
                // TODO: Undo commands
                file.CopyValues(MemoryCache.Default.Remove(EditFileCacheKey));
            }
        }

        protected virtual void RemoveFile(Tuple<TFolder, TFile> param)
        {
            try
            {
                if (param.Item1.Remove(param.Item2))
                {
                    try
                    {
                        FileSystem.DeleteFile(param.Item2.Info.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't delete {param.Item2.Info.FullName}. Make sure it's not used by any process", true);
                        param.Item1.Add(param.Item2); // delete failed, so get item back to collection
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected virtual void AddNewFile(TFolder collection)
        {
            try
            {
                DialogResult dialogResult = OpenFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    foreach (string filePath in OpenFileDialog.FileNames)
                    {
                        string fileName = new FileInfo(filePath).Name;
                        string newFilePath = Path.Combine(collection.Info.FullName, fileName);
                        try
                        {
                            if (File.Exists(newFilePath))
                            {
                                Log.Warning($"File {newFilePath} already exists.", true);
                                return;
                            }
                            File.Copy(filePath, newFilePath, true);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Couldn't add {fileName} to {collection.Info.Name}. Make sure the file is not opened by any process.", true);
                            continue;
                        }
                        collection.Add(newFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Log.UnexpectedErrorMessage, true);
            }
        }

        protected virtual ObservableCollection<TFolder> FindCollection(string path, bool createRootIfEmpty = false)
        {
            ObservableCollection<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = FindCollectionFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = DeserializeCollectionFromFile(path);
            }
            bool isEmpty = found == null || found.Count <= 0;
            if (createRootIfEmpty && isEmpty)
            {
                found = CreateEmptyRoot(path);
            }
            return found;
        }

        protected ObservableCollection<TFolder> CreateEmptyRoot(string path)
        {
            TFolder root = Util.CreateInstance<TFolder>(path);
            root.CollectionChanged += OnFolderChanged;
            return new ObservableCollection<TFolder>() { root };
        }

        protected static ObservableCollection<TFolder> DeserializeCollectionFromFile(string path)
        {
            string json = File.ReadAllText(path);
            try
            {
                return JsonConvert.DeserializeObject<ObservableCollection<TFolder>>(json);
            }
            catch (Exception ex)
            {
                Type loadType = typeof(ObservableCollection<TFolder>);
                Log.Error(ex, $"Couldnt load {loadType} from {json}");
                try
                {
                    return new ObservableCollection<TFolder>() { JsonConvert.DeserializeObject<TFolder>(json) };
                }
                catch (Exception ex2)
                {
                    loadType = typeof(TFolder);
                    Log.Error(ex2, $"Couldnt load {loadType} from {json}");
                }
            }
            return null;
        }

        protected ObservableCollection<TFolder> FindCollectionFromDirectory(string path)
        {
            List<TFolder> initCollection = new List<TFolder>(10);
            AddFilesToCollection(path);
            foreach (string directory in Directory.EnumerateDirectories(path, "*", System.IO.SearchOption.AllDirectories))
            {
                AddFilesToCollection(directory);
            }
            void AddFilesToCollection(string directoryPath)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(directoryPath).Where(filePath => AllowedExtensions.Any(ext => ext == Path.GetExtension(filePath)));
                if (files.Any())
                {
                    TFolder folder = Util.CreateInstance<TFolder>(directoryPath);
                    foreach (string filePath in files)
                    {
                        if (AllowedExtensions.Any(x => x == Path.GetExtension(filePath)))
                        {
                            folder.Add(Path.GetFullPath(filePath).Replace('\\', '/'));
                        }
                    }
                    folder.CollectionChanged += OnFolderChanged;
                    initCollection.Add(folder);
                }
            }
            return new ObservableCollection<TFolder>(initCollection);
        }

        protected virtual void OnFolderChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    OnFileAdded?.Invoke(item);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    OnFileRemoved?.Invoke(item);
                }
            }
        }

        protected virtual void OnSessionContexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SessionContext.SelectedMod))
            {
                Refresh();
            }
        }
    }
}
