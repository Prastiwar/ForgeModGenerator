using ForgeModGenerator.Core;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Model
{
    // struct for mcmod.info file
    public class McModInfo
    {
        [JsonProperty(Required = Required.Always)]
        public string Modid { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string McVersion { get; set; }
        public string Url { get; set; }
        public string UpdateUrl { get; set; }
        public string Credits { get; set; }
        public string LogoFile { get; set; }
        public string[] AuthorList { get; set; }
        public string[] Screenshots { get; set; }
        public string[] Dependencies { get; set; }

        public static McModInfo Import(string modPath)
        {
            string modname = new DirectoryInfo(modPath).Name;
            string modInfoFilePath = ModPaths.McModInfo(modname).Replace("\\", "/");
            string infoTextFormat = File.ReadAllText(modInfoFilePath);
            string fixedJson = infoTextFormat.Remove(0, 2).Remove(infoTextFormat.Length - 4, 2); // remove [\n and \n]
            return JsonConvert.DeserializeObject<McModInfo>(fixedJson);
        }
    }
}