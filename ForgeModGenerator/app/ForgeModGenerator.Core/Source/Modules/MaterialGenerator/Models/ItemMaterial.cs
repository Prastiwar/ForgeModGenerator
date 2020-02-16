namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class ItemMaterial : Material
    {
        private int enchantability;
        public int Enchantability {
            get => enchantability;
            set => SetProperty(ref enchantability, value);
        }
    }
}
