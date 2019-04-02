using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Data;

namespace ForgeModGenerator
{
    public class WpfObservableFolder<T> : ObservableFolder<T>
        where T : IFileSystemObject
    {
        public WpfObservableFolder(string path) : base(path) { }

        public WpfObservableFolder(IEnumerable<string> filePaths) : base(filePaths) { }

        public WpfObservableFolder(IEnumerable<T> files) : base(files) { }

        public WpfObservableFolder(string path, IEnumerable<string> filePaths) : base(path, filePaths) { }

        public WpfObservableFolder(string path, IEnumerable<T> files) : base(path, files) { }

        public WpfObservableFolder(string path, SearchOption searchOption) : base(path, searchOption) { }

        public WpfObservableFolder(string path, string fileSearchPatterns) : base(path, fileSearchPatterns) { }

        public WpfObservableFolder(string path, string fileSearchPatterns, SearchOption searchOption) : base(path, fileSearchPatterns, searchOption) { }

        protected WpfObservableFolder() { }

        protected override void InitializeFiles(ObservableRangeCollection<T> value)
        {
            if (value != null && !(value is WpfObservableRangeCollection<T>))
            {
                value = new WpfObservableRangeCollection<T>(value);
            }
            base.InitializeFiles(value);
        }
    }
}
