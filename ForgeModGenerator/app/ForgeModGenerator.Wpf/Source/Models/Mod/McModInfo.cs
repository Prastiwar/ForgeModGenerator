﻿using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace ForgeModGenerator.Models
{
    // struct for mcmod.info file
    public class McModInfo : BindableBase, ICopiable<McModInfo>
    {
        private string modid;
        [JsonProperty(Required = Required.Always, PropertyName = "modid")]
        public string Modid {
            get => modid;
            set => SetProperty(ref modid, value);
        }

        private string name;
        [JsonProperty(Required = Required.Always, PropertyName = "name")]
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string description;
        [JsonProperty(PropertyName = "description")]
        public string Description {
            get => description;
            set => SetProperty(ref description, value);
        }

        private string version;
        [JsonProperty(PropertyName = "version")]
        public string Version {
            get => version;
            set => SetProperty(ref version, value);
        }

        private string mcVersion;
        [JsonProperty(PropertyName = "mcversion")]
        public string McVersion {
            get => mcVersion;
            set => SetProperty(ref mcVersion, value);
        }

        private string url;
        [JsonProperty(PropertyName = "url")]
        public string Url {
            get => url;
            set => SetProperty(ref url, value);
        }

        private string updateUrl;
        [JsonProperty(PropertyName = "updateUrl")]
        public string UpdateUrl {
            get => updateUrl;
            set => SetProperty(ref updateUrl, value);
        }

        private string credits;
        [JsonProperty(PropertyName = "credits")]
        public string Credits {
            get => credits;
            set => SetProperty(ref credits, value);
        }

        private string logoFile;
        [JsonProperty(PropertyName = "logoFile")]
        public string LogoFile {
            get => logoFile;
            set => SetProperty(ref logoFile, value);
        }

        private ObservableCollection<string> authorList;
        [JsonProperty(PropertyName = "authorList")]
        public ObservableCollection<string> AuthorList {
            get => authorList;
            set => SetProperty(ref authorList, value);
        }

        private ObservableCollection<string> screenshots;
        [JsonProperty(PropertyName = "screenshots")]
        public ObservableCollection<string> Screenshots {
            get => screenshots;
            set => SetProperty(ref screenshots, value);
        }

        private ObservableCollection<string> dependencies;
        [JsonProperty(PropertyName = "dependencies")]
        public ObservableCollection<string> Dependencies {
            get => dependencies;
            set => SetProperty(ref dependencies, value);
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

        public bool CopyValues(McModInfo fromCopy)
        {
            AuthorList = fromCopy.AuthorList;
            Credits = fromCopy.Credits;
            Dependencies = fromCopy.Dependencies;
            Description = fromCopy.Description;
            LogoFile = fromCopy.LogoFile;
            McVersion = fromCopy.McVersion;
            Modid = fromCopy.Modid;
            Name = fromCopy.Name;
            Screenshots = fromCopy.Screenshots;
            UpdateUrl = fromCopy.UpdateUrl;
            Url = fromCopy.Url;
            Version = fromCopy.Version;
            return true;
        }

        public McModInfo DeepCopy()
        {
            McModInfo clone = new McModInfo() {
                AuthorList = AuthorList,
                Credits = Credits,
                Dependencies = Dependencies,
                Description = Description,
                LogoFile = LogoFile,
                McVersion = McVersion,
                Modid = Modid,
                Name = Name,
                Screenshots = Screenshots,
                UpdateUrl = UpdateUrl,
                Url = Url,
                Version = Version
            };
            return clone;
        }

        public McModInfo ShallowCopy() => (McModInfo)((ICloneable)this).Clone();

        bool ICopiable.CopyValues(object fromCopy) => fromCopy is McModInfo info ? CopyValues(info) : false;
        object ICopiable.DeepClone() => DeepCopy();
        object ICloneable.Clone() => MemberwiseClone();
    }
}