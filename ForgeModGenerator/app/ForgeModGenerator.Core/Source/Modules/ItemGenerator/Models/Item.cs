using ForgeModGenerator.Models;

namespace ForgeModGenerator.ItemGenerator.Models
{
    public class Item : ObservableDirtyObject, ICopiable
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ItemType type;
        public ItemType Type {
            get => type;
            set {
                SetProperty(ref type, value);
                if (type != ItemType.Armor && ArmorType != ArmorType.None)
                {
                    ArmorType = ArmorType.None;
                }
            }
        }

        private ArmorType armorType;
        public ArmorType ArmorType {
            get => armorType;
            set {
                SetProperty(ref armorType, value);
                if (armorType != ArmorType.None && Type != ItemType.Armor)
                {
                    Type = ItemType.Armor;
                }
            }
        }

        private string textureName;
        public string TextureName {
            get => textureName;
            set => SetProperty(ref textureName, value);
        }

        private int stackSize;
        public int StackSize {
            get => stackSize;
            set => SetProperty(ref stackSize, value);
        }

        private string material;
        public string Material {
            get => material;
            set => SetProperty(ref material, value);
        }
        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new Item() {
            Name = Name,
            Type = Type,
            ArmorType = ArmorType,
            TextureName = TextureName,
            StackSize = StackSize,
            Material = Material,
        };

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is Item item)
            {
                Name = item.Name;
                Type = item.Type;
                ArmorType = item.ArmorType;
                TextureName = item.TextureName;
                StackSize = item.StackSize;
                Material = item.Material;
                return true;
            }
            return false;
        }
    }
}
