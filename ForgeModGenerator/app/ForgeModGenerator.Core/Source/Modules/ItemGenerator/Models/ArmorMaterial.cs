namespace ForgeModGenerator.ItemGenerator.Models
{
    public class ArmorMaterial : Material
    {
        private int durability;
        public int Durability {
            get => durability;
            set => SetProperty(ref durability, value);
        }

        private int helmetDamageReduction;
        public int HelmetDamageReduction {
            get => helmetDamageReduction;
            set => SetProperty(ref helmetDamageReduction, value);
        }

        private int plateDamageReduction;
        public int PlateDamageReduction {
            get => plateDamageReduction;
            set => SetProperty(ref plateDamageReduction, value);
        }

        private int legsDamageReduction;
        public int LegsDamageReduction {
            get => legsDamageReduction;
            set => SetProperty(ref legsDamageReduction, value);
        }

        private int bootsDamageReduction;
        public int BootsDamageReduction {
            get => bootsDamageReduction;
            set => SetProperty(ref bootsDamageReduction, value);
        }

        private float toughness;
        public float Toughness {
            get => toughness;
            set => SetProperty(ref toughness, value);
        }

        private string textureName;
        public string TextureName {
            get => textureName;
            set => SetProperty(ref textureName, value);
        }

        private string soundEvent;
        public string SoundEvent {
            get => soundEvent;
            set => SetProperty(ref soundEvent, value);
        }

        public int[] DamageReductionAmountArray => new int[] { HelmetDamageReduction, PlateDamageReduction, LegsDamageReduction, BootsDamageReduction };
    }
}
