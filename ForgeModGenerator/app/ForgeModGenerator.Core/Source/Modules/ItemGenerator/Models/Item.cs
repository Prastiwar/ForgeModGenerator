using Prism.Mvvm;

namespace ForgeModGenerator.ItemGenerator.Models
{
    public class Item : BindableBase
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

        private string texturePath;
        public string TexturePath {
            get => texturePath;
            set => SetProperty(ref texturePath, value);
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
    }
}
