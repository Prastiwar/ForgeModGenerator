﻿using System;
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
        public string HeaderName { get; set; }
        public string DestinationPath { get; set; }

        public FileList(string destinationPath) : base()
        {
            DestinationPath = destinationPath;
            HeaderName = new DirectoryInfo(DestinationPath).Name;
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
            catch (Exception)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public bool RemoveFile(IFileItem fileItem) => fileItem != null ? Remove((T)fileItem) : false;
    }
}