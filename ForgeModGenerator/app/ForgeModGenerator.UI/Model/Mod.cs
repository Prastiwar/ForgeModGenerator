using ForgeModGenerator.Core;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.IO;

namespace ForgeModGenerator.Model
{
    public enum ModSide
    {
        ClientServer,
        Client,
        Server
    }

    public class Mod : ObservableObject
    {
        private string organization;
        [JsonProperty(Required = Required.Always)]
        public string Organization {
            get => organization;
            set => Set(ref organization, value);
        }

        private ModSide side;
        public ModSide Side {
            get => side;
            set => Set(ref side, value);
        }

        private McModInfo modInfo;
        [JsonProperty(Required = Required.Always)]
        public McModInfo ModInfo {
            get => modInfo;
            set => Set(ref modInfo, value);
        }

        public Mod(McModInfo modInfo, string organization, ModSide side = ModSide.ClientServer)
        {
            ModInfo = modInfo;
            Organization = organization;
            Side = side;
        }

        public static Mod Import(string modPath)
        {
            string fmgModInfoPath = ModPaths.FmgModInfo(new DirectoryInfo(modPath).Name);
            try
            {
                return JsonConvert.DeserializeObject<Mod>(File.ReadAllText(fmgModInfoPath));
            }
            catch (System.Exception)
            {
                Log.InfoBox($"Failed to load: {fmgModInfoPath}");
            }
            return null;
        }
    }
}
