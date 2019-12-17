using System;
using System.IO;
using System.IO.Compression;

namespace ForgeModGenerator
{
    /// <summary> Constains common application path locations </summary>
    public static class AppPaths
    {
        private static string AppRootPath => new DirectoryInfo(Binary).Parent.FullName;

        public static string Binary => Path.Combine(Environment.CurrentDirectory + "bin");

        public static string Assets {
            get {
                string path = Path.Combine(AppRootPath, "assets");
                CreateIfNotExist(path);
                return path;
            }
        }

        private static bool wasMCItemIconsExtracted;
        public static string GetMCItemIconsPath()
        {
            string path = Path.Combine(Assets, "mcitemicons");
            CreateIfNotExist(path);
            if (!wasMCItemIconsExtracted)
            {
                ExtractMCItemIcons(path);
                wasMCItemIconsExtracted = true;
            }
            return path;
        }

        private static void ExtractMCItemIcons(string iconsPath)
        {
            string tempZipPath = Path.Combine(Assets, Guid.NewGuid().ToString() + ".zip");
            File.WriteAllBytes(tempZipPath, Core.Properties.Resources.MCItemIcons);
            ZipArchive zipArchive = ZipFile.OpenRead(tempZipPath);
            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                string filePath = Path.Combine(iconsPath, entry.Name);
                entry.ExtractToFile(filePath, true);
            }
            zipArchive.Dispose();
            File.Delete(tempZipPath);
        }

        public static string ForgeVersions {
            get {
                string path = Path.Combine(AppRootPath, "forgeversions");
                CreateIfNotExist(path);
                return path;
            }
        }

        public static string Mods {
            get {
                string path = Path.Combine(AppRootPath, "mods");
                CreateIfNotExist(path);
                return path;
            }
        }

        public static string Cache {
            get {
                string path = Path.Combine(AppRootPath, "cache");
                CreateIfNotExist(path);
                return path;
            }
        }

        public static string Preferences {
            get {
                string path = Path.Combine(Cache, "preferences");
                CreateIfNotExist(path);
                return path;
            }
        }

        public static string Logs {
            get {
                string path = Path.Combine(AppRootPath, "logs");
                CreateIfNotExist(path);
                return path;
            }
        }

        private static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
