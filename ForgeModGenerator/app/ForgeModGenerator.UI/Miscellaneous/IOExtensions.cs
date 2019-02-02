using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;
using SearchOption = System.IO.SearchOption;

namespace ForgeModGenerator.Miscellaneous
{
    public static class IOExtensions
    {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULLCHAR = (char)0;

        public static IEnumerable<string> EnumerateAllFiles(string path) => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
        public static IEnumerable<string> EnumerateAllDirectories(string path) => Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);

        public static void DeleteFileToBin(string filePath) => FileSystem.DeleteDirectory(filePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        public static void DeleteDirectoryToBin(string directoryPath) => FileSystem.DeleteFile(directoryPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);

        public static bool IsFilePath(string path) => !IsDirectoryPath(path);

        public static bool IsDirectoryPath(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch (System.Exception)
            {
                return IsPathValid(path) ? string.IsNullOrEmpty(Path.GetExtension(path)) : false;
            }
        }

        public static bool IsPathValid(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch (System.Exception)
            {
                return false;
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
    }
}
