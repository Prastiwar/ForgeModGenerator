using System.IO;
using System.Linq;
using System.Reflection;

namespace ForgeModGenerator
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class FileSystemWatcherExtended : FileSystemWatcher
    {
        public FileSystemWatcherExtended() : this(string.Empty) { }

        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        public FileSystemWatcherExtended(string path) : this(path, "*") { }

        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        /// <param name="filters">The types of files to watch separated with "|". For example, "*.txt|*.png" watches for changes to all text or png images.</param>
        public FileSystemWatcherExtended(string path, string filters) : base(path, "*")
        {
            this.filters = filters.Split('|');
            strictMatchPatternMethodInfo = typeof(FileSystemWatcher).Assembly.GetTypes().First(x => x.Name == "PatternMatcher").GetMethod("StrictMatchPattern");
            FileSystemWatcher baseInstance = this;
            baseInstance.Changed += NotifyEvent;
            baseInstance.Created += NotifyEvent;
            baseInstance.Deleted += NotifyEvent;
            baseInstance.Renamed += NotifyEvent;
        }

        /// <summary> Defines if should monitor folders despite of "filters" construction </summary>
        public bool MonitorDirectoryChanges { get; set; }

        /// <summary> Array of common filter used in base class </summary>
        private readonly string[] filters;

        /// <summary> Cached two elements parameters array for StrictMatchPattern method </summary>
        private readonly string[] parameters = new string[] { "", "" };

        /// <summary> Method from internal class (System.IO.PatternMatcher.StrictMatchPattern(string expression, string name)) </summary>
        private readonly MethodInfo strictMatchPatternMethodInfo;

        private FileSystemEventHandler onChangedHandler = null;
        /// <summary> Occurs when a file or directory in the specified Path is changed. </summary>
        public new event FileSystemEventHandler Changed {
            add => onChangedHandler += value;
            remove => onChangedHandler -= value;
        }

        private FileSystemEventHandler onCreatedHandler = null;
        /// <summary> Occurs when a file or directory in the specified Path is created. </summary>
        public new event FileSystemEventHandler Created {
            add => onCreatedHandler += value;
            remove => onCreatedHandler -= value;
        }

        private FileSystemEventHandler onDeletedHandler = null;
        /// <summary> Occurs when a file or directory in the specified Path is deleted. </summary>
        public new event FileSystemEventHandler Deleted {
            add => onDeletedHandler += value;
            remove => onDeletedHandler -= value;
        }

        private RenamedEventHandler onRenamedHandler = null;
        /// <summary> Occurs when a file or directory in the specified Path is renamed. </summary>
        public new event RenamedEventHandler Renamed {
            add => onRenamedHandler += value;
            remove => onRenamedHandler -= value;
        }

        protected void NotifyEvent(object sender, FileSystemEventArgs e)
        {
            bool monitorDirectory = MonitorDirectoryChanges && Utility.IOHelper.IsDirectoryPath(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                RenamedEventArgs args = (RenamedEventArgs)e;
                if (monitorDirectory || StrictMatchPattern(filters, args.OldName))
                {
                    onRenamedHandler?.Invoke(sender, args);
                }
            }
            else if (monitorDirectory || StrictMatchPattern(filters, e.Name))
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        onCreatedHandler?.Invoke(sender, e);
                        break;
                    case WatcherChangeTypes.Deleted:
                        onDeletedHandler?.Invoke(sender, e);
                        break;
                    case WatcherChangeTypes.Changed:
                        onChangedHandler?.Invoke(sender, e);
                        break;
                    default:
                        break;
                }
            }
        }

        protected bool StrictMatchPattern(string[] expressions, string name)
        {
            bool nameMatches = false;
            foreach (string filter in filters)
            {
                if (StrictMatchPattern(filter, name))
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
