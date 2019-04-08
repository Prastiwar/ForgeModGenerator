using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public interface IFoldersExplorer<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFolderObject<TFolder> Folders { get; }

        IFolderSynchronizer<TFolder, TFile> FileSynchronizer { get; }

        IFileBrowser OpenFileDialog { get; }

        IFolderBrowser OpenFolderDialog { get; }

        HashSet<string> AllowedFileExtensions { get; }

        bool HasEmptyFolders { get; }

        void RemoveFolder(TFolder folder);

        void RemoveEmptyFolders();

        TFolder CreateFolder(string path);

        DialogResult ShowFolderDialog(out IFolderBrowser browser);

        DialogResult ShowFileDialog(out IFileBrowser browser);

        Task CopyFolderToRootAsync(string rootPath, string path);
        void CopyFolderToRoot(string rootPath, string path);

        Task CopyFilesToFolderAsync(TFolder folder, params string[] fileNames);
        void CopyFilesToFolder(TFolder folder, params string[] fileNames);

        void RemoveFileFromFolder(TFolder folder, TFile file);
    }
}
