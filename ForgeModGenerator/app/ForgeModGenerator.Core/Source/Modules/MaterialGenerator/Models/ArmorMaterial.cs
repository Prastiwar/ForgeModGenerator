using ForgeModGenerator.CodeGeneration;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class ArmorMaterial : ItemMaterial
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

        private StringGetter soundEvent;
        public StringGetter SoundEvent {
            get => soundEvent;
            set => SetProperty(ref soundEvent, value);
        }

        private readonly int[] damageReductionAmountArray = new int[4];
        public int[] DamageReductionAmountArray {
            get {
                damageReductionAmountArray[0] = HelmetDamageReduction;
                damageReductionAmountArray[1] = PlateDamageReduction;
                damageReductionAmountArray[2] = LegsDamageReduction;
                damageReductionAmountArray[3] = BootsDamageReduction;
                return damageReductionAmountArray;
            }
        }

        public override object DeepClone()
        {
            ArmorMaterial material = new ArmorMaterial();
            material.CopyValues(this);
            return material;
        }

        public override bool CopyValues(object fromCopy)
        {
            base.CopyValues(fromCopy);
            if (fromCopy is ArmorMaterial material)
            {
                Durability = material.Durability;
                HelmetDamageReduction = material.HelmetDamageReduction;
                PlateDamageReduction = material.PlateDamageReduction;
                LegsDamageReduction = material.LegsDamageReduction;
                BootsDamageReduction = material.BootsDamageReduction;
                Toughness = material.Toughness;
                TextureName = material.TextureName;
                SoundEvent = material.SoundEvent;
                return true;
            }
            return false;
        }
    }
}
