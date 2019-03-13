using System;
using System.IO;

namespace ForgeModGenerator.Utility
{
    public static class IOExtensions
    {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULLCHAR = (char)0;

        public static bool TryDeleteToBin(this FileSystemInfo fileSystemInfo)
        {
            if (IOHelper.PathExists(fileSystemInfo.FullName))
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
            if (IOHelper.IsFilePath(fileSystemInfo.FullName))
            {
                IOHelper.DeleteDirectoryRecycle(fileSystemInfo.FullName);
            }
            else if (IOHelper.IsDirectoryPath(fileSystemInfo.FullName))
            {
                IOHelper.DeleteDirectoryRecycle(fileSystemInfo.FullName);
            }
            else
            {
                throw new NotSupportedException(nameof(fileSystemInfo) + " is not valid path");
            }
        }

        public static string NormalizePath(this string path, bool forwardSlash = true) => forwardSlash ? path.Replace("\\", "/") : path.Replace("/", "\\");

        public static string NormalizeFullPath(this string path, bool forwardSlash = true) => forwardSlash ? Path.GetFullPath(path).Replace("\\", "/") : Path.GetFullPath(path).Replace("/", "\\");

        public static bool ComparePath(this string path, string otherPath) => string.Compare(path.NormalizeFullPath(), otherPath.NormalizeFullPath(), StringComparison.OrdinalIgnoreCase) == 0;

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
