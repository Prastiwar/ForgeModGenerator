using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace ForgeModGenerator
{
    public class FileSystemWin : IFileSystem
    {
        public bool MoveDirectoriesAndFiles(string from, string destination)
        {
            bool allSucceded = true;
            foreach (string folder in Directory.EnumerateDirectories(from))
            {
                MoveDirectory(folder, Path.Combine(destination, new DirectoryInfo(folder).Name));
            }
            foreach (string file in Directory.EnumerateFiles(from))
            {
                MoveFile(file, Path.Combine(destination, new FileInfo(file).Name));
            }
            return allSucceded;
        }

        public bool MoveDirectoriesAndFiles(string from, string destination, bool overwrite)
        {
            bool allSucceded = true;
            foreach (string folder in Directory.EnumerateDirectories(from))
            {
                MoveDirectory(folder, Path.Combine(destination, new DirectoryInfo(folder).Name), overwrite);
            }
            foreach (string file in Directory.EnumerateFiles(from))
            {
                MoveFile(file, Path.Combine(destination, new FileInfo(file).Name), overwrite);
            }
            return allSucceded;
        }

        public bool MoveDirectory(string from, string destination)
        {
            try
            {
                FileSystem.MoveDirectory(from, destination, UIOption.OnlyErrorDialogs, UICancelOption.ThrowException);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool MoveDirectory(string from, string destination, bool overwrite)
        {
            try
            {
                FileSystem.MoveDirectory(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CopyDirectory(string from, string destination)
        {
            try
            {
                FileSystem.CopyDirectory(from, destination, UIOption.OnlyErrorDialogs, UICancelOption.ThrowException);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CopyDirectory(string from, string destination, bool overwrite)
        {
            try
            {
                FileSystem.CopyDirectory(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool DeleteDirectory(string directoryPath, bool recycle)
        {
            try
            {
                if (recycle)
                {
                    FileSystem.DeleteDirectory(directoryPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                }
                else
                {
                    FileSystem.DeleteDirectory(directoryPath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool RenameDirectory(string name, string newName)
        {
            try
            {
                FileSystem.RenameDirectory(name, newName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool MoveFile(string from, string destination)
        {
            try
            {
                FileSystem.MoveFile(from, destination, UIOption.OnlyErrorDialogs, UICancelOption.ThrowException);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool MoveFile(string from, string destination, bool overwrite)
        {
            try
            {
                FileSystem.MoveFile(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CopyFile(string from, string destination)
        {
            try
            {
                FileSystem.CopyFile(from, destination, UIOption.OnlyErrorDialogs, UICancelOption.ThrowException);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CopyFile(string from, string destination, bool overwrite)
        {
            try
            {
                FileSystem.CopyFile(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool DeleteFile(string filePath, bool recycle)
        {
            try
            {
                if (recycle)
                {
                    FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
                }
                else
                {
                    FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool RenameFile(string name, string newName)
        {
            try
            {
                FileSystem.RenameFile(name, newName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CreateDirectory(string path)
        {
            try
            {
                FileSystem.CreateDirectory(path);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public IFileBrowser CreateFileBrowser() => new FileBrowserDialog();

        public IFolderBrowser CreateFolderBrowser() => new FolderBrowserDialog();
    }
}
