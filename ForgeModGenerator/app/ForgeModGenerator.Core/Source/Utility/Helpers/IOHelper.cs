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
        /// <summary> Returns DirectoryInfo or FileInfo as FileSystemInfo when paths exists </summary>
        public static FileSystemInfo GetFileSystemInfo(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                return fileInfo;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                return dirInfo;
            }
            return null;
        }

        public static string GetRenamedFileFullName(string oldFullPath, string newName, bool changeExtension = false)
        {
            FileInfo info = new FileInfo(oldFullPath);
            string folder = Path.GetDirectoryName(oldFullPath);
            return !changeExtension ? Path.Combine(folder, newName + info.Extension) : Path.Combine(folder, newName);
        }

        public static string GetRenamedDirectoryFullName(string oldFullPath, string newName)
        {
            DirectoryInfo info = new DirectoryInfo(oldFullPath);
            string folder = Path.GetDirectoryName(oldFullPath);
            return Path.Combine(folder, newName);
        }

        /// <summary> Rename file name </summary>
        /// <param name="oldFullPath"> Full path of file to be renamed </param>
        /// <param name="newName"> New file name </param>
        /// <param name="changeExtension"> Defines if <paramref name="newName"/> should change file extension </param>
        /// <exception cref="IOException"> Thrown when one of parameters are null </exception>
        /// <exception cref="ArgumentNullException"> Thrown when file with <paramref name="newName"/> already exists </exception>
        public static void RenameFile(string oldFullPath, string newName, bool changeExtension = false) => new FileInfo(oldFullPath).Rename(newName, changeExtension);

        /// <summary> Rename directory name </summary>
        /// <param name="oldFullPath"> Full path of directory to be renamed </param>
        /// <param name="newName"> New directory name </param>
        /// <exception cref="IOException"> Thrown when one of parameters are null </exception>
        /// <exception cref="ArgumentNullException"> Thrown when directory with <paramref name="newName"/> already exists </exception>
        public static void RenameDirectory(string oldFullPath, string newName) => new DirectoryInfo(oldFullPath).Rename(newName);

        /// <summary> Recursively copy directory </summary>
        /// <param name="sourceDirPath"> Full path of directory to be copied </param>
        /// <param name="destDirPath"> Full path of location where directory should be pasted </param>
        /// <param name="copySubDirs"> Should copy every sub directory? </param>
        /// <exception cref="DirectoryNotFoundException"> Thrown when <paramref name="sourceDirPath"/> doesn't exists </exception>
        public static void DirectoryCopy(string sourceDirPath, string destDirPath, bool copySubDirs = true) => DirectoryCopy(sourceDirPath, destDirPath, "*", copySubDirs);

        /// <summary> Recursively copy directory </summary>
        /// <param name="sourceDirPath"> Full path of directory to be copied </param>
        /// <param name="destDirPath"> Full path of location where directory should be pasted </param>
        /// <param name="fileSearchPatterns"> String accepts multiple patterns splitted by "|" </param>
        /// <param name="copySubDirs"> Should copy every sub directory? </param>
        /// <exception cref="DirectoryNotFoundException"> Thrown when <paramref name="sourceDirPath"/> doesn't exists </exception>
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
            if (string.IsNullOrEmpty(patterns))
            {
                patterns = "*";
            }
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
            if (string.IsNullOrEmpty(patterns))
            {
                patterns = "*";
            }
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
            if (string.IsNullOrEmpty(patterns))
            {
                patterns = "*";
            }
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
            if (string.IsNullOrEmpty(patterns))
            {
                patterns = "*";
            }
            foreach (string pattern in patterns.Split('|'))
            {
                foreach (DirectoryInfo file in new DirectoryInfo(path).EnumerateDirectories(pattern, searchOption))
                {
                    yield return file;
                }
            }
        }

        public static bool HasAnyFile(string directoryPath) => Directory.EnumerateFiles(directoryPath).Any();

        public static bool HasSubDirectories(string directoryPath) => Directory.EnumerateDirectories(directoryPath).Any();

        public static bool IsEmpty(string directoryPath) => !Directory.EnumerateFileSystemEntries(directoryPath).Any();

        /// <summary> if path is file, returns its directory, else returns path </summary>
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

        /// <summary>
        /// Create all directories inside <paramref name="rootPath"/>
        /// </summary>
        /// <param name="rootPath"> Full path where folders should be created. It will be created if doesn't exist </param>
        /// <param name="folderNames"> Directory names to create </param>
        public static void GenerateFolders(string rootPath, params string[] folderNames)
        {
            Directory.CreateDirectory(rootPath); // create root even if generatedFolders is empty
            if (folderNames != null)
            {
                foreach (string folder in folderNames)
                {
                    Directory.CreateDirectory(Path.Combine(rootPath, folder));
                }
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

        /// <summary> Increments <paramref name="name"/> as format Name(i) till <paramref name="isUnique"/> is true </summary>
        /// <param name="name"> name that should be unique </param>
        /// <param name="isUnique"> Func to define if name is unique </param>
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

        /// <summary> Recursively copy directory </summary>
        /// <param name="sourceDirPath"> Full path of directory to be copied </param>
        /// <param name="destDirPath"> Full path of location where directory should be pasted </param>
        /// <param name="fileSearchPatterns"> String accepts multiple patterns splitted by "|" </param>
        /// <param name="copySubDirs"> Should copy every sub directory? </param>
        /// <exception cref="DirectoryNotFoundException"> Thrown when <paramref name="sourceDirPath"/> doesn't exists </exception>
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
