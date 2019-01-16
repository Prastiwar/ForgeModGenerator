using System.IO;
using System.IO.Compression;

namespace ForgeModGenerator.Model
{
    public struct ForgeVersion
    {
        public string Name { get; set; }
        public string ZipPath { get; set; }

        public ForgeVersion(string zipPath)
        {
            ZipPath = zipPath ?? throw new System.ArgumentNullException(nameof(zipPath));
            Name = Path.GetFileNameWithoutExtension(zipPath).Replace("-mdk", "");
        }

        public void UnZip(string destination)
        {
            ZipFile.ExtractToDirectory(ZipPath, destination);
        }
    }
}
