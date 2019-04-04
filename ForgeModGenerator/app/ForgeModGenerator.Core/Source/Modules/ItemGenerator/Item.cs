using Prism.Mvvm;

namespace ForgeModGenerator.ItemGenerator.Models
{
    public enum ItemType
    {
        Hand,
        Hoe,
        Axe,
        Sword,
        Spade,
        Pickaxe
    }

    public struct ItemAttributes
    {
        public float Damage;
    }

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
            set => SetProperty(ref type, value);
        }

        private string texturePath;
        public string TexturePath {
            get => texturePath;
            set => SetProperty(ref texturePath, value);
        }

        private ItemAttributes attributes;
        public ItemAttributes Attributes {
            get => attributes;
            set => SetProperty(ref attributes, value);
        }
    }
}
