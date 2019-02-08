using System;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Models
{
    public interface ICopiable : ICloneable
    {
        bool CopyValues(object fromCopy);
        object DeepClone();
    }

    public interface IFileSystemInfo : INotifyPropertyChanged, ICopiable, IDirty
    {
        FileSystemInfo Info { get; }
        void SetInfo(string path);
    }

    public interface IFileItem : IFileSystemInfo { }

    public class FileItem : ObservableDirtyObject, IFileItem
    {
        protected FileItem() { }

        public FileItem(string filePath) => SetInfo(filePath);

        ~FileItem() => ReferenceCounter.RemoveReference(info?.FullName, this);

        private FileSystemInfo info;
        public FileSystemInfo Info {
            get => info;
            private set => DirtSet(ref info, value);
        }

        public virtual void SetInfo(string filePath)
        {
            ReferenceCounter.RemoveReference(info?.FullName, this);
            Info = new FileInfo(filePath);
            ReferenceCounter.AddReference(info.FullName, this);
        }

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new FileInfo(Info.FullName);

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
