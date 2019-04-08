using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.Models
{
    /// <summary> mcmod.info file representation </summary>
    public class McModInfo : BindableBase, ICopiable<McModInfo>
    {
        private string modid;
        public string Modid {
            get => modid;
            set => SetProperty(ref modid, value);
        }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string description;
        public string Description {
            get => description;
            set => SetProperty(ref description, value);
        }

        private string version;
        public string Version {
            get => version;
            set => SetProperty(ref version, value);
        }

        private string mcVersion;
        public string McVersion {
            get => mcVersion;
            set => SetProperty(ref mcVersion, value);
        }

        private string url;
        public string Url {
            get => url;
            set => SetProperty(ref url, value);
        }

        private string updateUrl;
        public string UpdateUrl {
            get => updateUrl;
            set => SetProperty(ref updateUrl, value);
        }

        private string credits;
        public string Credits {
            get => credits;
            set => SetProperty(ref credits, value);
        }

        private string logoFile;
        public string LogoFile {
            get => logoFile;
            set => SetProperty(ref logoFile, value);
        }

        private ObservableCollection<string> authorList;
        public ObservableCollection<string> AuthorList {
            get => authorList;
            set => SetProperty(ref authorList, value);
        }

        private ObservableCollection<string> screenshots;
        public ObservableCollection<string> Screenshots {
            get => screenshots;
            set => SetProperty(ref screenshots, value);
        }

        private ObservableCollection<string> dependencies;
        public ObservableCollection<string> Dependencies {
            get => dependencies;
            set => SetProperty(ref dependencies, value);
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
