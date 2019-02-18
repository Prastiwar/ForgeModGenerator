using System.IO;
using System.IO.Compression;

namespace ForgeModGenerator.ModGenerator.Models
{
    public class ForgeVersion
    {
        public string Name { get; set; }
        public string ZipPath { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        public ForgeVersion(string name, string zipPath)
        {
            Name = name;
            ZipPath = zipPath;
        }

        public ForgeVersion(string zipPath)
        {
            ZipPath = zipPath ?? throw new System.ArgumentNullException(nameof(zipPath));
            Name = Path.GetFileNameWithoutExtension(zipPath).Replace("-mdk", "");
        }

        public void UnZip(string destination)
        {
            ZipFile.ExtractToDirectory(ZipPath, destination);
        }

        public override bool Equals(object obj) => obj is ForgeVersion objForgeVersion && (objForgeVersion.Name == Name && objForgeVersion.ZipPath == ZipPath);
        public override int GetHashCode() => base.GetHashCode();
    }
}
