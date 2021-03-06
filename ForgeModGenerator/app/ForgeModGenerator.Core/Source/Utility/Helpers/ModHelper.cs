﻿using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using System.IO;

namespace ForgeModGenerator
{
    public static class ModHelper
    {
        // Writes to FmgModInfo file
        public static void ExportMod(ISerializer<McMod> serializer, McMod mcMod) => File.WriteAllText(ModPaths.FmgModInfoFile(mcMod.ModInfo.Name), serializer.Serialize(mcMod));

        public static McMod ImportMod(ISerializer<McMod> serializer, string modPath)
        {
            string fmgModInfoPath = ModPaths.FmgModInfoFile(new DirectoryInfo(modPath).Name);
            try
            {
                return serializer.Deserialize(File.ReadAllText(fmgModInfoPath));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, $"Failed to load: {fmgModInfoPath}");
            }
            return null;
        }

        public static McModInfo ImportMcInfo(ISerializer<McModInfo> serializer, string modPath)
        {
            string modname = new DirectoryInfo(modPath).Name;
            string modInfoFilePath = ModPaths.McModInfoFile(modname).Replace("\\", "/");
            string infoTextFormat = File.ReadAllText(modInfoFilePath);
            return serializer.Deserialize(infoTextFormat);
        }

        public static void ExportMcInfo(ISerializer<McModInfo> serializer, McModInfo modInfo) => File.WriteAllText(ModPaths.McModInfoFile(modInfo.Name), serializer.Serialize(modInfo));
    }
}
