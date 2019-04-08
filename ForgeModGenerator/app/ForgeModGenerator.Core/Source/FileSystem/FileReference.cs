using ForgeModGenerator.Utility;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace ForgeModGenerator
{
    public sealed class FileSystemInfoReference : BindableBase, IDisposable
    {
        private class RefCounter
        {
            public FileSystemInfo File { get; }
            public int ReferenceCount { get; set; }

            public RefCounter(string path)
            {
                File = IOHelper.GetFileSystemInfo(path);
                ReferenceCount = 1;
            }
        }

        /// <summary> Reference wrapper for FileSystemInfo </summary>
        /// <param name="path"> Full path to file or directory </param>
        /// <exception cref="ArgumentNullException"> Thrown when parameter is null </exception>
        /// <exception cref="ArgumentException"> Thrown when file or directory in <paramref name="path"/> doesn't exists </exception>
        public FileSystemInfoReference(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            else if (!IOHelper.PathExists(path))
            {
                throw new ArgumentException($"Path: {path} must exists to reference it");
            }
            Info = GetOrCreateInfo(path);
        }

        private FileSystemInfo info;
        public FileSystemInfo Info {
            get => !isDisposed ? info : throw new ObjectDisposedException("You cannot access disposed FileSystemInfoReference");
            private set {
                if (SetProperty(ref info, value))
                {
                    RaisePropertyChanged(nameof(NameWithoutExtension));
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(FullName));
                    RaisePropertyChanged(nameof(Extension));
                    RaisePropertyChanged(nameof(ReferenceCount));
                    RaisePropertyChanged(nameof(IsValidReference));
                }
            }
        }

        public string NameWithoutExtension => Info != null ? Path.GetFileNameWithoutExtension(Info.FullName) : null;
        public string Name => Info?.Name;
        public string FullName => Info?.FullName;
        public string Extension => Info?.Extension;
        public int ReferenceCount => GetReferenceCount(FullName);
        public bool IsValidReference => Info != null;

        private bool isDisposed = false;

        public void SetInfo(string newPath)
        {
            Dispose();
            isDisposed = false;
            Info = GetOrCreateInfo(newPath);
        }

        private FileSystemInfo GetOrCreateInfo(string path)
        {
            path = path.NormalizeFullPath();
            if (references.TryGetValue(path, out RefCounter newReference))
            {
                newReference.ReferenceCount++;
                return newReference.File;
            }
            return (references[path] = new RefCounter(path)).File;
        }

        /// <summary> Removes file reference </summary>
        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    RemoveReference(FullName);
                    Info = null;
                }
                isDisposed = true;
            }
        }

        private static ConcurrentDictionary<string, RefCounter> references = new ConcurrentDictionary<string, RefCounter>(2, 63);

        private static bool RemoveReference(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            filePath = filePath.NormalizeFullPath();
            if (references.ContainsKey(filePath))
            {
                references[filePath].ReferenceCount--;
                return references[filePath].ReferenceCount <= 0 ? references.TryRemove(filePath, out RefCounter del) : true;
            }
            return false;
        }
        public static bool IsReferenced(string filePath) => GetReferenceCount(filePath) > 0;
        public static int GetReferenceCount(string filePath) => references.TryGetValue(filePath.NormalizeFullPath(), out RefCounter refCounter) ? refCounter.ReferenceCount : 0;
    }
}
