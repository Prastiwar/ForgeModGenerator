﻿using ForgeModGenerator.Converter;
using ForgeModGenerator.Core;
using ForgeModGenerator.Miscellaneous;
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

        // Writes to FmgModInfo file
        public static void Export(Mod mod)
        {
            File.WriteAllText(ModPaths.FmgModInfo(mod.ModInfo.Name), JsonConvert.SerializeObject(mod, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
        }

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