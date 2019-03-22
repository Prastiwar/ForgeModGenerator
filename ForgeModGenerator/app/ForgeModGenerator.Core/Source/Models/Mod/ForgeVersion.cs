using System.IO;
using System.IO.Compression;

namespace ForgeModGenerator.Models
{
    public class ForgeVersion
    {
        public string Name { get; set; }
        public string ZipPath { get; set; }

        public ForgeVersion(string name, string zipPath)
        {
            ZipPath = zipPath ?? throw new System.ArgumentNullException(nameof(zipPath));
            Name = name ?? Path.GetFileNameWithoutExtension(zipPath).Replace("-mdk", "");
        }

        public ForgeVersion(string zipPath) : this(Path.GetFileNameWithoutExtension(zipPath).Replace("-mdk", ""), zipPath) { }

        public void UnZip(string destination) => ZipFile.ExtractToDirectory(ZipPath, destination);

        public override bool Equals(object obj) => obj is ForgeVersion objForgeVersion && (objForgeVersion.Name == Name && objForgeVersion.ZipPath == ZipPath);
        public override int GetHashCode() => base.GetHashCode();
    }
}
