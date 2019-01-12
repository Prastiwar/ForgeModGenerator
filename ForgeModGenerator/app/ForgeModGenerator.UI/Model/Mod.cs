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
        private string modid;
        public string Modid {
            get => modid;
            set => Set(ref modid, value);
        }

        private string organization;
        public string Organization {
            get => organization;
            set => Set(ref organization, value);
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
