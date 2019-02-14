using ForgeModGenerator.Converters;
using ForgeModGenerator.ModGenerator.Validations;
using ForgeModGenerator.Utils;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ForgeModGenerator.ModGenerator.Models
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

        [JsonProperty(Required = Required.Always)]
        internal string CachedName { get; private set; }

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
            CachedName = ModInfo.Name;
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

        public static string GetModidFromPath(string path)
        {
            path = path.Replace("\\", "/");
            int index = !path.Contains(":/") ? path.IndexOf(':') : -1;
            if (index >= 1)
            {
                return path.Substring(0, index);
            }
            string modname = GetModnameFromPath(path);
            if (modname != null)
            {
                string assetsPath = ModPaths.Assets(modname).Replace("\\", "/"); // in assets folder there should be always folder with modid
                int assetsPathLength = assetsPath.Length;
                try
                {
                    string directory = Directory.EnumerateDirectories(assetsPath).First();
                    string dir = directory.Replace("\\", "/");
                    return dir.Remove(0, assetsPathLength + 1);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return null;
        }

        public static string GetModnameFromPath(string path)
        {
            if (!IOExtensions.IsPathValid(path))
            {
                return null;
            }
            path = path.NormalizePath();
            string modsPath = AppPaths.Mods.NormalizePath();
            int length = modsPath.Length;
            if (!path.StartsWith(modsPath))
            {
                return null;
            }
            try
            {
                string sub = path.Remove(0, length + 1);
                int index = sub.IndexOf("/");
                return index >= 1 ? sub.Substring(0, index) : null;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public ValidationResult IsValid()
        {
            ModRules rules = new ModRules();
            ValidationResult result = rules.ValidateName(ModInfo.Name);
            if (!result.IsValid)
            {
                return result;
            }
            result = rules.ValidateModid(ModInfo.Modid);
            if (!result.IsValid)
            {
                return result;
            }
            return rules.ValidateOrganization(Organization);
        }

        // Writes to FmgModInfo file
        public static void Export(Mod mod) => File.WriteAllText(ModPaths.FmgModInfo(mod.ModInfo.Name), JsonConvert.SerializeObject(mod, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));

        public static Mod Import(string modPath)
        {
            string fmgModInfoPath = ModPaths.FmgModInfo(new DirectoryInfo(modPath).Name);
            try
            {
                return JsonConvert.DeserializeObject<Mod>(File.ReadAllText(fmgModInfoPath), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, $"Failed to load: {fmgModInfoPath}", true);
            }
            return null;
        }
    }
}
