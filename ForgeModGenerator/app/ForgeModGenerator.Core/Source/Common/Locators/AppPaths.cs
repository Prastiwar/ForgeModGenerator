using System;
using System.IO;

namespace ForgeModGenerator
{
    /// <summary> Constains common application path locations </summary>
    public static class AppPaths
    {
        private static string AppRootPath => new DirectoryInfo(Binary).Parent.FullName;

        public static string Binary => Path.Combine(Environment.CurrentDirectory + "bin");

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
