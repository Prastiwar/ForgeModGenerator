using ForgeModGenerator.Miscellaneous;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Model
{
    public delegate void OnFileChangedEventHandler<T>(T file);

    public interface IFileFolder : IFileSystemInfo, IDirty, INotifyCollectionChanged, INotifyPropertyChanged
    {
        void Add(string filePath);
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
            if (!IOExtensions.IsPathValid(path))
            {
                Log.Error(null, $"Called ObservableFolder constructor with invalid path parameter, {nameof(path)}");
                throw new ArgumentException("Invalid Path", nameof(path));
            }

            SetInfo(path);
            Files = new ObservableCollection<T>();
            IsDirty = false;
        }

        public ObservableFolder(IEnumerable<T> files)
        {
            if (files == null)
            {
                Log.Error(null, "Called ObservableFolder constructor with null files parameter");
                throw new ArgumentNullException(nameof(files));
            }
            else if (files.Count() <= 0)
            {
                Log.Error(null, "Called ObservableFolder constructor with files count <= 0 parameter");
                throw new Exception($"{nameof(files)} must have at least one occurency.");
            }

            Files = new ObservableCollection<T>(files);
            SetInfo(Files[0].Info.FullName);
            IsDirty = false;
        }

        public ObservableFolder(string path, IEnumerable<T> files)
        {
            if (!IOExtensions.IsPathValid(path))
            {
                throw new ArgumentException("Invalid Path", nameof(path));
            }
            else if (files == null)
            {
                Log.Error(null, "Called ObservableFolder constructor with null files parameter");
                throw new ArgumentNullException(nameof(files));
            }

            SetInfo(path);
            Files = new ObservableCollection<T>(files);
            IsDirty = false;
        }

        public ObservableFolder(string path, System.IO.SearchOption searchOption) : this(path, "*", searchOption) { }
        public ObservableFolder(string path, string fileSearchPattern) : this(path, fileSearchPattern, System.IO.SearchOption.TopDirectoryOnly) { }
        public ObservableFolder(string path, string fileSearchPattern, System.IO.SearchOption searchOption) : this(path)
        {
            foreach (string filePath in Directory.EnumerateFiles(path, fileSearchPattern, searchOption))
            {
                Add(filePath);
            }
            IsDirty = false;
        }

        public event OnFileChangedEventHandler<T> OnFileAdded;
        public event OnFileChangedEventHandler<T> OnFileRemoved;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private FileSystemInfo info;
        public FileSystemInfo Info {
            get => info;
            set => DirtSet(ref info, value);
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

        public virtual void SetInfo(string path)
        {
            ReferenceCounter.RemoveReference(info?.FullName, this);
            Info = new DirectoryInfo(IOExtensions.IsDirectoryPath(path) ? path : Path.GetDirectoryName(path));
            ReferenceCounter.AddReference(info.FullName, this);
        }

        [JsonIgnore]
        public int Count => Files.Count;

        public void Clear() => Files.Clear();
        public bool Contains(T item) => Files.Contains(item);

        public void Add(T item) => Files.Add(item);

        // Copy file from filePath to folder path and add to collection
        public virtual void Add(string filePath)
        {
            string fileName = new FileInfo(filePath).Name;
            string newFilePath = Path.Combine(Info.FullName, fileName);
            try
            {
                if (!File.Exists(newFilePath))
                {
                    File.Copy(filePath, newFilePath);
                }
                else if (Files.Any(x => x.Info.FullName == newFilePath))
                {
                    Log.Warning($"File {newFilePath} already exists.", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't add {fileName} to {Info.Name}. Make sure the file is not opened by any process.", true);
                return;
            }
            Add(CreateFileFromPath(newFilePath));
        }

        // Removes file from collection and if is not referenced in application, remove from explorer
        public virtual bool Remove(T item)
        {
            if (Files.Remove(item))
            {
                if (!ReferenceCounter.IsReferenced(item.Info.FullName))
                {
                    try
                    {
                        FileSystem.DeleteFile(item.Info.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Couldn't delete {item.Info.FullName}. Make sure it's not used by any process", true);
                        Files.Add(item); // delete failed, so get item back to collection
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        // Removes folder with all his content
        public virtual void Delete()
        {
            int length = Files.Count;
            for (int i = 0; i < length; i++)
            {
                Remove(Files[i]);
            }
        }

        protected virtual T CreateFileFromPath(string filePath)
        {
            try
            {
                return Util.CreateInstance<T>(filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt create instance of {typeof(T)} with {filePath}. Trying to create parameterless..");
                return Activator.CreateInstance<T>();
            }
        }

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
                    ReferenceCounter.RemoveReference(file.Info.FullName, file);
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
    }
}
