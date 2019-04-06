using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public interface IFoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFolderObject<TFolder> CreateFolders();
        TFolder Create();
        TFolder Create(string path, IEnumerable<string> filePaths);

        string Filters { get; set; }

        ICollection<TFolder> FilterFolderFiles(IEnumerable<TFolder> foldersToFilter, Func<TFile, bool> fileFilter);
        IEnumerable<string> EnumerateFilteredFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly);
        IEnumerable<string> EnumerateNotReferencedFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly);
        ICollection<TFolder> FilterToOnlyNotExistingFiles(IEnumerable<TFolder> foldersToFilter);
        ICollection<TFolder> FilterToOnlyExistingFiles(IEnumerable<TFolder> foldersToFilter);
        ICollection<TFolder> FindFoldersFromFile(string path, bool loadOnlyExisting = true);
        Task<ICollection<TFolder>> FindFoldersFromFileAsync(string path, bool loadOnlyExisting = true);
        Task<IEnumerable<TFolder>> FindFoldersAsync(string path, bool createRootIfEmpty = false);
        IEnumerable<TFolder> FindFoldersFromDirectory(string path);
        IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false);
    }
}
