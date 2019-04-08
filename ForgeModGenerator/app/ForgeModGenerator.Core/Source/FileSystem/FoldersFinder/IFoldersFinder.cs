using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public interface IFoldersFinder<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFoldersFactory<TFolder, TFile> Factory { get; }

        string Filters { get; set; }

        IEnumerable<string> EnumerateFilteredFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly);
        IEnumerable<string> EnumerateNotReferencedFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly);

        ICollection<TFolder> FilterFolderFiles(IEnumerable<TFolder> foldersToFilter, Func<TFile, bool> fileFilter);

        ICollection<TFolder> FindFoldersFromFile(string path, bool loadOnlyExisting = true);
        Task<ICollection<TFolder>> FindFoldersFromFileAsync(string path, bool loadOnlyExisting = true);

        IEnumerable<TFolder> FindFoldersFromDirectory(string path);
        Task<IEnumerable<TFolder>> FindFoldersFromDirectoryAsync(string path);

        Task<IEnumerable<TFolder>> FindFoldersAsync(string path, bool createRootIfEmpty = false);
        IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false);
    }
}
