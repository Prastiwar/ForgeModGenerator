namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class ToolMaterial : ItemMaterial
    {
        private int harvestLevel;
        public int HarvestLevel {
            get => harvestLevel;
            set => SetProperty(ref harvestLevel, value);
        }

        private int maxUses;
        public int MaxUses {
            get => maxUses;
            set => SetProperty(ref maxUses, value);
        }

        private float efficiency;
        public float Efficiency {
            get => efficiency;
            set => SetProperty(ref efficiency, value);
        }

        private float attackDamage;
        public float AttackDamage {
            get => attackDamage;
            set => SetProperty(ref attackDamage, value);
        }

        public override object DeepClone() => new ToolMaterial() {
            Name = Name,
            Enchantability = Enchantability,
            HarvestLevel = HarvestLevel,
            MaxUses = MaxUses,
            Efficiency = Efficiency,
            AttackDamage = AttackDamage,
        };

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is ToolMaterial material)
            {
                base.CopyValues(fromCopy);
                HarvestLevel = material.HarvestLevel;
                MaxUses = material.MaxUses;
                Efficiency = material.Efficiency;
                AttackDamage = material.AttackDamage;
                return true;
            }
            return false;
        }
    }
}
