using ForgeModGenerator.Serialization;
using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ForgeModGenerator
{
    public class DefaultFoldersFinder<TFolder, TFile> : FoldersFinder<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public DefaultFoldersFinder(ISerializer serializer, IFoldersFactory<TFolder, TFile> factory) : base(factory) => Serializer = serializer;

        protected ISerializer Serializer { get; }

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
            List<TFolder> foundList = found?.ToList();

            bool isEmpty = foundList == null || !foundList.Any();
            if (isEmpty && createRootIfEmpty)
            {
                foundList = CreateEmptyFoldersRoot(IOHelper.GetDirectoryPath(path)).ToList();
            }
            return foundList;
        }

        protected override ICollection<TFolder> DeserializeFolders(string fileCotent) => Serializer.DeserializeObject<Collection<TFolder>>(fileCotent);
    }
}
