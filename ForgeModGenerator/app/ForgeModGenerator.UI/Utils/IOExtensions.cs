using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using SearchOption = System.IO.SearchOption;

namespace ForgeModGenerator.Utils
{
    public static class IOExtensions
    {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULLCHAR = (char)0;

        public static IEnumerable<string> EnumerateAllFiles(string path) => Directory.EnumerateFiles(GetDirectoryPath(path), "*", SearchOption.AllDirectories);
        public static IEnumerable<string> EnumerateAllDirectories(string path) => Directory.EnumerateDirectories(GetDirectoryPath(path), "*", SearchOption.AllDirectories);
        public static IEnumerable<FileInfo> EnumerateAllFileInfos(string path) => new DirectoryInfo(GetDirectoryPath(path)).EnumerateFiles("*", SearchOption.AllDirectories);
        public static IEnumerable<DirectoryInfo> EnumerateAllDirectoryInfos(string path) => new DirectoryInfo(GetDirectoryPath(path)).EnumerateDirectories("*", SearchOption.AllDirectories);

        /// <summary> Increments Name(i) till isUnique is true </summary>
        public static string GetUniqueName(string name, Func<string, bool> isUnique)
        {
            string uniqueName = name;
            int i = 1;
            while (!isUnique(uniqueName))
            {
                uniqueName = $"{name}({i})";
                i++;
            }
            return uniqueName;
        }

        /// <summary> Directory.EnumerateFiles with multiple patterns splitted by "|" (e.g *.txt|*.log) </summary>
        public static IEnumerable<string> EnumerateFiles(string path, string patterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (string pattern in patterns.Split('|'))
            {
                foreach (string file in Directory.EnumerateFiles(path, pattern, searchOption))
                {
                    yield return file;
                }
            }
        }

        /// <summary> DirectoryInfo.EnumerateFiles with multiple patterns splitted by "|" (e.g *.txt|*.log) </summary>
        public static IEnumerable<FileInfo> EnumerateFileInfos(string path, string patterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (string pattern in patterns.Split('|'))
            {
                foreach (FileInfo file in new DirectoryInfo(path).EnumerateFiles(pattern, searchOption))
                {
                    yield return file;
                }
            }
        }

        /// <summary> Directory.EnumerateDirectories with multiple patterns splitted by "|" (e.g a*|b*) </summary>
        public static IEnumerable<string> EnumerateDirectories(string path, string patterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (string pattern in patterns.Split('|'))
            {
                foreach (string file in Directory.EnumerateDirectories(path, pattern, searchOption))
                {
                    yield return file;
                }
            }
        }

        /// <summary> DirectoryInfo.EnumerateDirectories with multiple patterns splitted by "|" (e.g a*|b*) </summary>
        public static IEnumerable<DirectoryInfo> EnumerateDirectoryInfos(string path, string patterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (string pattern in patterns.Split('|'))
            {
                foreach (DirectoryInfo file in new DirectoryInfo(path).EnumerateDirectories(pattern, searchOption))
                {
                    yield return file;
                }
            }
        }

        /// <summary> if path is file, return it's directory, else return path </summary>
        public static string GetDirectoryPath(string path) => IsFilePath(path) ? new FileInfo(path).Directory.FullName : path;

        public static bool IsFilePath(string path) => !IsDirectoryPath(path);

        public static bool PathExists(string path) => File.Exists(path) || Directory.Exists(path);

        public static bool IsDirectoryPath(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch (Exception)
            {
                return IsPathValid(path) ? string.IsNullOrEmpty(Path.GetExtension(path)) : false;
            }
        }

