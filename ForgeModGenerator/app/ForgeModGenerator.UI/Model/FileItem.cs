using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public interface IFileItem : INotifyPropertyChanged
    {
        string FileName { get; }
        string FilePath { get; }
    }

    public class FileItem : ObservableObject, IFileItem
    {
        public string FileName { get; }
        public string FilePath { get; }

        public FileItem(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }
    }
}
