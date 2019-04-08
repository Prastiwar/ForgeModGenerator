using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ForgeModGenerator
{
    public class WpfObservableFolder<T> : ObservableFolder<T>
        where T : IFileSystemObject
    {
        public static WpfObservableFolder<T> CreateEmpty() => new WpfObservableFolder<T>();

        protected WpfObservableFolder() => Files = new WpfObservableRangeCollection<T>();

        public WpfObservableFolder(string path) : base(path) { }
        public WpfObservableFolder(IEnumerable<string> filePaths) : base(filePaths) { }
        public WpfObservableFolder(IEnumerable<T> files) : base(files) { }
        public WpfObservableFolder(string path, IEnumerable<string> filePaths) : base(path, filePaths) { }
        public WpfObservableFolder(string path, IEnumerable<T> files) : base(path, files) { }

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
