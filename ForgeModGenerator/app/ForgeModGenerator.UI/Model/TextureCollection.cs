using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    public struct TextureCollection
    {
        public string HeaderName { get; set; }
        public string DestinationPath { get; set; }
        public ObservableCollection<string> Textures { get; set; }

        public TextureCollection(string destinationPath)
        {
            DestinationPath = destinationPath;
            HeaderName = new DirectoryInfo(DestinationPath).Name;
            Textures = new ObservableCollection<string>();
        }

        public TextureCollection(string destinationPath, ObservableCollection<string> textures) : this(destinationPath)
        {
            Textures = textures;
        }
    }
}
