using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ForgeModGenerator
{
    public class FileSubPathEventArgs : EventArgs
    {
        public FileSubPathEventArgs(string oldFullPath, string newFullPath)
        {
            OldFullPath = oldFullPath;
            FullPath = newFullPath;
        }

        public string OldFullPath { get; }
        public string FullPath { get; }
    }

    public class FileSystemWatcherExtended : FileSystemWatcher
    {
        public FileSystemWatcherExtended() : this(string.Empty) { }

        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        public FileSystemWatcherExtended(string path) : this(path, "*") { }

        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        /// <param name="filters">The types of files to watch separated with "|". For example, "*.txt|*.png" watches for changes to all text or png images.</param>
        public FileSystemWatcherExtended(string path, string filters) : base(path, "*")
        {
            Filters = filters;
            strictMatchPatternMethodInfo = typeof(FileSystemWatcher).Assembly.GetTypes().First(x => x.Name == "PatternMatcher").GetMethod("StrictMatchPattern");
            Changed += NotifyEvent;
            Created += NotifyEvent;
            Deleted += NotifyEvent;
            Renamed += NotifyEvent;
        }

        /// <summary> Occurs when a file or directory in the specified Path that matches Filters is changed. </summary>
        public event FileSystemEventHandler FileChanged;

        /// <summary> Occurs when a file or directory in the specified Path that matches Filters is created. </summary>
        public event FileSystemEventHandler FileCreated;

        /// <summary> Occurs when a file or directory in the specified Path that matches Filters is deleted. </summary>
        public event FileSystemEventHandler FileDeleted;

        /// <summary> Occurs when a file or directory in the specified Path that matches Filters is renamed. </summary>
        public event RenamedEventHandler FileRenamed;

        /// <summary> Occurs when a directory with files in it in the specified Path that matches Filters is renamed. </summary>
        public event EventHandler<FileSubPathEventArgs> FileSubPathRenamed;

        /// <summary> 
        /// Defines if should monitor folders despite of "filters" construction
        /// NOTE: NotifyFilters must have NotifyFilters.DirectoryName flag
        /// </summary>
        public bool MonitorDirectoryChanges { get; set; }

        private string filters;
        /// <summary> 
        /// The types of files to watch separated with "|". For example, "*.txt|*.png" watches for changes to all text or png images.
        /// These Filters are applied after base Filter
        /// </summary>
        public string Filters {
            get => filters;
            set {
                if (string.IsNullOrEmpty(value))
                {
                    value = "*.*";
                }
                if (string.Compare(filters, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    filters = value;
                    filtersArray = Filters.Split('|');
                }
            }
        }

        /// <summary> Array of common filter used in base class </summary>
        private string[] filtersArray;

        /// <summary> Cached two elements parameters array for StrictMatchPattern method </summary>
        private readonly string[] parameters = new string[] { "", "" };

        /// <summary> Method from internal class (System.IO.PatternMatcher.StrictMatchPattern(string expression, string name)) </summary>
        private readonly MethodInfo strictMatchPatternMethodInfo;

        protected void NotifyEvent(object sender, FileSystemEventArgs e)
        {
            bool monitorDirectory = MonitorDirectoryChanges && Utility.IOHelper.IsDirectoryPath(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                RenamedEventArgs args = (RenamedEventArgs)e;
                if (monitorDirectory || StrictMatchPattern(filtersArray, args.OldName))
                {
                    FileRenamed?.Invoke(sender, args);
                    if (Directory.Exists(args.FullPath))
                    {
                        NotifySubPathChanged(this, args.OldFullPath, args.FullPath);
                    }
                }
            }
            else if (monitorDirectory || StrictMatchPattern(filtersArray, e.Name))
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        FileCreated?.Invoke(sender, e);
                        break;
                    case WatcherChangeTypes.Deleted:
                        FileDeleted?.Invoke(sender, e);
                        break;
                    case WatcherChangeTypes.Changed:
                        FileChanged?.Invoke(sender, e);
                        break;
                    default:
                        break;
                }
            }
        }

        protected void NotifySubPathChanged(object sender, string oldPath, string newPath)
        {
            if (FileSubPathRenamed != null)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(newPath);
                foreach (FileInfo file in dirInfo.EnumerateFiles())
                {
                    string oldFilePath = System.IO.Path.Combine(oldPath, file.Name);
                    FileSubPathRenamed.Invoke(sender, new FileSubPathEventArgs(oldFilePath, file.FullName));
                }
                foreach (DirectoryInfo subDir in dirInfo.EnumerateDirectories())
                {
                    string oldSubDirPath = System.IO.Path.Combine(oldPath, subDir.Name);
                    NotifySubPathChanged(sender, oldSubDirPath, subDir.FullName);
                }
            }
        }

        protected bool StrictMatchPattern(string[] expressions, string name)
        {
            bool nameMatches = false;
            foreach (string expression in expressions)
            {
                if (StrictMatchPattern(expression, name))
                {
                    nameMatches = true;
                    break;
                }
            }
            return nameMatches;
        }

        protected bool StrictMatchPattern(string expression, string name)
        {
            parameters[0] = expression;
            parameters[1] = name;
            return (bool)strictMatchPatternMethodInfo.Invoke(null, parameters);
        }
    }
}
