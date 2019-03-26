using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace ForgeModGenerator
{
    public class WpfObservableFolder<T> : ObservableDirtyObject, IFolderObject<T>
        where T : IFileSystemObject
    {
        protected WpfObservableFolder() { }

        public WpfObservableFolder(string path)
        {
            ThrowExceptionIfInvalid(path);
            Files = new WpfObservableRangeCollection<T>();
            SetInfo(path);
            IsDirty = false;
        }

        public WpfObservableFolder(IEnumerable<string> filePaths) : this(filePaths?.First(), filePaths) { }
        public WpfObservableFolder(string path, IEnumerable<string> filePaths) : this(path)
        {
            if (filePaths != null)
            {
                AddRange(filePaths);
            }
            IsDirty = false;
        }

        public WpfObservableFolder(IEnumerable<T> files) : this(files?.FirstOrDefault()?.Info.FullName, files) { }
        public WpfObservableFolder(string path, IEnumerable<T> files) : this(path)
        {
            if (files != null)
            {
                AddRange(files);
            }
            IsDirty = false;
        }

        public WpfObservableFolder(string path, SearchOption searchOption) : this(path, "*", searchOption) { }
        public WpfObservableFolder(string path, string fileSearchPatterns) : this(path, fileSearchPatterns, SearchOption.TopDirectoryOnly) { }
        public WpfObservableFolder(string path, string fileSearchPatterns, SearchOption searchOption) : this(path)
        {
            AddRange(IOHelper.EnumerateFiles(path, fileSearchPatterns, searchOption));
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
            protected set {
                if (value != null && !(value is WpfObservableRangeCollection<T>))
                {
                    value = new WpfObservableRangeCollection<T>(value);
                }
                if (DirtSetProperty(ref files, value))
                {
                    if (Files != null)
                    {
                        Files.CollectionChanged -= Files_CollectionChanged;
                        Files.CollectionChanged += Files_CollectionChanged;
                        if (Files.Count > 0)
                        {
                            foreach (T file in Files)
                            {
                                file.PropertyChanged -= File_PropertyChanged;
                                file.PropertyChanged += File_PropertyChanged;
                            }
                        }
                    }
                }
            }
        }

        [JsonIgnore]
        public int Count => Files != null ? Files.Count : 0;

        public bool Contains(T item) => Files.Contains(item);

        protected virtual bool CanAdd(T item) => CanAdd(item.Info.FullName);
        protected virtual bool CanAdd(string filePath) => !Files.Exists(file => file.Info.FullName.ComparePath(filePath));

        /// <summary> Add file without existing check </summary>
        protected void AddFile(T item)
        {
            Files.Add(item);
            item.PropertyChanged += File_PropertyChanged;
        }

        /// <summary> Add files without existing check </summary>
        protected void AddFileRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged += File_PropertyChanged;
            }
            Files.AddRange(items);
        }

        /// <summary> Add files without existing check </summary>
        protected void RemoveFileRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged -= File_PropertyChanged;
            }
            Files.RemoveRange(items);
        }

        /// <summary> Enumerates files that are sub paths of given folder path </summary>
        public IEnumerable<T> EnumerateSubPathFiles(string path) => Files.Where(file => IOHelper.IsSubPathOf(file.Info.FullName, path));

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
                file.PropertyChanged -= File_PropertyChanged;
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
                item.PropertyChanged -= File_PropertyChanged;
                item.Info.Dispose();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            foreach (T file in Files)
            {
                file.PropertyChanged -= File_PropertyChanged;
                file.Info.Dispose();
            }
            Files.Clear();
        }

        public void Rename(string newName) => IOHelper.RenameDirectory(Info.FullName, newName);

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
                Info.PropertyChanged += Info_PropertyChanged;
            }
        }

        protected T CreateFileFromPath(string filePath)
        {
            try
            {
                return ReflectionExtensions.CreateInstance<T>(filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt create instance of {typeof(T)} with {filePath}. Trying to create parameterless..");
                return Activator.CreateInstance<T>();
            }
        }

        protected virtual void File_PropertyChanged(object sender, PropertyChangedEventArgs e) => FilePropertyChanged?.Invoke((T)sender, e);

        protected virtual void Info_PropertyChanged(object sender, PropertyChangedEventArgs e) { }

        protected virtual void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            ObservableCollection<T> cloneFiles = new ObservableCollection<T>();
            foreach (T file in Files)
            {
                cloneFiles.Add((T)file.DeepClone());
            }
            WpfObservableFolder<T> folder = new WpfObservableFolder<T>() { Files = new WpfObservableRangeCollection<T>() };
            folder.AddRange(cloneFiles);
            folder.SetInfo(Info.FullName);
            folder.IsDirty = false;
            return folder;
        }

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is WpfObservableFolder<T> item)
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
                Log.Error(null, $"Called WpfObservableFolder constructor with invalid path parameter, {nameof(path)}");
                throw new ArgumentException("Invalid Path", nameof(path));
            }
        }
    }
}
