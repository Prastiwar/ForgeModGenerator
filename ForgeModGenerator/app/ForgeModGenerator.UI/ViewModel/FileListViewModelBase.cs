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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> Business ViewModel Base class for making file list </summary>
    public abstract class FileListViewModelBase<T> : ViewModelBase where T : IFileItem
    {
        public delegate void OnFileChangedEventHandler(object itemChanged);

        public ISessionContextService SessionContext { get; }
        protected OpenFileDialog OpenFileDialog { get; }

        protected event OnFileChangedEventHandler OnFileAdded;
        protected event OnFileChangedEventHandler OnFileRemoved;

        public FileListViewModelBase(ISessionContextService sessionContext)
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

        public Visibility EmptyMessageVisibility => SessionContext.IsModSelected ? Visibility.Collapsed : Visibility.Visible;

        public abstract string CollectionRootPath { get; }

        public string[] AllowedExtensions { get; protected set; }

        protected FrameworkElement FileEditForm { get; set; }

        private ObservableCollection<FileList<T>> files;
        public ObservableCollection<FileList<T>> Files {
            get => files;
            set => Set(ref files, value);
        }

        private FileList<T> selectedFiles;
        public FileList<T> SelectedFiles {
            get => selectedFiles;
            set => Set(ref selectedFiles, value);
        }

        private T selectedFileItem;
        public T SelectedFileItem {
            get => selectedFileItem;
            set => Set(ref selectedFileItem, value);
        }

        private ICommand editCommand;
        public ICommand EditCommand => editCommand ?? (editCommand = new RelayCommand<IFileItem>(Edit));

        private ICommand addCommand;
        public ICommand AddCommand => addCommand ?? (addCommand = new RelayCommand<FileList<T>>(AddNewFile));

        private ICommand removeCommand;
        public ICommand RemoveCommand => removeCommand ?? (removeCommand = new RelayCommand<Tuple<IFileFolder, IFileItem>>(Remove));

        protected virtual bool CanRefresh() => SessionContext.SelectedMod != null && (Directory.Exists(CollectionRootPath) || File.Exists(CollectionRootPath));

        protected virtual bool Refresh()
        {
            if (CanRefresh())
            {
                Files = FindCollection(CollectionRootPath, true);
                return true;
            }
            return false;
        }

        protected virtual void OnEdited(bool result, IFileItem file) { }
        protected virtual void OpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs) { }
        protected virtual void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) { }
        protected virtual async void Edit(IFileItem file)
        {
            SelectedFileItem = (T)file;
            bool result = false;
            try
            {
                FileEditForm.DataContext = SelectedFileItem;
                result = (bool)await DialogHost.Show(FileEditForm, OpenedEventHandler, ClosingEventHandler);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't open edit form for {file.FileName}", true);
                return;
            }
            OnEdited(result, file);
        }

        protected virtual void Remove(Tuple<IFileFolder, IFileItem> param)
        {
            if (param == null)
            {
                Log.Warning("Remove item called with null parameter");
                return;
            }
            else if (param.Item1 == null)
            {
                Log.Warning("Remove item called with null collection", true);
                return;
            }
            else if (param.Item2 == null)
            {
                Log.Warning("Remove item called with null file item", true);
                return;
            }

            if (param.Item1.RemoveFile(param.Item2))
            {
                try
                {
                    FileSystem.DeleteFile(param.Item2.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Couldn't delete {param.Item2.FilePath}. Make sure it's not used by any process", true);
                    param.Item1.Add(param.Item2); // delete failed, so get item back to collection
                    return;
                }
            }
        }

        protected virtual void AddNewFile(FileList<T> collection)
        {
            if (collection == null)
            {
                Log.Warning("Can't add new file to null collection", true);
                return;
            }
            DialogResult result = OpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach (string filePath in OpenFileDialog.FileNames)
                {
                    string fileName = new FileInfo(filePath).Name;
                    string newFilePath = Path.Combine(collection.DestinationPath, fileName);
                    try
                    {
                        File.Copy(filePath, newFilePath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't add {fileName} to {collection.HeaderName}. Make sure the file is not opened by any process.", true);
                        continue;
                    }
                    collection.Add(newFilePath);
                }
            }
        }

        protected virtual ObservableCollection<FileList<T>> FindCollection(string path, bool createRootIfEmpty = false)
        {
            ObservableCollection<FileList<T>> found = null;
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

        private ObservableCollection<FileList<T>> CreateEmptyRoot(string path)
        {
            FileList<T> root = new FileList<T>(path);
            root.CollectionChanged += FileCollection_CollectionChanged;
            return new ObservableCollection<FileList<T>>() { root };
        }

        private static ObservableCollection<FileList<T>> DeserializeCollectionFromFile(string path)
        {
            string json = File.ReadAllText(path);
            try
            {
                return JsonConvert.DeserializeObject<ObservableCollection<FileList<T>>>(json);
            }
            catch (Exception ex)
            {
                Type loadType = typeof(ObservableCollection<FileList<T>>);
                Log.Error(ex, $"Couldnt load {loadType} from {json}");
                try
                {
                    return new ObservableCollection<FileList<T>>() { JsonConvert.DeserializeObject<FileList<T>>(json) };
                }
                catch (Exception ex2)
                {
                    loadType = typeof(FileList<T>);
                    Log.Error(ex2, $"Couldnt load {loadType} from {json}");
                }
            }
            return null;
        }

        protected ObservableCollection<FileList<T>> FindCollectionFromDirectory(string path)
        {
            List<FileList<T>> initCollection = new List<FileList<T>>(10);
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
                    FileList<T> fileCollection = new FileList<T>(directoryPath);
                    foreach (string filePath in files)
                    {
                        if (AllowedExtensions.Any(x => x == Path.GetExtension(filePath)))
                        {
                            fileCollection.Add(Path.GetFullPath(filePath).Replace('\\', '/'));
                        }
                    }
                    fileCollection.CollectionChanged += FileCollection_CollectionChanged;
                    initCollection.Add(fileCollection);
                }
            }
            return new ObservableCollection<FileList<T>>(initCollection);
        }

        protected virtual void FileCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
