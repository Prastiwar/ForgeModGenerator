using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ForgeModGenerator
{
    /// <summary>
    /// Observable collection of file objects. It doesn't make use of any IO operations
    /// Default implementation for IFolderObject<T>
    /// </summary>
    public class ObservableFolder<T> : ObservableDirtyObject, IFolderObject<T>
        where T : IFileSystemObject
    {
        internal static ObservableFolder<T> CreateEmpty() => new ObservableFolder<T>();

        protected ObservableFolder() => Files = new ObservableRangeCollection<T>();

        public ObservableFolder(string path)
        {
            ThrowExceptionIfInvalid(path);
            Files = new ObservableRangeCollection<T>();
            SetInfo(path);
            IsDirty = false;
        }

        public ObservableFolder(IEnumerable<string> filePaths) : this(filePaths?.First(), filePaths) { }
        public ObservableFolder(string path, IEnumerable<string> filePaths) : this(path)
        {
            if (filePaths != null)
            {
                AddRange(filePaths);
            }
            IsDirty = false;
        }

        public ObservableFolder(IEnumerable<T> files) : this(files?.FirstOrDefault()?.Info.FullName, files) { }
        public ObservableFolder(string path, IEnumerable<T> files) : this(path)
        {
            if (files != null)
            {
                AddRange(files);
            }
            IsDirty = false;
        }

        public event OnFileChangedEventHandler<T> FilesChanged;
        public event OnFilePropertyChangedEventHandler<T> FilePropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private FileSystemInfoReference info;
        public FileSystemInfoReference Info {
            get => info;
            private set => DirtSetProperty(ref info, value);
        }

        private ObservableRangeCollection<T> files;
        public ObservableRangeCollection<T> Files {
            get => files;
            protected set => InitializeFiles(value);
        }

        public int Count => Files != null ? Files.Count : 0;

        public bool Contains(T item) => Files.Contains(item);

        /// <summary> Removes files from collection </summary>
        public void RemoveFileRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged -= OnFilePropertyChanged;
            }
            Files.RemoveRange(items);
        }

        /// <summary> Enumerates files that are sub paths of given folder path </summary>
        public IEnumerable<T> EnumerateSubPathFiles(string folderPath) => Files.Where(file => IOHelper.IsSubPathOf(file.Info.FullName, folderPath));

        public bool TryGetFile(string path, out T file)
        {
            file = Files.Find(x => x.Info.FullName.ComparePath(path));
            return file != null;
        }

        public bool Add(T item)
        {
            if (CanAdd(item))
            {
                AddFile(item);
                return true;
            }
            return false;
        }

        public bool Add(string filePath)
        {
            if (CanAdd(filePath))
            {
                AddFile(CreateFileFromPath(filePath));
                return true;
            }
            return false;
        }

        public void AddRange(IEnumerable<T> items)
        {
            IEnumerable<T> itemsToAdd = items.Where(item => CanAdd(item));
            AddFileRange(itemsToAdd.ToList());
        }

        public void AddRange(IEnumerable<string> filePaths)
        {
            IEnumerable<string> filePathsToAdd = filePaths.Where(filePath => CanAdd(filePath));
            IEnumerable<T> files = filePathsToAdd.Select(filePath => CreateFileFromPath(filePath));
            AddFileRange(files.ToList());
        }

        public bool RemoveAt(int index)
        {
            if (index >= 0 && index < Files.Count)
            {
                T file = Files[index];
                file.PropertyChanged -= OnFilePropertyChanged;
                file.Info.Dispose();
                Files.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            if (Files.Remove(item))
            {
                item.PropertyChanged -= OnFilePropertyChanged;
                item.Info.Dispose();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            foreach (T file in Files)
            {
                file.PropertyChanged -= OnFilePropertyChanged;
                file.Info.Dispose();
            }
            Files.Clear();
        }

        public void Rename(string newName) => Info.Info.Rename(newName);

        /// <summary> Initialize DirectoryInfoReference or rename </summary>
        public void SetInfo(string path)
        {
            path = IOHelper.GetDirectoryPath(path);
            if (Info != null)
            {
                Info.SetInfo(path);
            }
            else
            {
                Info = new FileSystemInfoReference(path);
                Info.PropertyChanged += OnInfoPropertyChanged;
            }
        }

        protected virtual bool CanAdd(T item) => CanAdd(item.Info.FullName);
        protected virtual bool CanAdd(string filePath) => !Files.Exists(file => file.Info.FullName.ComparePath(filePath));

        /// <summary> Forces Add file without CanAdd() check </summary>
        protected void AddFile(T item)
        {
            Files.Add(item);
            item.PropertyChanged += OnFilePropertyChanged;
        }

        /// <summary> Forces Add files without CanAdd() check </summary>
        protected void AddFileRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged += OnFilePropertyChanged;
            }
            Files.AddRange(items);
        }

        protected T CreateFileFromPath(string filePath)
        {
            try
            {
                return ReflectionHelper.CreateInstance<T>(filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt create instance of {typeof(T)} with {filePath}. Trying to create parameterless..");
                return Activator.CreateInstance<T>();
            }
        }

        /// <summary> Setter for Files </summary>
        protected virtual void InitializeFiles(ObservableRangeCollection<T> value)
        {
            if (DirtSetProperty(ref files, value))
            {
                if (Files != null)
                {
                    Files.CollectionChanged -= OnFilesCollectionChanged;
                    Files.CollectionChanged += OnFilesCollectionChanged;
                    if (Files.Count > 0)
                    {
                        foreach (T file in Files)
                        {
                            file.PropertyChanged -= OnFilePropertyChanged;
                            file.PropertyChanged += OnFilePropertyChanged;
                        }
                    }
                }
            }
        }

        protected virtual void OnFilePropertyChanged(object sender, PropertyChangedEventArgs e) => FilePropertyChanged?.Invoke((T)sender, e);

        protected virtual void OnInfoPropertyChanged(object sender, PropertyChangedEventArgs e) { }

        protected virtual void OnFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged(nameof(Files));
            CollectionChanged?.Invoke(sender, e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                FilesChanged?.Invoke(this, new FileChangedEventArgs<T>(e.NewItems.Cast<T>(), FileChange.Add));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                FilesChanged?.Invoke(this, new FileChangedEventArgs<T>(e.OldItems.Cast<T>(), FileChange.Remove));
            }
        }

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone()
        {
            ObservableRangeCollection<T> cloneFiles = new ObservableRangeCollection<T>();
            foreach (T file in Files)
            {
                cloneFiles.Add((T)file.DeepClone());
            }
            ObservableFolder<T> folder = new ObservableFolder<T>() { Files = new ObservableRangeCollection<T>() };
            folder.AddRange(cloneFiles);
            folder.SetInfo(Info.FullName);
            folder.IsDirty = false;
            return folder;
        }

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is ObservableFolder<T> item)
            {
                Files = item.Files;
                SetInfo(item.Info.FullName);
                return true;
            }
            return false;
        }

        protected void ThrowExceptionIfInvalid(string path)
        {
            if (!IOHelper.IsPathValid(path))
            {
                Log.Error(null, $"Called {nameof(ObservableFolder<T>)} constructor with invalid path parameter, {nameof(path)}");
                throw new ArgumentException("Invalid Path", nameof(path));
            }
        }
    }
}
