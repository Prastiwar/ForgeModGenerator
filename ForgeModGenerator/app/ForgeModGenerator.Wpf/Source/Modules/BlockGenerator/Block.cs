using Prism.Mvvm;
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

    public class Block : BindableBase
    {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private BlockType type;
        public BlockType Type {
            get => type;
            set => SetProperty(ref type, value);
        }

        private Image texture;
        public Image Texture {
            get => texture;
            set => SetProperty(ref texture, value);
        }

        private BlockAttributes attributes;
        public BlockAttributes Attributes {
            get => attributes;
            set => SetProperty(ref attributes, value);
        }
    }
}
