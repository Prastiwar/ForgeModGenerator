using Prism.Mvvm;

namespace ForgeModGenerator.CommandGenerator.Models
{
    public class Command : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string usage;
        public string Usage {
            get => usage;
            set => SetProperty(ref usage, value);
        }

        private string permission;
        public string Permission {
            get => permission;
            set => SetProperty(ref permission, value);
        }
    }
}
