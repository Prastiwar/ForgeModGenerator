using GalaSoft.MvvmLight;
using System.Drawing;

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

    public class Item : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private ItemType type;
        public ItemType Type {
            get => type;
            set => Set(ref type, value);
        }

        private Image texture;
        public Image Texture {
            get => texture;
            set => Set(ref texture, value);
        }

        private ItemAttributes attributes;
        public ItemAttributes Attributes {
            get => attributes;
            set => Set(ref attributes, value);
        }
    }
}
