namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class ItemMaterial : Material
    {
        private int enchantability;
        public int Enchantability {
            get => enchantability;
            set => SetProperty(ref enchantability, value);
        }

        public override object DeepClone()
        {
            ItemMaterial material = new ItemMaterial();
            material.CopyValues(this);
            return material;
        }

        public override bool CopyValues(object fromCopy)
        {
            base.CopyValues(fromCopy);
            if (fromCopy is ItemMaterial material)
            {
                Enchantability = material.Enchantability;
                return true;
            }
            return false;
        }
    }
}
