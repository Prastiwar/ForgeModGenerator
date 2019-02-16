using ForgeModGenerator.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Models
{
    public delegate void OnFileChangedEventHandler<T>(T file);

    public interface IFileFolder : IFileSystemInfo, IDirty, INotifyCollectionChanged, INotifyPropertyChanged
    {
        void Add(string filePath, bool overwrite = false);
        void Clear();
    }

    public interface IFileFolder<T> : IFileFolder where T : IFileSystemInfo
    {
        event OnFileChangedEventHandler<T> OnFileAdded;
        event OnFileChangedEventHandler<T> OnFileRemoved;

        ObservableCollection<T> Files { get; }

        void Add(T item);
        bool Remove(T item);
        bool Contains(T item);
        void Delete();
    }

    public class ObservableFolder<T> : ObservableDirtyObject, IFileFolder<T>
        where T : IFileSystemInfo
    {
        protected ObservableFolder() { }

        public ObservableFolder(string path)
        {
            ThrowExceptionIfInvalid(path);
            Files = new ObservableCollection<T>();
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

        public ObservableFolder(string path, SearchOption searchOption) : this(path, "*", searchOption) { }
        public ObservableFolder(string path, string fileSearchPatterns) : this(path, fileSearchPatterns, SearchOption.TopDirectoryOnly) { }
        public ObservableFolder(string path, string fileSearchPatterns, SearchOption searchOption) : this(path)
        {
            foreach (string filePath in IOExtensions.EnumerateFiles(path, fileSearchPatterns, searchOption))
            {
                Add(filePath);
            }
            IsDirty = false;
        }

        public event OnFileChangedEventHandler<T> OnFileAdded;
        public event OnFileChangedEventHandler<T> OnFileRemoved;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private FileSystemInfoReference info;
        public FileSystemInfoReference Info {
            get => info;
            private set => DirtSet(ref info, value);
        }

        private ObservableCollection<T> files;
        public ObservableCollection<T> Files {
            get => files;
            protected set {
                if (DirtSet(ref files, value))
                {
                    if (Files != null)
                    {
                        Files.CollectionChanged -= Files_CollectionChanged;
                        Files.CollectionChanged += Files_CollectionChanged;
                    }
                }
            }
        }

        [JsonIgnore]
        public int Count => Files.Count;

        [JsonIgnore]
        protected DirectoryInfo DirInfo => (DirectoryInfo)Info.FileSystemInfo;

        public void Clear() => Files.Clear();
        public bool Contains(T item) => Files.Contains(item);

        public void Add(T item) => Files.Add(item);

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public void AddRange(IEnumerable<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                Add(filePath);
            }
        }

        public virtual void Add(string filePath, bool overwrite = false)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            int index = Files.FindIndex(x => x.Info.FullName == fileInfo.FullName);
            if (index != -1)
            {
                if (overwrite)
                {
                    Files[index] = CreateFileFromPath(fileInfo.FullName);
                }
                else
                {
                    Log.Warning($"File {fileInfo.FullName} already exists.", true);
                }
            }
            else
            {
                Add(CreateFileFromPath(fileInfo.FullName));
            }
        }

        public virtual bool Remove(T item)
        {
            if (Files.Remove(item))
            {
                item.Info.Remove();
                return true;
            }
            return false;
        }

        // Removes folder with all his content
        public virtual void Delete()
        {
            for (int i = Files.Count - 1; i > 0; i--)
            {
                Remove(Files[i]);
            }
        }

        // Initialize DirectoryInfoReference or rename
        public virtual void SetInfo(string path)
        {
            if (IOExtensions.IsFilePath(path))
            {
                path = new FileInfo(path).Directory.FullName;
            }
            if (Info != null)
            {
                Info.SetInfo(path);
            }
            else
            {
                Info = new DirectoryInfoReference(path);
                Info.PropertyChanged += Info_PropertyChanged;
            }
        }

        protected virtual T CreateFileFromPath(string filePath)
        {
            try
            {
                return WPFExtensions.CreateInstance<T>(filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt create instance of {typeof(T)} with {filePath}. Trying to create parameterless..");
                return Activator.CreateInstance<T>();
            }
        }

        protected virtual void Info_PropertyChanged(object sender, PropertyChangedEventArgs e) { }

        protected virtual void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
            RaisePropertyChanged(nameof(Count));
            CollectionChanged?.Invoke(sender, e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    T file = (T)item;
                    OnFileAdded?.Invoke(file);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    T file = (T)item;
                    OnFileRemoved?.Invoke(file);
                }
            }
        }

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone()
        {
            ObservableCollection<T> cloneFiles = new ObservableCollection<T>();
            foreach (T file in Files)
            {
                cloneFiles.Add((T)file.DeepClone());
            }
            ObservableFolder<T> folder = new ObservableFolder<T>() { Files = cloneFiles };
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

        #region Constructor Checks
        protected void ThrowExceptionIfInvalid(string path)
        {
            if (!IOExtensions.IsPathValid(path))
            {
                Log.Error(null, $"Called ObservableFolder constructor with invalid path parameter, {nameof(path)}");
                throw new ArgumentException("Invalid Path", nameof(path));
            }
        }
        #endregion
    }
}
