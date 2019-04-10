using ForgeModGenerator.Converters;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace ForgeModGenerator
{
    /// <summary> Wrapper for System.Windows.Forms.OpenFileDialog </summary>
    public sealed class FileBrowserDialog : IFileBrowser, IDisposable
    {
        private readonly OpenFileDialog dialog = new OpenFileDialog();

        public bool Multiselect { get => dialog.Multiselect; set => dialog.Multiselect = value; }
        public bool ReadOnlyChecked { get => dialog.ReadOnlyChecked; set => dialog.ReadOnlyChecked = value; }
        public bool ShowReadOnly { get => dialog.ShowReadOnly; set => dialog.ShowReadOnly = value; }

        public string SafeFileName => dialog.SafeFileName;
        public string[] SafeFileNames => dialog.SafeFileNames;

        public object Tag { get => dialog.Tag; set => dialog.Tag = value; }
        public bool ValidateNames { get => dialog.ValidateNames; set => dialog.ValidateNames = value; }
        public string Title { get => dialog.Title; set => dialog.Title = value; }
        public bool ShowHelp { get => dialog.ShowHelp; set => dialog.ShowHelp = value; }

        public bool RestoreDirectory { get => dialog.RestoreDirectory; set => dialog.RestoreDirectory = value; }
        public string InitialDirectory { get => dialog.InitialDirectory; set => dialog.InitialDirectory = value; }

        public int FilterIndex { get => dialog.FilterIndex; set => dialog.FilterIndex = value; }
        public string Filter { get => dialog.Filter; set => dialog.Filter = value; }

        public string FileName { get => dialog.FileName; set => dialog.FileName = value; }
        public string[] FileNames => dialog.FileNames;

        public bool AddExtension { get => dialog.AddExtension; set => dialog.AddExtension = value; }
        public bool SupportMultiDottedExtensions { get => dialog.SupportMultiDottedExtensions; set => dialog.SupportMultiDottedExtensions = value; }
        public string DefaultExt { get => dialog.DefaultExt; set => dialog.DefaultExt = value; }

        public bool CheckPathExists { get => dialog.CheckPathExists; set => dialog.CheckPathExists = value; }
        public bool CheckFileExists { get => dialog.CheckFileExists; set => dialog.CheckFileExists = value; }

        public bool DereferenceLinks { get => dialog.DereferenceLinks; set => dialog.DereferenceLinks = value; }

        public event EventHandler HelpRequest {
            add => dialog.HelpRequest += value;
            remove => dialog.HelpRequest -= value;
        }

        public event CancelEventHandler FileOk {
            add => dialog.FileOk += value;
            remove => dialog.FileOk -= value;
        }

        public Stream OpenFile() => dialog.OpenFile();

        public void Reset() => dialog.Reset();

        public DialogResult ShowDialog() => DialogResultAssemblyConverter.Convert(dialog.ShowDialog());
        
        public void Dispose() => dialog.Dispose();
    }
}
