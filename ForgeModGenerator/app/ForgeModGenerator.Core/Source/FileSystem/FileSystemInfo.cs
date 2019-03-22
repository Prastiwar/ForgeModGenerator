using System.ComponentModel;

namespace ForgeModGenerator
{
    public interface IFileSystemInfo : INotifyPropertyChanged, ICopiable, IDirty
    {
        FileSystemInfoReference Info { get; }
        void SetInfo(string path);
    }
}
