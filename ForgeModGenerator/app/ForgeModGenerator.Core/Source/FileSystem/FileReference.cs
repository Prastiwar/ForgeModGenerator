using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator
{
    // Base class that manages file references in application and synchronizes it with explorer
    public abstract class FileSystemInfoReference : BindableBase, IDataErrorInfo
    {
        public ValidateResult IsValid => new ValidateResult(((IDataErrorInfo)this)[nameof(ChangeName)] != null, "Path is not valid");

        private string error;
        string IDataErrorInfo.Error => error;
        string IDataErrorInfo.this[string propertyName] => ((IDataErrorInfo)this).Error;

        protected abstract class RefCounter
        {
            public FileSystemInfo File { get; }
            public int ReferenceCount;

            public RefCounter(string filePath)
            {
                File = CreateInstance(filePath);
                ReferenceCount = 1;
            }
            protected abstract FileSystemInfo CreateInstance(string path);
        }

        protected sealed class FileRefCounter : RefCounter
        {
            public FileRefCounter(string filePath) : base(filePath) { }
            protected override FileSystemInfo CreateInstance(string path) => new FileInfo(path);
        }

        protected sealed class DirectoryRefCounter : RefCounter
        {
            public DirectoryRefCounter(string filePath) : base(filePath) { }
            protected override FileSystemInfo CreateInstance(string path) => new DirectoryInfo(path);
        }

        public FileSystemInfoReference(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            FileSystemInfo = GetOrCreateInfo(path);
        }

        private FileSystemInfo fileSystemInfo;
        public FileSystemInfo FileSystemInfo {
            get => fileSystemInfo;
            protected set {
                if (SetProperty(ref fileSystemInfo, value))
                {
                    changeName = FileSystemInfo is DirectoryInfo ? Name : Path.GetFileNameWithoutExtension(Name);
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(FullName));
                    RaisePropertyChanged(nameof(ChangeName));
                }
            }
        }

        private string changeName;
        public string ChangeName {
            get => changeName;
            set {
                if (!SetProperty(ref changeName, value))
                {
                    return;
                }
                string newPath = null;
                if (FileSystemInfo is DirectoryInfo)
                {
                    newPath = new DirectoryInfo(FullName).Parent.FullName + "\\" + value;
                }
                else
                {
                    string valueWithExt = value + Path.GetExtension(Name);
                    newPath = new FileInfo(FullName).Directory.FullName + "\\" + valueWithExt;
                }
                if (FullName != newPath)
                {
                    throw new NotImplementedException();
                    //FluentValidation.Results.ValidationResult validationResults = new FullPathValidator().Validate(newPath);
                    //if (validationResults.IsValid)
                    //{
                    //    if (IOHelper.IsFilePath(FullName))
                    //    {
                    //        IOHelperWin.MoveFile(FullName, newPath);
                    //    }
                    //    else
                    //    {
                    //        IOHelperWin.MoveDirectory(FullName, newPath);
                    //    }
                    //    SetInfo(newPath);
                    //    error = null;
                    //}
                    //else
                    //{
                    //    error = validationResults.ToString();
                    //}
                }
                else
                {
                    error = null;
                }
            }
        }

        public string Name => FileSystemInfo.Name;
        public string FullName => FileSystemInfo.FullName;

        public int GetReferenceCount() => GetReferenceCount(FullName);

        public virtual void SetInfo(string newPath)
        {
            newPath = newPath.NormalizeFullPath();
            Remove();
            if (references.TryGetValue(newPath, out RefCounter newReference))
            {
                FileSystemInfo = newReference.File;
                newReference.ReferenceCount++;
            }
            else
            {
                AddReference(newPath);
                FileSystemInfo = references[newPath].File;
            }
        }

        /// <summary> Removes file reference </summary>
        public void Remove()
        {
            string path = FullName.NormalizeFullPath();
            RemoveReference(path);
        }

        protected FileSystemInfo GetOrCreateInfo(string filePath)
        {
            filePath = filePath.NormalizeFullPath();
            AddReference(filePath);
            return references[filePath].File;
        }

        protected void AddReference(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            filePath = filePath.NormalizeFullPath();
            if (references.ContainsKey(filePath))
            {
                references[filePath].ReferenceCount++;
            }
            else
            {
                references[filePath] = CreateRefCounter(filePath);
            }
        }

        protected static bool RemoveReference(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            filePath = filePath.NormalizeFullPath();
            if (references.ContainsKey(filePath))
            {
                references[filePath].ReferenceCount--;
                if (references[filePath].ReferenceCount <= 0)
                {
                    return references.TryRemove(filePath, out RefCounter del);
                }
                return true;
            }
            return false;
        }

        private static ConcurrentDictionary<string, RefCounter> references = new ConcurrentDictionary<string, RefCounter>();

        public static bool IsReferenced(string filePath) => GetReferenceCount(filePath.NormalizeFullPath()) > 0;
        public static int GetReferenceCount(string filePath) => references.TryGetValue(filePath.NormalizeFullPath(), out RefCounter refCounter) ? refCounter.ReferenceCount : 0;

        protected abstract RefCounter CreateRefCounter(string path);
    }

    // Wrapper for file references
    public sealed class FileInfoReference : FileSystemInfoReference
    {
        public FileInfoReference(string filePath) : base(filePath) { }
        protected override RefCounter CreateRefCounter(string path) => new FileRefCounter(path);
    }

    // Wrapper for directory references
    public sealed class DirectoryInfoReference : FileSystemInfoReference
    {
        public DirectoryInfoReference(string path) : base(IOHelper.GetDirectoryPath(path)) { }
        protected override RefCounter CreateRefCounter(string path) => new DirectoryRefCounter(IOHelper.GetDirectoryPath(path));
    }
}
