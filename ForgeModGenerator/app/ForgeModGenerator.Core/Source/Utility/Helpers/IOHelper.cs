using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeModGenerator.Utility
{
    public static class IOHelper
    {
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

        public static IEnumerable<string> EnumerateAllFiles(string path) => Directory.EnumerateFiles(GetDirectoryPath(path), "*", SearchOption.AllDirectories);
        public static IEnumerable<string> EnumerateAllDirectories(string path) => Directory.EnumerateDirectories(GetDirectoryPath(path), "*", SearchOption.AllDirectories);
        public static IEnumerable<FileInfo> EnumerateAllFileInfos(string path) => new DirectoryInfo(GetDirectoryPath(path)).EnumerateFiles("*", SearchOption.AllDirectories);
        public static IEnumerable<DirectoryInfo> EnumerateAllDirectoryInfos(string path) => new DirectoryInfo(GetDirectoryPath(path)).EnumerateDirectories("*", SearchOption.AllDirectories);

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

        public static bool HasAnyFile(string path) => Directory.EnumerateFiles(path).Any();

        public static bool HasSubDirectories(string path) => Directory.EnumerateDirectories(path).Any();

        public static bool IsEmpty(string path) => !Directory.EnumerateFileSystemEntries(path).Any();

        /// <summary> if path is file, return it's directory, else return path </summary>
        public static string GetDirectoryPath(string path) => IsFilePath(path) ? new FileInfo(path).Directory.FullName : path;

        public static bool PathExists(string path) => File.Exists(path) || Directory.Exists(path);

        /// <summary> 
        /// Defines is path is (or could be if not exists) file.
        /// IMPORTANT: If not exists, name without extension is not considered to be file
        /// </summary>
        public static bool IsFilePath(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return !attr.HasFlag(FileAttributes.Directory);
            }
            catch (Exception)
            {
                return IsPathValid(path) ? !string.IsNullOrEmpty(Path.GetExtension(path)) : false;
            }
        }

        /// <summary> 
        /// Defines is path is (or could be if not exists) directory.
        /// IMPORTANT: If not exists, name with extension is not considered to be directory
        /// </summary>
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

        public static void GenerateFolders(string rootPath, params string[] generatedFolders)
        {
            Directory.CreateDirectory(rootPath); // create root even if generatedFolders is empty
            foreach (string folder in generatedFolders)
            {
                Directory.CreateDirectory(Path.Combine(rootPath, folder));
            }
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

        /// <summary> fileSearchPatterns accepts multiple patterns splitted by "|" </summary>
        public static async Task DirectoryCopyAsync(string sourceDirPath, string destDirPath, string fileSearchPatterns, bool copySubDirs = true)
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
                using (FileStream SourceStream = file.OpenRead())
                {
                    using (FileStream DestinationStream = File.Create(destFilePath))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                {
                    string destSubDirPath = Path.Combine(destDirPath, subDir.Name);
                    await DirectoryCopyAsync(subDir.FullName, destSubDirPath, fileSearchPatterns, copySubDirs);
                }
            }
        }

        public static async Task AppendAllTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        public static async Task WriteAllTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        public static async Task<string> ReadAllTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }
    }
}
