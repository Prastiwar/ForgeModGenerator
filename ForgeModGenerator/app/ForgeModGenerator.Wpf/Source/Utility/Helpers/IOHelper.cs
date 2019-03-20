using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace ForgeModGenerator.Utility
{
    public static class IOHelperWin
    {
        public static void MoveDirectory(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.MoveDirectory(from, destination, option, UICancelOption.ThrowException);
        public static void MoveDirectory(string from, string destination, bool overwrite) => FileSystem.MoveDirectory(from, destination, overwrite);

        public static void CopyDirectory(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.CopyDirectory(from, destination, option, UICancelOption.ThrowException);
        public static void CopyDirectory(string from, string destination, bool overwrite) => FileSystem.CopyDirectory(from, destination, overwrite);

        public static void DeleteDirectoryPerm(string directoryPath, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.DeleteDirectory(directoryPath, option, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
        public static void DeleteDirectoryRecycle(string directoryPath, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.DeleteDirectory(directoryPath, option, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);

        public static void RenameDirectory(string name, string newName) => FileSystem.RenameDirectory(name, newName);

        public static void MoveFile(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.MoveFile(from, destination, option, UICancelOption.ThrowException);
        public static void MoveFile(string from, string destination, bool overwrite) => FileSystem.MoveFile(from, destination, overwrite);

        public static void CopyFile(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.CopyFile(from, destination, option, UICancelOption.ThrowException);
        public static void CopyFile(string from, string destination, bool overwrite) => FileSystem.CopyFile(from, destination, overwrite);

        public static void DeleteFilePerm(string filePath, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.DeleteFile(filePath, option, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
        public static void DeleteFileRecycle(string filePath, UIOption option = UIOption.OnlyErrorDialogs) =>
            FileSystem.DeleteFile(filePath, option, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);

        public static void RenameFile(string name, string newName) => FileSystem.RenameFile(name, newName);

        public static void MoveDirectoriesAndFiles(string from, string destination)
        {
            foreach (string folder in Directory.EnumerateDirectories(from))
            {
                MoveDirectory(folder, Path.Combine(destination, new DirectoryInfo(folder).Name));
            }
            foreach (string file in Directory.EnumerateFiles(from))
            {
                MoveFile(file, Path.Combine(destination, new FileInfo(file).Name));
            }
        }

        public static void MoveDirectoriesAndFiles(string from, string destination, bool overwrite)
        {
            foreach (string folder in Directory.EnumerateDirectories(from))
            {
                MoveDirectory(folder, Path.Combine(destination, new DirectoryInfo(folder).Name), overwrite);
            }
            foreach (string file in Directory.EnumerateFiles(from))
            {
                MoveFile(file, Path.Combine(destination, new FileInfo(file).Name), overwrite);
            }
        }
    }
}
