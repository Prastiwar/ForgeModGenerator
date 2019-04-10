using ForgeModGenerator.Converters;
using System;

namespace ForgeModGenerator
{
    /// <summary> Wrapper for System.Windows.Forms.FolderBrowserDialog </summary>
    public sealed class FolderBrowserDialog : IFolderBrowser, IDisposable
    {
        private readonly System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

        public event EventHandler HelpRequest {
            add => dialog.HelpRequest += value;
            remove => dialog.HelpRequest -= value;
        }

        public bool ShowNewFolderButton { get => dialog.ShowNewFolderButton; set => dialog.ShowNewFolderButton = value; }
        public string SelectedPath { get => dialog.SelectedPath; set => dialog.SelectedPath = value; }
        public Environment.SpecialFolder RootFolder { get => dialog.RootFolder; set => dialog.RootFolder = value; }
        public string Description { get => dialog.Description; set => dialog.Description = value; }
        public object Tag { get => dialog.Tag; set => dialog.Tag = value; }

        public void Reset() => dialog.Reset();

        public DialogResult ShowDialog() => DialogResultAssemblyConverter.Convert(dialog.ShowDialog());

        public void Dispose() => dialog.Dispose();
    }
}
