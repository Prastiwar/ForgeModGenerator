using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ForgeModGenerator
{
    public enum FileChange { Add, Remove }

    public delegate void OnFileChangedEventHandler<TFile>(object sender, FileChangedEventArgs<TFile> e) where TFile : IFileSystemObject;
    public delegate void OnFilePropertyChangedEventHandler<T>(T sender, PropertyChangedEventArgs e);

    public class FileChangedEventArgs<TFile> : EventArgs where TFile : IFileSystemObject
    {
        public FileChangedEventArgs(IEnumerable<TFile> files, FileChange change)
        {
            Files = files ?? throw new ArgumentNullException(nameof(files));
            File = files.FirstOrDefault();
            Change = change;
        }

        public TFile File { get; }
        public IEnumerable<TFile> Files { get; }
        public FileChange Change { get; }
    }

    public interface IFolderObject : IFileSystemObject, IDirty, INotifyCollectionChanged, INotifyPropertyChanged
    {
        bool Add(string filePath);
        void AddRange(IEnumerable<string> filePaths);
        void Clear();
    }

    public interface IFolderObject<T> : IFolderObject where T : IFileSystemObject
    {
        event OnFileChangedEventHandler<T> FilesChanged;
        event OnFilePropertyChangedEventHandler<T> FilePropertyChanged;

        ObservableRangeCollection<T> Files { get; }

        int Count { get; }

        void AddRange(IEnumerable<T> items);
        bool Add(T item);

        bool Remove(T item);
        bool RemoveAt(int index);

        bool Contains(T item);
        bool TryGetFile(string path, out T folder);
    }
}
