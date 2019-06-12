using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;

namespace ForgeModGenerator
{
    public class FoldersFactory<TFolder, TFile> : IFoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public virtual IFolderObject<TFolder> CreateFolders() => ObservableFolder<TFolder>.CreateEmpty();
        public TFolder Create() => Create(null, null);
        TFolder IFoldersFactory<TFolder, TFile>.Create(string path, IEnumerable<string> filePaths) => Create(path, filePaths);

        public virtual TFolder Create(string path, IEnumerable<string> filePaths)
        {
            TFolder folder = null;
            if (path != null)
            {
                try
                {
                    folder = ReflectionHelper.CreateInstance<TFolder>(path, filePaths);
                }
                catch (Exception)
                {
                    folder = ReflectionHelper.CreateInstance<TFolder>(path);
                    if (filePaths != null)
                    {
                        folder.AddRange(filePaths);
                    }
                }
            }
            else
            {
                folder = ReflectionHelper.CreateInstance<TFolder>(true);
                if (filePaths != null)
                {
                    folder.AddRange(filePaths);
                }
            }
            return folder;
        }
    }
}
