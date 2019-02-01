using System;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public interface ICopiable : ICloneable
    {
        bool CopyValues(object fromCopy);
        object DeepClone();
    }

    public interface IFileItem : INotifyPropertyChanged, ICopiable, IDirty
    {
        string FileName { get; }
        string FilePath { get; }
        void SetFileItem(string filePath);
        object DeepClone(bool countReference);
    }

    public class FileItem : ObservableDirtyObject, IFileItem
    {
        protected FileItem() { }

        public FileItem(string filePath) => SetFileItem(filePath);

        ~FileItem() => ReferenceCounter.RemoveReference(FilePath, this);

        private string fileName;
        public string FileName {
            get => fileName;
            protected set => Set(ref fileName, value);
        }

        protected string filePath;
        public string FilePath {
            get => filePath;
            protected set {
                ReferenceCounter.RemoveReference(filePath, this);
                Set(ref filePath, value);
                ReferenceCounter.AddReference(filePath, this);
            }
        }

        public virtual void SetFileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }

        public virtual bool ShouldSerializeFilePath() => true;

        public virtual bool ShouldSerializeFileName() => true;

        public virtual object Clone() => MemberwiseClone();

        public virtual object DeepClone() => DeepClone(true);

        public virtual object DeepClone(bool countReference) => countReference ? new FileItem() { FileName = FileName, FilePath = FilePath } : new FileItem() { FileName = FileName, filePath = FilePath };

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is IFileItem fileItem)
            {
                FileName = fileItem.FileName;
                FilePath = fileItem.FilePath;
                return true;
            }
            return false;
        }
    }
}
