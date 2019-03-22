using ForgeModGenerator.Converters;
using ForgeModGenerator.Models;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator
{
    public static class ModHelper
    {
        // Writes to FmgModInfo file
        public static void ExportMod(Mod mod)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            File.WriteAllText(ModPaths.FmgModInfoFile(mod.ModInfo.Name), JsonConvert.SerializeObject(mod, settings));
        }

        public static Mod ImportMod(string modPath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            settings.Converters.Add(new ModJsonConverter());
            settings.Converters.Add(new McModInfoJsonConverter());
            settings.Converters.Add(new ForgeVersionJsonConverter());
            string fmgModInfoPath = ModPaths.FmgModInfoFile(new DirectoryInfo(modPath).Name);
            try
            {
                return JsonConvert.DeserializeObject<Mod>(File.ReadAllText(fmgModInfoPath), settings);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, $"Failed to load: {fmgModInfoPath}");
            }
            return null;
        }

        public static McModInfo ImportMcInfo(string modPath)
        {
            McModInfoJsonConverter c = new McModInfoJsonConverter();
            string modname = new DirectoryInfo(modPath).Name;
            string modInfoFilePath = ModPaths.McModInfoFile(modname).Replace("\\", "/");
            string infoTextFormat = File.ReadAllText(modInfoFilePath);
            string fixedJson = infoTextFormat.Remove(0, 2).Remove(infoTextFormat.Length - 4, 2); // remove [\n and \n]
            return JsonConvert.DeserializeObject<McModInfo>(fixedJson, c);
        }

        public static void ExportMcInfo(McModInfo modInfo)
        {
            string modInfoPath = ModPaths.McModInfoFile(modInfo.Name);
            string serializedModInfo = JsonConvert.SerializeObject(modInfo, Formatting.Indented, new McModInfoJsonConverter());
            using (StreamWriter writer = new StreamWriter(modInfoPath))
            {
                writer.Write("[\n");
                writer.Write(serializedModInfo);
                writer.Write("\n]");
            }
        }
    }
}
