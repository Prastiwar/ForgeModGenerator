using System.ComponentModel;

namespace ForgeModGenerator
{
    public interface IFileSystemObject : INotifyPropertyChanged, ICopiable, IDirty
    {
        FileSystemInfoReference Info { get; }

        void SetInfo(string path);
        void Rename(string newName);
    }
}
