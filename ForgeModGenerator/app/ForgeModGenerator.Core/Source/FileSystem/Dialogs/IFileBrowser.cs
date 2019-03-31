using System.IO;

namespace ForgeModGenerator
{
    public interface IFileBrowser : IFileDialog
    {
        bool Multiselect { get; set; }
        bool ReadOnlyChecked { get; set; }
        bool ShowReadOnly { get; set; }
        string SafeFileName { get; }
        string[] SafeFileNames { get; }
        Stream OpenFile();
    }
}
