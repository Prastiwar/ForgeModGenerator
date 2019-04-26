﻿using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using System.ComponentModel;

namespace ForgeModGenerator
{
    public class FileObject : ObservableDirtyObject, IFileObject
    {
        protected FileObject() { }

        public FileObject(string filePath) => SetInfo(filePath);

        private FileSystemInfoReference info;
        public FileSystemInfoReference Info {
            get => info;
            private set => DirtSetProperty(ref info, value);
        }

        public void SetInfo(string path)
        {
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

        public void Rename(string newName)
        {
            Info.Info.Rename(newName);
            RaisePropertyChanged(nameof(Info));
        }

        protected virtual void OnInfoPropertyChanged(object sender, PropertyChangedEventArgs e) { }

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new FileObject(Info.FullName);

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is IFileObject fileItem)
            {
                SetInfo(fileItem.Info.FullName);
                return true;
            }
            return false;
        }
    }
}
