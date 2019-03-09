using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace ForgeModGenerator.Utility
{
    public static class IOSafe
    {
        public const string UnauthorizedAccessMessage = "You do not have privilaged to perform this action.";

        public static string GetOperationFailedMessage(string name) => $"Operation failed. Check if {name} is not used by any process and you have privilages to do this.";

        public static bool MoveDirectoriesAndFiles(string from, string destination, bool overwrite)
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

        public static bool MoveDirectory(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.MoveDirectory(from, destination, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool MoveDirectory(string from, string destination, bool overwrite)
        {
            try
            {
                IOHelper.MoveDirectory(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyDirectory(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.CopyDirectory(from, destination, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool CopyDirectory(string from, string destination, bool overwrite)
        {
            try
            {
                IOHelper.CopyDirectory(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DeleteDirectoryPerm(string directoryPath, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.DeleteDirectoryPerm(directoryPath, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool DeleteDirectoryRecycle(string directoryPath, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.DeleteDirectoryRecycle(directoryPath, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool RenameDirectory(string name, string newName)
        {
            try
            {
                IOHelper.RenameDirectory(name, newName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool MoveFile(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.MoveFile(from, destination, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool MoveFile(string from, string destination, bool overwrite)
        {
            try
            {
                IOHelper.MoveFile(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyFile(string from, string destination, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.CopyFile(from, destination, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool CopyFile(string from, string destination, bool overwrite)
        {
            try
            {
                IOHelper.CopyFile(from, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFilePerm(string filePath, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.DeleteFilePerm(filePath, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool DeleteFileRecycle(string filePath, UIOption option = UIOption.OnlyErrorDialogs)
        {
            try
            {
                IOHelper.DeleteFileRecycle(filePath, option);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool RenameFile(string name, string newName)
        {
            try
            {
                IOHelper.RenameFile(name, newName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CreateDirectory(string path)
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
    }
}
