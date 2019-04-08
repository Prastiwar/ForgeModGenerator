using System;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Utility
{
    public static class IOExtensions
    {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULLCHAR = (char)0;

        /// <summary> Replaces inconsistent path separator to forwardSlash or backwards </summary>
        public static string NormalizePath(this string path, bool forwardSlash = true) => forwardSlash ? path.Replace("\\", "/") : path.Replace("/", "\\");

        /// <summary> Replaces inconsistent path separator to forwardSlash or backwards after getting FullPath </summary>
        public static string NormalizeFullPath(this string path, bool forwardSlash = true) => forwardSlash ? Path.GetFullPath(path).Replace("\\", "/") : Path.GetFullPath(path).Replace("/", "\\");

        /// <summary> Compares two paths and returns true if they're equal. Always returns false if one of path is not valid path </summary>
        public static bool ComparePath(this string path, string otherPath)
        {
            if (!IOHelper.IsPathValid(path) || !IOHelper.IsPathValid(otherPath))
            {
                return false;
            }
            return string.Compare(path.NormalizeFullPath(), otherPath.NormalizeFullPath(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary> Returns string without `/` or `\` separator on the end </summary>
        public static string TrimLastSeparator(this string directoryPath)
        {
            char lastChar = directoryPath[directoryPath.Length - 1];
            if (lastChar == '/' || lastChar == '\\')
            {
                return directoryPath.Substring(0, directoryPath.Length - 1);
            }
            return directoryPath;
        }

        /// <summary> Rename file or directory name </summary>
        /// <param name="newName"> New file or directory name </param>
        /// <param name="changeExtension"> Defines if <paramref name="newName"/> should change file extension for file </param>
        /// <exception cref="IOException"> Thrown when one of parameters are null </exception>
        /// <exception cref="ArgumentNullException"> Thrown when file with <paramref name="newName"/> already exists </exception>
        public static void Rename(this FileSystemInfo fileSystemInfo, string newName, bool changeExtension = false)
        {
            if (fileSystemInfo is FileInfo fileInfo)
            {
                Rename(fileInfo, newName, changeExtension);
            }
            else if (fileSystemInfo is DirectoryInfo dirInfo)
            {
                Rename(dirInfo, newName);
            }
        }

        /// <summary> Rename directory name </summary>
        /// <param name="newName"> New directory name </param>
        /// <exception cref="IOException"> Thrown when one of parameters are null </exception>
        /// <exception cref="ArgumentNullException"> Thrown when directory with <paramref name="newName"/> already exists </exception>
        public static void Rename(this DirectoryInfo dirInfo, string newName)
        {
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException(dirInfo.FullName);
            }
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }
            string oldExactName = dirInfo.Parent.EnumerateDirectories(dirInfo.Name).First().Name;
            if (!string.Equals(oldExactName, newName, StringComparison.CurrentCulture))
            {
                string folder = Path.GetDirectoryName(dirInfo.FullName.TrimLastSeparator());
                string newPath = Path.Combine(folder, newName);
                bool changeCase = string.Equals(oldExactName, newName, StringComparison.CurrentCultureIgnoreCase);

                if (Directory.Exists(newPath) && !changeCase)
                {
                    throw new IOException($"Directory already exists: {newPath}");
                }
                else if (changeCase)
                {
                    // Move fails when changing case, so need to perform two moves
                    string tempPath = Path.Combine(folder, Guid.NewGuid().ToString());
                    dirInfo.MoveTo(tempPath);
                    dirInfo.MoveTo(newPath);
                }
                else
                {
                    dirInfo.MoveTo(newPath);
                }
            }
        }

        /// <summary> Rename file name </summary>
        /// <param name="newName"> New file name </param>
        /// <param name="changeExtension"> Defines if <paramref name="newName"/> should change file extension </param>
        /// <exception cref="IOException"> Thrown when one of parameters are null </exception>
        /// <exception cref="ArgumentNullException"> Thrown when file with <paramref name="newName"/> already exists </exception>
        public static void Rename(this FileInfo fileInfo, string newName, bool changeExtension = false)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(fileInfo.FullName);
            }
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }
            string oldExactNAme = fileInfo.Directory.EnumerateFiles(fileInfo.Name).First().Name;
            if (!string.Equals(oldExactNAme, newName, StringComparison.CurrentCulture))
            {
                string folder = Path.GetDirectoryName(fileInfo.FullName);
                string newPath = !changeExtension ? Path.Combine(folder, newName + fileInfo.Extension) : Path.Combine(folder, newName);
                bool changeCase = string.Equals(oldExactNAme, newName, StringComparison.CurrentCultureIgnoreCase);

                if (File.Exists(newPath) && !changeCase)
                {
                    throw new IOException($"File already exists: {newPath}");
                }
                fileInfo.MoveTo(newPath);
            }
        }

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
