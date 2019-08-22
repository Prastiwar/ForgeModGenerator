using Prism.Mvvm;

namespace ForgeModGenerator.ItemGenerator.Models
{
    public class Material : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private int enchantability;
        public int Enchantability {
            get => enchantability;
            set => SetProperty(ref enchantability, value);
        }
    }
}
