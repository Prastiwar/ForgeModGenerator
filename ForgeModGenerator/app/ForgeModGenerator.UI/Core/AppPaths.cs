using System;
using System.IO;

namespace ForgeModGenerator.Core
{
    public static class AppPaths
    {
        public static string Binary => Path.Combine(Environment.CurrentDirectory + "bin");
        public static string ForgeVersions => Path.Combine(new DirectoryInfo(Binary).Parent.FullName, "forgeversions");
        public static string Mods => Path.Combine(new DirectoryInfo(Binary).Parent.FullName, "mods");
        public static string Templates => Path.Combine(new DirectoryInfo(Binary).Parent.FullName, "templates");
        public static string Cache => Path.Combine(new DirectoryInfo(Binary).Parent.FullName, "cache");
    }
}
