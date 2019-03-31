using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public interface IFoldersExplorer<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        /// <summary> Path to folder root where are all files localized </summary>
        string FoldersRootPath { get; set; }

        IFolderObject<TFolder> Folders { get; set; }

        FoldersFactory<TFolder, TFile> FolderFactory { get; set; }

        FoldersSynchronizer<TFolder, TFile> FileSynchronizer { get; set; }

        IFileBrowser OpenFileDialog { get; set; }

        IFolderBrowser OpenFolderDialog { get; set; }

        HashSet<string> AllowedFileExtensions { get; }

        string AllowedFileExtensionsPatterns { get; }

        bool HasEmptyFolders { get; }

        /// <summary> Removes folder and if it's empty, sends it to RecycleBin </summary>
        void RemoveFolder(TFolder folder);

        void RemoveEmptyFolders();

        TFolder CreateFolder(string path);

        DialogResult ShowFolderDialog(out IFolderBrowser browser);

        DialogResult ShowFileDialog(out IFileBrowser browser);

        Task CopyFolderToRoot(string path);

        Task CopyFilesToFolderAsync(TFolder folder, params string[] fileNames);

        void RemoveFileFromFolder(TFolder folder, TFile file);
    }
}
