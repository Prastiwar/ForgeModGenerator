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
    public class FoldersSynchronizer<TFolder, TFile>
        where TFolder : class, IFileFolder<TFile>
        where TFile : class, IFileItem
    {
        public FoldersSynchronizer(Collection<TFolder> folders, string rootPath = null, string filters = null)
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
            FileWatcher.Created += FileWatcher_Created;
            FileWatcher.Deleted += FileWatcher_Deleted;
            FileWatcher.Renamed += FileWatcher_Renamed;
        }

        public Collection<TFolder> Folders { get; set; }

        protected event EventHandler<TFolder> folderInstantiatedHandler;
        public event EventHandler<TFolder> FolderInstantiated {
            add => folderInstantiatedHandler += value;
            remove => folderInstantiatedHandler -= value;
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
                FileWatcher.Filter = RootPath;
            }
        }

        protected FileSystemWatcherExtended FileWatcher { get; set; }

        public bool TryGetFolder(string path, out TFolder folder)
        {
            string folderPath = IOHelper.GetDirectoryPath(path);
            folder = Folders?.Find(x => x.Info.FullName == folderPath);
            return folder != null;
        }

        public bool TryGetFile(string path, out TFile file, out TFolder folder)
        {
            if (TryGetFolder(path, out folder))
            {
                file = folder.Files.Find(x => x.Info.FullName == path);
                return file != null;
            }
            file = null;
            return false;
        }

        public virtual async Task<ObservableCollection<TFolder>> FindFolders(string path, bool createRootIfEmpty = false)
        {
            if (!IOHelper.IsPathValid(path))
            {
                throw new InvalidOperationException($"Path is not valid {path}");
            }
            ObservableCollection<TFolder> found = null;
            if (Directory.Exists(path))
            {
                found = await FindFoldersFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                found = await FindFoldersFromFile(path);
            }

            bool isEmpty = found == null || found.Count <= 0;
            if (isEmpty && createRootIfEmpty)
            {
                found = CreateEmptyFoldersRoot(IOHelper.GetDirectoryPath(path));
            }
            return found;
        }

        /// <summary> Creates TFolder with found TFile's for each subdirectory </summary>
        public async Task<ObservableCollection<TFolder>> FindFoldersFromDirectory(string path) => await Task.Run(() => {
            ObservableCollection<TFolder> foundFolders = new ObservableCollection<TFolder>();

            AddFilesToCollection(path);
            foreach (string directory in IOHelper.EnumerateAllDirectories(path))
            {
                AddFilesToCollection(directory);
            }

            void AddFilesToCollection(string directoryPath)
            {
                IEnumerable<string> filePaths = EnumerateFilteredFiles(directoryPath);
                if (filePaths.Any())
                {
                    TFolder folder = ConstructFolderInstance(directoryPath, filePaths);
                    foundFolders.Add(folder);
                }
            }
            return foundFolders;
        });

        /// <summary> Returns folders by deserializing them from file in given path </summary>
        public async Task<ObservableCollection<TFolder>> FindFoldersFromFile(string path, bool loadOnlyExisting = true) => await Task.Run(() => {
            string content = File.ReadAllText(path);
            ObservableCollection<TFolder> deserializedFolders = null;
            try
            {
                deserializedFolders = DeserializeFolders(content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldnt load {typeof(ObservableCollection<TFolder>)} from {content}", true);
                return null;
            }
            if (folderInstantiatedHandler != null)
            {
                foreach (TFolder folder in deserializedFolders)
                {
                    folderInstantiatedHandler.Invoke(this, folder);
                }
            }
            if (loadOnlyExisting)
            {
                deserializedFolders = FilterToOnlyExistingFiles(deserializedFolders);
            }
            return deserializedFolders;
        });

        /// <summary> Returns file that use extension from AllowedFileExtensions </summary>
        public IEnumerable<string> EnumerateFilteredFiles(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => IOHelper.EnumerateFiles(directoryPath, Filters, searchOption);

        public ObservableCollection<TFolder> FilterToOnlyNotExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => !File.Exists(file.Info.FullName));
        public ObservableCollection<TFolder> FilterToOnlyExistingFiles(IEnumerable<TFolder> foldersToFilter) => FilterFolderFiles(foldersToFilter, file => File.Exists(file.Info.FullName));

        /// <summary> Returns folders which files matches fileFilter </summary>
        public ObservableCollection<TFolder> FilterFolderFiles(IEnumerable<TFolder> foldersToFilter, Func<TFile, bool> fileFilter)
        {
            ObservableCollection<TFolder> folders = new ObservableCollection<TFolder>();
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

        protected ObservableCollection<TFolder> CreateEmptyFoldersRoot(string folderPath) => new ObservableCollection<TFolder>() { ConstructFolderInstance(IOHelper.GetDirectoryPath(folderPath), null) };

        /// <summary> Create TFolder instance and subscrive its events </summary>
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
            folderInstantiatedHandler?.Invoke(this, folder);
            return folder;
        }

        /// <summary> Deserializes Json content to ObservableCollection<TFolder> </summary>
        protected virtual ObservableCollection<TFolder> DeserializeFolders(string fileCotent) => JsonConvert.DeserializeObject<ObservableCollection<TFolder>>(fileCotent);

        /// <summary> Called when FileSystemWatcher detects folder creation </summary>
        protected async void SyncCreateFolder(string path, bool includeSubDirectories = true)
        {
            SynchronizationCheck(path);
            if (Folders != null)
            {
                Folders?.Add(ConstructFolderInstance(path, null));
                if (includeSubDirectories)
                {
                    ObservableCollection<TFolder> foundFolders = await FindFoldersFromDirectory(path);
                    if (foundFolders.Count > 0)
                    {
                        foreach (TFolder folder in foundFolders)
                        {
                            Folders.Add(folder);
                        }
                    }
                }
            }
        }

        /// <summary> Called when FileSystemWatcher detects file creation </summary>
        protected void SyncCreateFile(string path)
        {
            SynchronizationCheck(path);
            string folderPath = IOHelper.GetDirectoryPath(path);
            if (TryGetFolder(folderPath, out TFolder folder))
            {
                folder.Add(path);
            }
        }

        /// <summary> Called when FileSystemWatcher detects folder deletion </summary>
        protected void SyncRemoveFolder(string path)
        {
            SynchronizationCheck(path);
            if (TryGetFolder(path, out TFolder folder))
            {
                if (Folders.Remove(folder))
                {
                    folder.Clear();
                }
            }
        }

        /// <summary> Called when FileSystemWatcher detects file deletion </summary>
        protected void SyncRemoveFile(string path)
        {
            SynchronizationCheck(path);
            if (TryGetFile(path, out TFile file, out TFolder folder))
            {
                folder.Remove(file);
            }
        }

        /// <summary> Called when FileSystemWatcher detects folder rename </summary>
        protected void SyncRenameFolder(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            string oldFolderPath = IOHelper.GetDirectoryPath(oldPath);
            string folderPath = IOHelper.GetDirectoryPath(newPath);
            if (TryGetFolder(oldFolderPath, out TFolder oldFolder))
            {
                oldFolder.SetInfo(folderPath);
            }
        }

        /// <summary> Called when FileSystemWatcher detects file rename </summary>
        protected void SyncRenameFile(string oldPath, string newPath)
        {
            SynchronizationCheck(oldPath);
            if (TryGetFile(oldPath, out TFile file, out TFolder folder))
            {
                file.SetInfo(newPath);
            }
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

        private void SynchronizationCheck(string actualPath)
        {
            if (!IOHelper.IsSubPathOf(actualPath, RootPath))
            {
                throw new InvalidSynchronizationArgument(RootPath, actualPath);
            }
        }
    }
}
