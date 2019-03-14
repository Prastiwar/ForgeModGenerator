using System.Collections.Generic;
using System.Linq;

namespace ForgeModGenerator.Utility
{
    public static class ObservableFolderExtensions
    {
        public static bool TryGetFolderFile<TFolder, TFile>(this ObservableFolder<TFolder> folderCollection, string path, out TFile file)
            where TFolder : class, IFileFolder<TFile>
            where TFile : class, IFileItem => TryGetFolderFile(folderCollection, path, out file, out TFolder folder);

        public static bool TryGetFolderFile<TFolder, TFile>(this ObservableFolder<TFolder> folderCollection, string path, out TFile file, out TFolder folder)
            where TFolder : class, IFileFolder<TFile>
            where TFile : class, IFileItem
        {
            foreach (TFolder folderItem in folderCollection.Files)
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

        /// <summary> Enumerates files that matches given file path </summary>
        public static IEnumerable<TFile> EnumerateFiles<TFile>(this ObservableFolder<TFile> folders, string path) where TFile : IFileSystemInfo =>
            folders.Files.Where(file => file.Info.FullName.ComparePath(path));

        /// <summary> Enumerates files that matches given file path </summary>
        public static IEnumerable<TFile> EnumerateFolderFiles<TFolder, TFile>(this ObservableFolder<TFolder> folderCollection, string path)
            where TFolder : class, IFileFolder<TFile>
            where TFile : class, IFileItem
        {
            foreach (TFolder soundEvent in folderCollection.Files)
            {
                TFile sound = soundEvent.Files.Find(x => x.Info.FullName.ComparePath(path));
                if (sound != null)
                {
                    yield return sound;
                }
            }
        }

        /// <summary> Enumerates files that matches given file path </summary>
        public static IEnumerable<(TFolder folder, TFile file)> EnumerateFolderFilesExtended<TFolder, TFile>(this ObservableFolder<TFolder> folderCollection, string path)
            where TFolder : class, IFileFolder<TFile>
            where TFile : class, IFileItem
        {
            foreach (TFolder soundEvent in folderCollection.Files)
            {
                TFile sound = soundEvent.Files.Find(x => x.Info.FullName.ComparePath(path));
                if (sound != null)
                {
                    yield return (folder: soundEvent, file: sound);
                }
            }
        }
    }
}
