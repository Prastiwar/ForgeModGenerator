using ForgeModGenerator.Exceptions;
using ForgeModGenerator.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeModGenerator
{
    public class DefaultFoldersSynchronizer<TFolder, TFile> : FoldersSynchronizer<TFolder, TFile>
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public DefaultFoldersSynchronizer(ObservableFolder<TFolder> folders, string rootPath = null, string filters = null) : base(folders, rootPath, filters) { }

        public override IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!IOHelper.IsPathValid(path))
            {
                throw new InvalidOperationException($"Path is not valid {path}");
            }
            IEnumerable<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = EnumerateFoldersFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = GetFoldersFromFile(path);
            }

            bool isEmpty = found == null || !found.Any();
            if (isEmpty && createRootIfEmpty)
            {
                found = CreateEmptyFoldersRoot(IOHelper.GetDirectoryPath(path));
            }
            return found.ToList();
        }
    }

    public abstract class FoldersSynchronizer<TFolder, TFile>
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public FoldersSynchronizer(ObservableFolder<TFolder> folders, string rootPath = null, string filters = null)
        {
            this.rootPath = rootPath;
            this.filters = filters;
            Folders = folders;

            FileWatcher = new FileSystemWatcherExtended(RootPath, Filters) {
                IncludeSubdirectories = true,
                MonitorDirectoryChanges = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName
            };
            FileWatcher.FileCreated += FileWatcher_Created;
            FileWatcher.FileDeleted += FileWatcher_Deleted;
            FileWatcher.FileRenamed += FileWatcher_Renamed;
            FileWatcher.FileSubPathRenamed += FileWatcher_SubPathRenamed;
        }

        private ObservableFolder<TFolder> folders;
        public ObservableFolder<TFolder> Folders {
            get => folders;
            set => folders = value ?? throw new ArgumentNullException(nameof(value));
        }

        private string rootPath;
        public string RootPath {
            get => rootPath;
            set {
                rootPath = value;
                FileWatcher.Path = RootPath;
            }
        }

        private string filters;

        public string Filters {
            get => filters;
            set {
                filters = value;
                FileWatcher.Filter = Filters;
            }
        }

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        public abstract IEnumerable<TFolder> FindFolders(string path, bool createRootIfEmpty = false);

        public Task<IEnumerable<TFolder>> FindFoldersAsync(string path, bool createRootIfEmpty = false) => Task.Run(() => { return FindFolders(path, createRootIfEmpty); });

        public bool TryGetFolder(string path, out TFolder folder)
        {
            string folderPath = IOHelper.GetDirectoryPath(path);
            folder = Folders.Files.Find(x => x.Info.FullName.ComparePath(folderPath));
            return folder != null;
        }

        public bool TryGetFile(string path, out TFile file) => TryGetFile(path, out file, out TFolder folder);
        public bool TryGetFile(string path, out TFile file, out TFolder folder)
        {
            foreach (TFolder folderItem in Folders.Files)
            {
                foreach (TFile folderFile in folderItem.Files)
                {
                    if (folderFile.Info.FullName.ComparePath(path))
                    {
                        file = folderFile;
                        folder = folderItem;
                        return true;
                    }
                }
            }
            folder = null;
            file = null;
            return false;
        }

        /// <summary> Creates TFolder with found TFile's for each subdirectory </summary>
        public IEnumerable<TFolder> EnumerateFoldersFromDirectory(string path)
        {
            IEnumerable<string> filePaths = EnumerateFilteredFiles(path);
            if (filePaths.Any())
            {
                TFolder folder = ConstructFolderInstance(path, filePaths);
                yield return folder;
            }
            foreach (string directory in IOHelper.EnumerateAllDirectories(path))
            {
                filePaths = EnumerateFilteredFiles(directory);
                if (filePaths.Any())
                {
                    TFolder folder = ConstructFolderInstance(directory, filePaths);
                    yield return folder;
                }
            }
        }

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        public ICollection<TFolder> GetFoldersFromFile(string path, bool loadOnlyExisting = true)
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

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        public async Task<ICollection<TFolder>> GetFoldersFromFileAsync(string path, bool loadOnlyExisting = true)
        {
            if (!File.Exists(path))
            {
                return new Collection<TFolder>();
            }
            string content = await IOHelper.ReadAllTextAsync(path);
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

        /// <summary> Searches root path for files that are not referenced in Folders collection and adds them </summary>
        public void AddNotReferencedFiles()
        {
            IEnumerable<string> notReferencedFiles = EnumerateNotReferencedFiles(RootPath, SearchOption.AllDirectories);
            foreach (string filePath in notReferencedFiles)
            {
                string dirPath = IOHelper.GetDirectoryPath(filePath);
                if (!TryGetFolder(dirPath, out TFolder folder))
                {
                    folder = ConstructFolderInstance(dirPath, null);
                    Folders.Add(folder);
                }
                List<string> notReferencedFilesForFolder = notReferencedFiles.Where(path => IOHelper.GetDirectoryPath(path) == folder.Info.FullName).ToList();
                folder.AddRange(notReferencedFilesForFolder);
                notReferencedFiles = notReferencedFiles.Except(notReferencedFilesForFolder);
            }
        }

        protected ICollection<TFolder> CreateEmptyFoldersRoot(string folderPath) => new Collection<TFolder>() { ConstructFolderInstance(IOHelper.GetDirectoryPath(folderPath), null) };

        /// <summary> Create TFolder instance and subscribe its events </summary>
        protected virtual TFolder ConstructFolderInstance(string path, IEnumerable<string> filePaths)
        {
            TFolder folder = null;
            try
            {
                folder = ReflectionExtensions.CreateInstance<TFolder>(path, filePaths);
            }
            catch (Exception)
            {
                folder = ReflectionExtensions.CreateInstance<TFolder>(path);
                if (filePaths != null)
                {
                    foreach (string filePath in filePaths)
                    {
                        folder.Add(filePath);
                    }
                }
            }
            return folder;
        }

        /// <summary> Deserializes Json content to ICollection<TFolder> </summary>
        protected virtual ICollection<TFolder> DeserializeFolders(string fileCotent) => JsonConvert.DeserializeObject<Collection<TFolder>>(fileCotent);

        protected bool RemoveFolder(TFolder folder)
        {
            if (Folders.Remove(folder))
            {
                folder.Clear();
                return true;
            }
            return false;
        }

        protected bool Rename(IFileSystemInfo info, string newPath)
        {
            if (!info.Info.FullName.ComparePath(newPath))
            {
                info.SetInfo(newPath);
                return true;
            }
            return false;
        }

        /// <summary> Called when FileSystemWatcher detects folder creation </summary>
        protected bool SyncCreateFolder(string path, bool includeSubDirectories = true)
        {
            SynchronizationCheck(path);
            Folders.Add(ConstructFolderInstance(path, null));
            if (includeSubDirectories)
            {
                IEnumerable<TFolder> foundFolders = EnumerateFoldersFromDirectory(path);
                if (foundFolders.Any())
                {
                    Folders.AddRange(foundFolders);
                }
            }
            return true;
        }

        /// <summary> Called when FileSystemWatcher detects file creation. Finds folder from path and adds path to it </summary>
        protected bool SyncCreateFile(string path)
        {
            SynchronizationCheck(path);
            string folderPath = IOHelper.GetDirectoryPath(path);
            return TryGetFolder(folderPath, out TFolder folder) ? folder.Add(path) : false;
        }

        /// <summary> Called when FileSystemWatcher detects folder deletion. Finds folder from path, if removes from Folders - clear it </summary>
        protected bool SyncRemoveFolder(string path)
        {
            SynchronizationCheck(path);
            return TryGetFolder(path, out TFolder folder) ? RemoveFolder(folder) : false;
        }

        /// <summary> Called when FileSystemWatcher detects file deletion. Finds File from path and removes it from folder it belongs to </summary>
        protected bool SyncRemoveFile(string path)
        {
            SynchronizationCheck(path);
            return TryGetFile(path, out TFile file, out TFolder folder) ? folder.Remove(file) : false;
        }

        /// <summary> Called when FileSystemWatcher detects folder rename </summary>
        protected bool SyncRenameFolder(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            string oldFolderPath = IOHelper.GetDirectoryPath(oldPath);
            newPath = IOHelper.GetDirectoryPath(newPath).NormalizeFullPath();
            return TryGetFolder(oldFolderPath, out TFolder oldFolder) ? Rename(oldFolder, newPath) : false;
        }

        /// <summary> Called when FileSystemWatcher detects file rename </summary>
        protected bool SyncRenameFile(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            return TryGetFile(oldPath, out TFile file) ? Rename(file, newPath.NormalizeFullPath()) : false;
        }

        protected virtual void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { SyncCreateFolder(e.FullPath, true); });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { SyncCreateFile(e.FullPath); });
            }
        }

        protected virtual void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRemoveFolder(e.FullPath); });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRemoveFile(e.FullPath); });
            }
        }

        protected virtual void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (IOHelper.IsDirectoryPath(e.FullPath))
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRenameFolder(e.OldFullPath, e.FullPath); });
            }
            else // is file
            {
                Application.Current.Dispatcher.Invoke(() => { SyncRenameFile(e.OldFullPath, e.FullPath); });
            }
        }

        protected virtual void FileWatcher_SubPathRenamed(object sender, FileSubPathEventArgs e) => Application.Current.Dispatcher.Invoke(() => { SyncRenameFile(e.OldFullPath, e.FullPath); });

        /// <summary> Throws exception if given path is not sub path of RootPath </summary>
        protected void SynchronizationCheck(string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, RootPath))
            {
                throw new InvalidSynchronizationArgument(RootPath, actualPath);
            }
        }
    }
}
