using System.ComponentModel;

namespace ForgeModGenerator
{
    public interface IFileDialog : ICommonDialog
    {
        event CancelEventHandler FileOk;
        bool ValidateNames { get; set; }
        string Title { get; set; }
        bool SupportMultiDottedExtensions { get; set; }
        bool ShowHelp { get; set; }
        bool RestoreDirectory { get; set; }
        string InitialDirectory { get; set; }
        int FilterIndex { get; set; }
        string Filter { get; set; }
        string[] FileNames { get; }
        bool DereferenceLinks { get; set; }
        string DefaultExt { get; set; }
        bool CheckPathExists { get; set; }
        bool CheckFileExists { get; set; }
        bool AddExtension { get; set; }
        string FileName { get; set; }
    }
}
