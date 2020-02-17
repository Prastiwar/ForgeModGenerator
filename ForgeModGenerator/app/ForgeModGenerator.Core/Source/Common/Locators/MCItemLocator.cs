using ForgeModGenerator.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.Core
{
    public readonly struct MCItemLocator : IEquatable<MCItemLocator>
    {
        public MCItemLocator(string name, string imageFilePath)
        {
            Name = name;
            ImageFilePath = imageFilePath;
        }

        public string Name { get; }
        public string ImageFilePath { get; }

        public bool Equals(MCItemLocator other) => Name == other.Name;
        public override bool Equals(object obj) => obj is MCItemLocator item && Equals(item);

        public override int GetHashCode()
        {
            int hashCode = -1964435599;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ImageFilePath);
            return hashCode;
        }

        public static bool operator ==(MCItemLocator left, MCItemLocator right) => left.Name == right.Name;
        public static bool operator !=(MCItemLocator left, MCItemLocator right) => !(left == right);

        public static MCItemLocator[] GetAllPossibleItems(string modname, string modid) => GetAllModItems(modname, modid).Concat(GetAllMinecraftItems()).ToArray();

        public static MCItemLocator[] GetAllModItems(string modname, string modid)
        {
            List<MCItemLocator> locators = new List<MCItemLocator>(128);
            string defaultIconsPath = ModPaths.TexturesFolder(modname, modid);
            foreach (FileInfo item in IOHelper.EnumerateFileInfos(defaultIconsPath, "*.png"))
            {
                string locatorName = modid + ":" + Path.GetFileNameWithoutExtension(item.Name);
                MCItemLocator newLocator = new MCItemLocator(locatorName, Path.Combine(defaultIconsPath, item.Name));
                locators.Add(newLocator);
            }
            return locators.ToArray();
        }

        public static MCItemLocator[] GetAllMinecraftItems()
        {
            List<MCItemLocator> locators = new List<MCItemLocator>(128);
            string defaultIconsPath = AppPaths.GetMCItemIconsPath();
            foreach (FileInfo item in IOHelper.EnumerateFileInfos(defaultIconsPath, "*.png"))
            {
                string locatorName = "minecraft:" + Path.GetFileNameWithoutExtension(item.Name);
                MCItemLocator newLocator = new MCItemLocator(locatorName, Path.Combine(defaultIconsPath, item.Name));
                locators.Add(newLocator);
            }
            return locators.ToArray();
        }
    }
}
