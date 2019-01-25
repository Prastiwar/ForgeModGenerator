using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public class FileCollection : ObservableCollection<string>
    {
        public string HeaderName { get; set; }
        public string DestinationPath { get; set; }

        public FileCollection(string destinationPath) : base()
        {
            DestinationPath = destinationPath;
            HeaderName = new DirectoryInfo(DestinationPath).Name;
        }

        public FileCollection(string destinationPath, IEnumerable<string> textures) : this(destinationPath)
        {
            CopyFrom(textures);
        }

        private void CopyFrom(IEnumerable<string> collection)
        {
            IList<string> items = Items;
            if (collection != null && items != null)
            {
                using (IEnumerator<string> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }
    }
}
