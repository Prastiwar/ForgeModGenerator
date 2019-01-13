using GalaSoft.MvvmLight;

namespace ForgeModGenerator.Model
{
    public enum ModSide
    {
        Client,
        Server,
        Both
    }

    public class Mod : ObservableObject
    {
        private string organization;
        public string Organization {
            get => organization;
            set => Set(ref organization, value);
        }

        private string fullPath;
        public string FullPath {
            get => fullPath;
            set => Set(ref fullPath, value);
        }

        private ModSide side;
        public ModSide Side {
            get => side;
            set => Set(ref side, value);
        }

        private string modid;
        public string Modid {
            get => modid;
            set => Set(ref modid, value);
        }

        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private string version;
        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        private string mcVersion;
        public string McVersion {
            get => mcVersion;
            set => Set(ref mcVersion, value);
        }

        private string url;
        public string Url {
            get => url;
            set => Set(ref url, value);
        }

        private string updateUrl;
        public string UpdateUrl {
            get => updateUrl;
            set => Set(ref updateUrl, value);
        }

        private string credits;
        public string Credits {
            get => credits;
            set => Set(ref credits, value);
        }

        private string logoFile;
        public string LogoFile {
            get => logoFile;
            set => Set(ref logoFile, value);
        }

        private string[] authorList;
        public string[] AuthorList {
            get => authorList;
            set => Set(ref authorList, value);
        }

        private string[] screenshots;
        public string[] Screenshots {
            get => screenshots;
            set => Set(ref screenshots, value);
        }

        private string[] dependencies;
        public string[] Dependencies {
            get => dependencies;
            set => Set(ref dependencies, value);
        }

        public Mod(string version, string modid, string organization, string name = null, ModSide side = ModSide.Both)
        {
            Version = version;
            Modid = modid;
            Organization = organization;
            Name = name ?? modid;
            Side = side;
        }
    }
}
