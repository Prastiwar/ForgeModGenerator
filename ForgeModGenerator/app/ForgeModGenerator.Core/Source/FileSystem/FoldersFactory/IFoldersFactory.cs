using System.Collections.Generic;

namespace ForgeModGenerator
{
    public interface IFoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        IFolderObject<TFolder> CreateFolders();
        TFolder Create();
        TFolder Create(string path, IEnumerable<string> filePaths);
    }
}