        public static bool IsPathValid(string path)
        {
            try
            {
                return !string.IsNullOrEmpty(Path.GetFullPath(path));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DirectoryCopy(string sourceDirPath, string destDirPath, bool copySubDirs = true) => DirectoryCopy(sourceDirPath, destDirPath, "*", copySubDirs);

        /// <summary> fileSearchPatterns accepts multiple patterns splitted by "|" </summary>
        public static void DirectoryCopy(string sourceDirPath, string destDirPath, string fileSearchPatterns, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + nameof(sourceDirPath));
            }

            Directory.CreateDirectory(destDirPath);
            foreach (FileInfo file in EnumerateFileInfos(sourceDirPath, fileSearchPatterns))
            {
                string destFilePath = Path.Combine(destDirPath, file.Name);
                file.CopyTo(destFilePath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                {
                    string destSubDirPath = Path.Combine(destDirPath, subDir.Name);
                    DirectoryCopy(subDir.FullName, destSubDirPath, fileSearchPatterns, copySubDirs);
                }
            }
        }

        public static bool ShowOverwriteDialog(string filePath)
        {
            MessageBoxResult result = MessageBox.Show($"File {filePath} already exists.\nDo you want to overwrite it?", "Existing file conflict", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static void DirectoryCopy(string sourceDirPath, string destDirPath, HashSet<string> searchFileExtensions, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + nameof(sourceDirPath));
            }

            Directory.CreateDirectory(destDirPath);
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                if (searchFileExtensions.Contains(file.Extension))
                {
                    string destFilePath = Path.Combine(destDirPath, file.Name);
                    file.CopyTo(destFilePath, false);
                }
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                {
                    string destSubDirPath = Path.Combine(destDirPath, subDir.Name);
                    DirectoryCopy(subDir.FullName, destSubDirPath, searchFileExtensions, copySubDirs);
                }
            }
        }

        public static void MoveDirectoriesAndFiles(string from, string destination)
        {
            foreach (string folder in Directory.EnumerateDirectories(from))
            {
                Directory.Move(folder, Path.Combine(destination, new DirectoryInfo(folder).Name));
            }
            foreach (string file in Directory.EnumerateFiles(from))
            {
                File.Move(file, Path.Combine(destination, new FileInfo(file).Name));
            }
        }

        public static void GenerateFolders(string rootPath, params string[] generatedFolders)
        {
            Directory.CreateDirectory(rootPath); // create root even if generatedFolders is null
            foreach (string folder in generatedFolders)
            {
                Directory.CreateDirectory(Path.Combine(rootPath, folder));
            }
        }

        public static void DeleteFileToBin(string filePath) => FileSystem.DeleteFile(filePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        public static void DeleteDirectoryToBin(string directoryPath) => FileSystem.DeleteDirectory(directoryPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);

        public static bool TryDeleteToBin(this FileSystemInfo fileSystemInfo)
        {
            if (PathExists(fileSystemInfo.FullName))
            {
                try
                {
                    DeleteToBin(fileSystemInfo);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void DeleteToBin(this FileSystemInfo fileSystemInfo)
        {
            if (IsFilePath(fileSystemInfo.FullName))
            {
                DeleteFileToBin(fileSystemInfo.FullName);
            }
            else if (IsDirectoryPath(fileSystemInfo.FullName))
            {
                DeleteDirectoryToBin(fileSystemInfo.FullName);
            }
            else
            {
                throw new NotSupportedException(nameof(fileSystemInfo) + " is not valid path");
            }
        }

        public static string NormalizePath(this string path, bool forwardSlash = true) => forwardSlash ? path.Replace("\\", "/") : path.Replace("/", "\\");

        public static string NormalizeFullPath(this string path, bool forwardSlash = true) => forwardSlash ? Path.GetFullPath(path).Replace("\\", "/") : Path.GetFullPath(path).Replace("/", "\\");

        public static long GetLineCount(this Stream stream)
        {
            long lineCount = 0L;

            byte[] byteBuffer = new byte[1024 * 1024];
            char detectedEOL = NULLCHAR;
            char currentChar = NULLCHAR;

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULLCHAR)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }

            if (currentChar != LF && currentChar != CR && currentChar != NULLCHAR)
            {
                lineCount++;
            }
            stream.Position = 0;
            return lineCount;
        }

        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\').WithEnding("\\"));
            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\').WithEnding("\\"));
            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
            {
                return ending;
            }
            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                {
                    return tmp;
                }
            }
            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }
            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }
    }
}
