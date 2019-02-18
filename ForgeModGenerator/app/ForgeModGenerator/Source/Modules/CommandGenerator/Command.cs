using GalaSoft.MvvmLight;

namespace ForgeModGenerator.CommandGenerator.Models
{
    public class Command : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private string usage;
        public string Usage {
            get => usage;
            set => Set(ref usage, value);
        }

        private string permission;
        public string Permission {
            get => permission;
            set => Set(ref permission, value);
        }
    }
}
