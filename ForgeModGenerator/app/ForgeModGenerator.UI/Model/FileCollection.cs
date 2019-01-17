using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public class FileCollection
    {
        public string HeaderName { get; set; }
        public string DestinationPath { get; set; }
        public ObservableCollection<string> Paths { get; set; }

        public FileCollection(string destinationPath)
        {
            DestinationPath = destinationPath;
            HeaderName = new DirectoryInfo(DestinationPath).Name;
            Paths = new ObservableCollection<string>();
        }

        public FileCollection(string destinationPath, ObservableCollection<string> textures) : this(destinationPath)
        {
            Paths = textures;
        }
    }
}
