using ForgeModGenerator.Models;
using ForgeModGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator
{
    public abstract class FileSystemInfoReference : ObservableDirtyObject
    {
        protected abstract class RefCounter
        {
            public FileSystemInfo File;
            public int ReferenceCount;

            public RefCounter(string filePath)
            {
                File = CreateInstance(filePath);
                ReferenceCount = 1;
            }
            protected abstract FileSystemInfo CreateInstance(string path);
        }

        protected class FileRefCounter : RefCounter
        {
            public FileRefCounter(string filePath) : base(filePath) { }
            protected override FileSystemInfo CreateInstance(string path) => new FileInfo(path);
        }

        protected class DirectoryRefCounter : RefCounter
        {
            public DirectoryRefCounter(string filePath) : base(filePath) { }
            protected override FileSystemInfo CreateInstance(string path) => new DirectoryInfo(path);
        }

        private FileSystemInfo fileSystemInfo;
        public FileSystemInfo FileSystemInfo {
            get => fileSystemInfo;
            protected set {
                if (DirtSet(ref fileSystemInfo, value))
                {
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(FullName));
                }
            }
        }

        public string Name => FileSystemInfo.Name;
        public string FullName => FileSystemInfo.FullName;

        public FileSystemInfoReference(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            FileSystemInfo = GetOrCreate(path);
        }

        public virtual bool Rename(string newPath)
        {
            if (newPath == null)
            {
                return false;
            }
            if (references.TryGetValue(newPath, out RefCounter newReference))
            {
                if (!IsReferenced(FileSystemInfo.FullName))
                {
                    FileSystemInfo.DeleteToBin();
                }
                FileSystemInfo = newReference.File;
            }
            else
            {
                try
                {
                    if (IOExtensions.IsDirectoryPath(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                        Directory.Move(FullName, newPath);
                    }
                    else
                    {
                        new FileInfo(newPath).Directory.Create();
                        File.Move(FullName, newPath);
                        try
                        {
                            bool noFilesLeft = !IOExtensions.EnumerateAllFiles(FullName).Any();
                            if (noFilesLeft)
                            {
                                new FileInfo(FullName).Directory.Delete(true);
                            }
                        }
                        catch (Exception)
                        {
                            File.Move(newPath, FullName);
                            return false;
                        }
                    }
                    AddReference(newPath);
                    FileSystemInfo = references[newPath].File;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool Remove()
        {
            System.Windows.MessageBox.Show(GetReferenceCount(FullName).ToString());
            return GetReferenceCount(FullName) > 1 ? true : FileSystemInfo.TryDeleteToBin();
        }

        protected FileSystemInfo GetOrCreate(string filePath)
        {
            if (references.TryGetValue(filePath, out RefCounter reference))
            {
                return reference.File;
            }
            AddReference(filePath);
            return references[filePath].File;
        }

        public int GetReferenceCount() => GetReferenceCount(FullName);

        private static Dictionary<string, RefCounter> references = new Dictionary<string, RefCounter>();

        public static bool IsReferenced(string filePath) => references.ContainsKey(filePath);
        public static int GetReferenceCount(string filePath) => references.TryGetValue(filePath, out RefCounter TFileSystemInfoReference) ? TFileSystemInfoReference.ReferenceCount : 0;

        protected void AddReference(string filePath)
        {
            if (filePath == null)
            {
                return;
            }
            if (references.ContainsKey(filePath))
            {
                references[filePath].ReferenceCount++;
            }
            else
            {
                references[filePath] = CreateRefCounter(filePath);
            }
        }

        protected bool RemoveReference(string filePath)
        {
            if (filePath == null)
            {
                return false;
            }
            if (references.ContainsKey(filePath))
            {
                references[filePath].ReferenceCount--;
                if (references[filePath].ReferenceCount <= 0)
                {
                    return references.Remove(filePath);
                }
                return true;
            }
            return false;
        }

        protected abstract RefCounter CreateRefCounter(string path);
    }

    public class FileInfoReference : FileSystemInfoReference
    {
        public FileInfoReference(string filePath) : base(filePath) { }
        protected override RefCounter CreateRefCounter(string path) => new FileRefCounter(path);
    }

    public class DirectoryInfoReference : FileSystemInfoReference
    {
        public DirectoryInfoReference(string filePath) : base(filePath) { }
        protected override RefCounter CreateRefCounter(string path) => new DirectoryRefCounter(path);
    }
}
