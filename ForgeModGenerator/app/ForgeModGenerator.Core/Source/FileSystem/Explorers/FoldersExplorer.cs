using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public class FoldersExplorer<TFolder, TFile> : BindableBase, IFoldersExplorer<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FoldersExplorer(string foldersRootPath, IDialogService dialogService)
        {
            FoldersRootPath = foldersRootPath;
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(foldersRootPath));
            AllowedFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public string FoldersRootPath { get; set; }

        public FoldersFactory<TFolder, TFile> FolderFactory { get; set; }

        public FoldersSynchronizer<TFolder, TFile> FileSynchronizer { get; set; }

        public IFileBrowser OpenFileDialog { get; set; }

        public IFolderBrowser OpenFolderDialog { get; set; }

        public HashSet<string> AllowedFileExtensions { get; protected set; }

        public bool HasEmptyFolders => Folders.Files.Any(x => x.Count == 0);

        public string AllowedFileExtensionsPatterns => AllowedFileExtensions != null ? "*" + string.Join("|*", AllowedFileExtensions) : "";

        protected IDialogService DialogService { get; }

        private IFolderObject<TFolder> folders;
        public IFolderObject<TFolder> Folders {
            get => folders;
            set {
                SetProperty(ref folders, value);
                if (folders != null && FileSynchronizer != null)
                {
                    FileSynchronizer.SyncedFolders = folders;
                }
            }
        }

        public void RemoveEmptyFolders()
        {
            for (int i = Folders.Files.Count - 1; i >= 0; i--)
            {
                if (Folders.Files[i].Files.Count == 0)
                {
                    Folders.RemoveAt(i);
                }
            }
        }

        public DialogResult ShowFolderDialog(out IFolderBrowser browser)
        {
            browser = OpenFolderDialog;
            return OpenFolderDialog.ShowDialog();
        }

        public DialogResult ShowFileDialog(out IFileBrowser browser)
        {
            browser = OpenFileDialog;
            return OpenFileDialog.ShowDialog();
        }

        /// <summary> Copies directory to root path, if directory with given name exists, add (n) number to its name </summary>
        public async Task CopyFolderToRoot(string path)
        {
            path = IOHelper.GetDirectoryPath(path);
            string newFolderPath = IOHelper.GetUniqueName(Path.Combine(FoldersRootPath, new DirectoryInfo(path).Name), (name) => !Directory.Exists(name));
            await IOHelper.DirectoryCopyAsync(path, newFolderPath, AllowedFileExtensionsPatterns);
        }

        /// <summary> Copies files to folder path, if file with given name exists, prompt for overwriting </summary>
        public async Task CopyFilesToFolderAsync(TFolder folder, params string[] fileNames)
        {
            foreach (string filePath in fileNames)
            {
                string newPath = Path.Combine(folder.Info.FullName, Path.GetFileName(filePath));
                if (File.Exists(newPath))
                {
                    if (filePath != newPath)
                    {
                        bool overwrite = await DialogService.ShowMessage($"File {newPath} already exists.{Environment.NewLine}Do you want to overwrite it?", "Existing file conflict", "Yes", "No", null);
                        if (overwrite)
                        {
                            //if (!IOSafeWin.CopyFile(filePath, newPath, true))
                            //{
                            //    await DialogService.ShowMessage(IOSafeWin.GetOperationFailedMessage(filePath), "Copy failed");
                            //}
                        }
                    }
                    else
                    {
                        folder.Add(newPath);
                    }
                }
                else
                {
                    //if (!IOSafeWin.CopyFile(filePath, newPath))
                    //{
                    //    await DialogService.ShowMessage(IOSafeWin.GetOperationFailedMessage(filePath), "Copy failed");
                    //}
                }
            }
        }

        /// <summary> Removes folder and if it's empty, sends it to RecycleBin </summary>
        public void RemoveFolder(TFolder folder)
        {
            string folderPath = folder.Info.FullName;
            if (Folders.Remove(folder))
            {
                for (int i = folder.Files.Count - 1; i >= 0; i--)
                {
                    string filePath = folder.Files[i].Info.FullName;
                    if (FileSystemInfoReference.GetReferenceCount(filePath) <= 1 && File.Exists(filePath))
                    {
                        //IOSafeWin.DeleteFileRecycle(filePath);
                    }
                }
                folder.Clear();
                if (Directory.Exists(folderPath) && IOHelper.IsEmpty(folderPath))
                {
                    //IOSafeWin.DeleteDirectoryRecycle(folderPath);
                }
            }
        }

        public void RemoveFileFromFolder(TFolder folder, TFile file)
        {
            if (folder.Remove(file))
            {
                if (!FileSystemInfoReference.IsReferenced(file.Info.FullName))
                {
                    //if (!IOSafeWin.DeleteFileRecycle(param.Item2.Info.FullName))
                    //{
                    //    DialogService.ShowMessage(IOSafeWin.GetOperationFailedMessage(param.Item2.Info.FullName), "Deletion failed");
                    //    param.Item1.Add(param.Item2);
                    //}
                }
            }
        }

        public TFolder CreateFolder(string path) => FolderFactory.ConstructFolderInstance(path, null);
    }
}
