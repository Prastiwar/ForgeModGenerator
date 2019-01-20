using ForgeModGenerator.Converter;
using ForgeModGenerator.Core;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace ForgeModGenerator.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
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

        private WorkspaceSetup workspaceSetup;
        public WorkspaceSetup WorkspaceSetup {
            get => workspaceSetup;
            set => Set(ref workspaceSetup, value);
        }

        private McModInfo modInfo;
        [JsonProperty(Required = Required.Always)]
        public McModInfo ModInfo {
            get => modInfo;
            set => Set(ref modInfo, value);
        }

        private ForgeVersion forgeVersion;
        [JsonProperty(Required = Required.Always)]
        public ForgeVersion ForgeVersion {
            get => forgeVersion;
            set => Set(ref forgeVersion, value);
        }

        private LaunchSetup launchSetup;
        [JsonProperty(Required = Required.Always)]
        public LaunchSetup LaunchSetup {
            get => launchSetup;
            set => Set(ref launchSetup, value);
        }

        public Mod(McModInfo modInfo, string organization, ForgeVersion forgeVersion)
        {
            ModInfo = modInfo;
            Organization = organization;
            ForgeVersion = forgeVersion;
            Side = side;
            LaunchSetup = launchSetup ?? (
                Side == ModSide.Client
                    ? new LaunchSetup()
                    : Side == ModSide.Server
                        ? new LaunchSetup(false, true)
                        : new LaunchSetup(true, true)
            );
            WorkspaceSetup = workspaceSetup ?? WorkspaceSetup.NONE;
        }

        public Mod SetSide(ModSide side)
        {
            Side = side;
            return this;
        }

        public Mod SetLaunchSetup(LaunchSetup setup)
        {
            LaunchSetup = setup;
            return this;
        }

        public Mod SetWorkspaceSetup(WorkspaceSetup setup)
        {
            WorkspaceSetup = setup;
            return this;
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
