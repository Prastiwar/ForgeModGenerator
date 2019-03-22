using ForgeModGenerator.Models;
using System.ComponentModel;

namespace ForgeModGenerator
{
    public interface IFileItem : IFileSystemInfo { }

    public class FileItem : ObservableDirtyObject, IFileItem
    {
        protected FileItem() { }

        public FileItem(string filePath) => SetInfo(filePath);

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
                Info = new FileInfoReference(path);
                Info.PropertyChanged += Info_PropertyChanged;
            }
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
