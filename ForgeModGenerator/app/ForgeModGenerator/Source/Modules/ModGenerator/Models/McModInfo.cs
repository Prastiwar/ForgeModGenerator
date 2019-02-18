using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.ModGenerator.Models
{
    // struct for mcmod.info file
    public class McModInfo : ObservableObject
    {
        private string modid;
        [JsonProperty(Required = Required.Always, PropertyName = "modid")]
        public string Modid {
            get => modid;
            set => Set(ref modid, value);
        }

        private string name;
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private string description;
        [JsonProperty(PropertyName = "description")]
        public string Description {
            get => description;
            set => Set(ref description, value);
        }

        private string version;
        [JsonProperty(PropertyName = "version")]
        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        private string mcVersion;
        [JsonProperty(PropertyName = "mcversion")]
        public string McVersion {
            get => mcVersion;
            set => Set(ref mcVersion, value);
        }

        private string url;
        [JsonProperty(PropertyName = "url")]
        public string Url {
            get => url;
            set => Set(ref url, value);
        }

        private string updateUrl;
        [JsonProperty(PropertyName = "updateUrl")]
        public string UpdateUrl {
            get => updateUrl;
            set => Set(ref updateUrl, value);
        }

        private string credits;
        [JsonProperty(PropertyName = "credits")]
        public string Credits {
            get => credits;
            set => Set(ref credits, value);
        }

        private string logoFile;
        [JsonProperty(PropertyName = "logoFile")]
        public string LogoFile {
            get => logoFile;
            set => Set(ref logoFile, value);
        }

        private ObservableCollection<string> authorList;
        [JsonProperty(PropertyName = "authorList")]
        public ObservableCollection<string> AuthorList {
            get => authorList;
            set => Set(ref authorList, value);
        }

        private ObservableCollection<string> screenshots;
        [JsonProperty(PropertyName = "screenshots")]
        public ObservableCollection<string> Screenshots {
            get => screenshots;
            set => Set(ref screenshots, value);
        }

        private ObservableCollection<string> dependencies;
        [JsonProperty(PropertyName = "dependencies")]
        public ObservableCollection<string> Dependencies {
            get => dependencies;
            set => Set(ref dependencies, value);
        }

        public static McModInfo Import(string modPath)
        {
            string modname = new DirectoryInfo(modPath).Name;
            string modInfoFilePath = ModPaths.McModInfoFile(modname).Replace("\\", "/");
            string infoTextFormat = File.ReadAllText(modInfoFilePath);
            string fixedJson = infoTextFormat.Remove(0, 2).Remove(infoTextFormat.Length - 4, 2); // remove [\n and \n]
            return JsonConvert.DeserializeObject<McModInfo>(fixedJson);
        }

        public static void Export(McModInfo modInfo)
        {
            string modInfoPath = ModPaths.McModInfoFile(modInfo.Name);
            string serializedModInfo = JsonConvert.SerializeObject(modInfo, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(modInfoPath))
            {
                writer.Write("[\n");
                writer.Write(serializedModInfo);
                writer.Write("\n]");
            }
        }
    }
}
