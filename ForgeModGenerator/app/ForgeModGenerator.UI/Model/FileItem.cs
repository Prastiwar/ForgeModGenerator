using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public interface IFileItem : INotifyPropertyChanged
    {
        string FileName { get; }
        string FilePath { get; }
        void SetFileItem(string filePath);
    }

    public class FileItem : ObservableObject, IFileItem
    {
        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }

        public FileItem(string filePath)
        {
            SetFileItem(filePath);
        }

        public void SetFileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }
    }
}
