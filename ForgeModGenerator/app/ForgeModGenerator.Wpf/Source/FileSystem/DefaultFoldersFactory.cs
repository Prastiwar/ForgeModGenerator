using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator
{
    public class DefaultFoldersFactory<TFolder, TFile> : FoldersFactory<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public DefaultFoldersFactory(string filters) : base(filters) { }

        public override IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!IOHelper.IsPathValid(path))
            {
                throw new InvalidOperationException($"Path is not valid {path}");
            }
            IEnumerable<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = FindFoldersFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = FindFoldersFromFile(path);
            }

            bool isEmpty = found == null || !found.Any();
            if (isEmpty && createRootIfEmpty)
            {
                found = CreateEmptyFoldersRoot(IOHelper.GetDirectoryPath(path));
            }
            return found.ToList();
        }

        protected override ICollection<TFolder> DeserializeFolders(string fileCotent) => JsonConvert.DeserializeObject<Collection<TFolder>>(fileCotent);
    }
}
