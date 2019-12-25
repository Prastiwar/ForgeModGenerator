using ForgeModGenerator.Utility;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Base ViewModel class to explore folders </summary>
    public abstract class FoldersWatcherViewModelBase<TFolder, TFile> : GeneratorViewModelBase<TFile>, IDisposable
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FoldersWatcherViewModelBase(GeneratorContext<TFile> context,
                                           IFoldersExplorerFactory<TFolder, TFile> explorerFactory)
            : base(context)
        {
            Explorer = explorerFactory.Create();
            Explorer.OpenFileDialog.Multiselect = true;
            Explorer.OpenFileDialog.CheckFileExists = true;
            Explorer.OpenFileDialog.ValidateNames = true;
            Explorer.OpenFolderDialog.ShowNewFolderButton = true;
        }

        public IFoldersExplorer<TFolder, TFile> Explorer { get; }

        public bool HasEmptyFolders => Explorer.HasEmptyFolders;

        private ICommand addFileCommand;
        public ICommand AddFileCommand => addFileCommand ?? (addFileCommand = new DelegateCommand<TFolder>(ShowFileDialogAndCopyToFolder));

        private ICommand removeFileCommand;
        public ICommand RemoveFileCommand => removeFileCommand ?? (removeFileCommand = new DelegateCommand<Tuple<TFolder, TFile>>(RemoveFileFromFolder));

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand => removeFolderCommand ?? (removeFolderCommand = new DelegateCommand<TFolder>(Explorer.RemoveFolder));

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand => addFolderCommand ?? (addFolderCommand = new DelegateCommand(ShowFolderDialogAndCopyToRoot));

        private ICommand addFileAsFolderCommand;
        public ICommand AddFileAsFolderCommand => addFileAsFolderCommand ?? (addFileAsFolderCommand = new DelegateCommand(ShowFileDialogAndCreateFolder));

        private ICommand removeEmptyFoldersCommand;
        public ICommand RemoveEmptyFoldersCommand => removeEmptyFoldersCommand ?? (removeEmptyFoldersCommand = new DelegateCommand(Explorer.RemoveEmptyFolders));

        protected override bool CanRefresh() => SessionContext.SelectedMod != null && Directory.Exists(DirectoryRootPath);

        protected override void RemoveItem(TFile item) => throw new NotImplementedException(); // TODO: Refactor RemoveFileFromFolder
        protected void RemoveFileFromFolder(Tuple<TFolder, TFile> param) => Explorer.RemoveFileFromFolder(param.Item1, param.Item2);

        protected async Task InitializeFoldersAsync(IEnumerable<TFolder> folders)
        {
            foreach (TFolder folder in folders)
            {
                ObservableRangeCollection<TFile> temp = folder.Files.DeepCollectionClone<ObservableRangeCollection<TFile>, TFile>();
                folder.Clear();
                Explorer.Folders.Add(folder);
                foreach (TFile file in temp)
                {
                    folder.Add(file);
                    await Task.Delay(1).ConfigureAwait(true);
                }
            }
        }

        protected async void ShowFolderDialogAndCopyToRoot()
        {
            DialogResult dialogResult = Explorer.ShowFolderDialog(out IFolderBrowser browser);
            if (dialogResult == DialogResult.OK)
            {
                await Explorer.CopyFolderToRootAsync(DirectoryRootPath, browser.SelectedPath).ConfigureAwait(false);
            }
        }

        protected void ShowFileDialogAndCreateFolder()
        {
            DialogResult dialogResult = Explorer.ShowFileDialog(out IFileBrowser browser);
            if (dialogResult == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(browser.FileName);
                string newFolderPath = Path.Combine(DirectoryRootPath, fileName);
                string newFolderName = IOHelper.GetUniqueName(fileName, name => !Directory.Exists((newFolderPath = Path.Combine(DirectoryRootPath, name))));
                Directory.CreateDirectory(newFolderPath);
                TFolder folder = Explorer.CreateFolder(newFolderPath);
                Explorer.CopyFilesToFolder(folder, browser.FileNames);
            }
        }

        protected void ShowFileDialogAndCopyToFolder(TFolder folder)
        {
            DialogResult dialogResult = Explorer.ShowFileDialog(out IFileBrowser browser);
            if (dialogResult == DialogResult.OK)
            {
                Explorer.CopyFilesToFolder(folder, browser.FileNames);
            }
        }

        protected override void EditItem(TFile item)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(item.Info.FullName) {
                    Verb = "edit"
                };
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Explorer.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
