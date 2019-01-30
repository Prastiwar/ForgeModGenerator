using GalaSoft.MvvmLight;
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

    public interface IFileItem : INotifyPropertyChanged, ICopiable
    {
        string FileName { get; }
        string FilePath { get; }
        void SetFileItem(string filePath);
    }

    public class FileItem : ObservableObject, IFileItem
    {
        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }

        private FileItem() { }

        public FileItem(string filePath)
        {
            SetFileItem(filePath);
        }

        public void SetFileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }

        public object Clone() => MemberwiseClone();

        public object DeepClone() => new FileItem() { FileName = FileName, FilePath = FilePath };

        public bool CopyValues(object fromCopy)
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
