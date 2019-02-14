﻿using System;
using System.ComponentModel;

namespace ForgeModGenerator.Models
{
    public interface ICopiable : ICloneable
    {
        bool CopyValues(object fromCopy);
        object DeepClone();
    }

    public interface IFileSystemInfo : INotifyPropertyChanged, ICopiable, IDirty
    {
        FileSystemInfoReference Info { get; }
        bool SetInfo(string path);
    }

    public interface IFileItem : IFileSystemInfo { }

    public class FileItem : ObservableDirtyObject, IFileItem
    {
        protected FileItem() { }

        public FileItem(string filePath) => SetInfo(filePath);

        private FileSystemInfoReference info;
        public FileSystemInfoReference Info {
            get => info;
            private set => DirtSet(ref info, value);
        }

        public virtual bool SetInfo(string path)
        {
            if (Info != null)
            {
                return Info.Rename(path);
            }
            Info = new FileInfoReference(path);
            Info.PropertyChanged += Info_PropertyChanged;
            return true;
        }

        protected virtual void Info_PropertyChanged(object sender, PropertyChangedEventArgs e) { }

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new FileItem(Info.FullName);

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is IFileItem fileItem)
            {
                SetInfo(fileItem.Info.FullName);
                return true;
            }
            return false;
        }
    }
}