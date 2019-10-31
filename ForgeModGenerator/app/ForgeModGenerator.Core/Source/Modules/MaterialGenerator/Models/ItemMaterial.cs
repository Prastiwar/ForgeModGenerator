namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class ItemMaterial : Material
    {
        private int enchantability;
        public int Enchantability {
            get => enchantability;
            set => SetProperty(ref enchantability, value);
        }

        public override object DeepClone() => new ItemMaterial() {
            Name = Name,
            Enchantability = Enchantability
        };

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ItemMaterial material)
            {
                base.CopyValues(fromCopy);
                Enchantability = material.Enchantability;
                return true;
            }
            return false;
        }
    }
}
