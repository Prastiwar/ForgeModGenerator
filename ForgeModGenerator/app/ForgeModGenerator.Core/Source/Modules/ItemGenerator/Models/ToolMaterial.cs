﻿namespace ForgeModGenerator.ItemGenerator.Models
{
    public class ToolMaterial : Material
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
    }
}
