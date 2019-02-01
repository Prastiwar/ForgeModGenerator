using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public interface IFileFolder : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        string HeaderName { get; }
        string DestinationPath { get; }
        bool RemoveFile(IFileItem fileItem);
    }

    public class FileList<T> : ObservableCollection<T>, IFileFolder
        where T : IFileItem
    {
        public delegate void OnFileChangedEventHandler(T file);
        public event OnFileChangedEventHandler OnFileAdded;
        public event OnFileChangedEventHandler OnFileRemoved;

        public string HeaderName { get; set; }
        public string DestinationPath { get; set; }

        public FileList(string destinationPath) : base()
        {
            DestinationPath = destinationPath;
            HeaderName = new DirectoryInfo(DestinationPath).Name;
            CollectionChanged += FileList_CollectionChanged;
        }

        private void FileList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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
                    ReferenceCounter.RemoveReference(file.FilePath, file);
                    OnFileRemoved?.Invoke(file);
                }
            }
        }

        public FileList(string destinationPath, IEnumerable<T> otherCollection) : this(destinationPath)
        {
            CopyFrom(otherCollection);
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = Items as IList<T>;
            if (collection != null && items != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        public void Add(string filePath)
        {
            Add(CreateFromPath(filePath));
        }

        public virtual T CreateFromPath(string filePath)
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt create instance of {typeof(T)} with {filePath}. Trying to create parameterless..");
                return Activator.CreateInstance<T>();
            }
        }

        public bool RemoveFile(IFileItem fileItem) => fileItem != null ? Remove((T)fileItem) : false;
    }
}
