using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using System.ComponentModel;

namespace ForgeModGenerator
{
    public class FileObject : ObservableModel, IFileObject
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

        public override object DeepClone() => new FileObject(Info.FullName);

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is IFileObject fileItem)
            {
                SetInfo(fileItem.Info.FullName);
                if (fileItem is ObservableModel model)
                {
                    SetValidateProperty(model);
                }
                return true;
            }
            return false;
        }
    }
}
