using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public abstract class FoldersFinder<TFolder, TFile> : IFoldersFinder<TFolder, TFile>
        where TFolder : class, IFolderObject<TFile>
        where TFile : class, IFileObject
    {
        public FoldersFinder(IFoldersFactory<TFolder, TFile> factory) => Factory = factory;

        public IFoldersFactory<TFolder, TFile> Factory { get; }

        /// <summary> Multiple filters separated with "|" e.g ".txt|*.png applied to when searching for files </summary>
        public string Filters { get; set; } = "*";

        public abstract IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false);
        public Task<IEnumerable<TFolder>> FindFoldersAsync(string path, bool createRootIfEmpty = false) => Task.Run(() => FindFolders(path, createRootIfEmpty));

        public Task<IEnumerable<TFolder>> FindFoldersFromDirectoryAsync(string path) => Task.Run(() => FindFoldersFromDirectory(path));

        /// <summary> Creates TFolder with found TFile's for each subdirectory </summary>
        public IEnumerable<TFolder> FindFoldersFromDirectory(string path)
        {
            IEnumerable<string> filePaths = EnumerateFilteredFiles(path);
            if (filePaths.Any())
            {
                yield return Factory.Create(path, filePaths);
            }
            foreach (string directory in IOHelper.EnumerateAllDirectories(path))
            {
                filePaths = EnumerateFilteredFiles(directory);
                if (filePaths.Any())
                {
                    yield return Factory.Create(directory, filePaths);
                }
            }
        }

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        public ICollection<TFolder> FindFoldersFromFile(string path, bool loadOnlyExisting = true)
        {
            if (!File.Exists(path))
            {
                return new Collection<TFolder>();
            }
            string content = File.ReadAllText(path);
            List<TFolder> deserializedFolders = null;
            try
            {
                deserializedFolders = new List<TFolder>(DeserializeFolders(content));
            }
            catch (Exception)
            {
                return new Collection<TFolder>();
            }
            if (loadOnlyExisting)
            {
                deserializedFolders = new List<TFolder>(FilterToOnlyExistingFiles(deserializedFolders));
            }
            return deserializedFolders;
        }

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        public async Task<ICollection<TFolder>> FindFoldersFromFileAsync(string path, bool loadOnlyExisting = true)
        {
            if (!File.Exists(path))
            {
                return new Collection<TFolder>();
            }
            string content = await IOHelper.ReadAllTextAsync(path).ConfigureAwait(false);
            List<TFolder> deserializedFolders = null;
            try
            {
                deserializedFolders = new List<TFolder>(DeserializeFolders(content));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt load {path}", true, $"File content: {content}");
                return new Collection<TFolder>();
            }
            if (loadOnlyExisting)
            {
                deserializedFolders = new List<TFolder>(FilterToOnlyExistingFiles(deserializedFolders));
            }
            return deserializedFolders;
        }

        /// <summary> Returns file that use extension from AllowedFileExtensions </summary>
        public IEnumerable<string> EnumerateFilteredFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => IOHelper.EnumerateFiles(directoryPath, Filters, searchOption);

        public IEnumerable<string> EnumerateNotReferencedFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => EnumerateFilteredFiles(path, searchOption).Where(filePath => !FileSystemInfoReference.IsReferenced(filePath));

        public ICollection<TFolder> FilterToOnlyNotExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => !File.Exists(file.Info.FullName));
        public ICollection<TFolder> FilterToOnlyExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => File.Exists(file.Info.FullName));

        /// <summary> Returns folders which files matches fileFilter </summary>
        public ICollection<TFolder> FilterFolderFiles(IEnumerable<TFolder> foldersToFilter, Func<TFile, bool> fileFilter)
        {
            ICollection<TFolder> folders = new Collection<TFolder>();
            foreach (TFolder folder in foldersToFilter.Where(dir => Directory.Exists(dir.Info.FullName)))
            {
                TFolder copyFolder = (TFolder)folder.DeepClone();
                for (int i = copyFolder.Files.Count - 1; i >= 0; i--)
                {
                    TFile file = copyFolder.Files[i];
                    if (!fileFilter(file))
                    {
                        copyFolder.Remove(file);
                    }
                }
                if (copyFolder.Files.Count > 0)
                {
                    folders.Add(copyFolder);
                }
            }
            return folders;
        }

        /// <summary> Deserializes Json content to ICollection<TFolder> </summary>
        protected abstract ICollection<TFolder> DeserializeFolders(string fileCotent);

        protected ICollection<TFolder> CreateEmptyFoldersRoot(string folderPath) => new Collection<TFolder>() { Factory.Create(IOHelper.GetDirectoryPath(folderPath), null) };
    }
}
