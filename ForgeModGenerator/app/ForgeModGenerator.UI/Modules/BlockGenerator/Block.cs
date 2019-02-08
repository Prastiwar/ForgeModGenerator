using GalaSoft.MvvmLight;
using System.Drawing;

namespace ForgeModGenerator.BlockGenerator.Models
{
    public enum BlockType
    {
        Hard,
        Ore,
        Falling
    }

    public struct BlockAttributes
    {
        public float HarvestLevel;
        public int LightLevel; // 0-15
    }

    public class Block : ObservableObject
    {
        private string name;
        public string Name {
            get => name;
            set => Set(ref name, value);
        }

        private BlockType type;
        public BlockType Type {
            get => type;
            set => Set(ref type, value);
        }

        private Image texture;
        public Image Texture {
            get => texture;
            set => Set(ref texture, value);
        }

        private BlockAttributes attributes;
        public BlockAttributes Attributes {
            get => attributes;
            set => Set(ref attributes, value);
        }
    }
}
