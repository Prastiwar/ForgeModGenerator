using Prism.Mvvm;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class Material : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }
    }
}
