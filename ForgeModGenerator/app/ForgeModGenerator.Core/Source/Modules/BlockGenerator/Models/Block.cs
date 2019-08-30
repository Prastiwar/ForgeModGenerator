using ForgeModGenerator.ItemGenerator.Models;
using Prism.Mvvm;
using System.Collections.Generic;

namespace ForgeModGenerator.BlockGenerator.Models
{
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

        private MaterialType materialType;
        public MaterialType MaterialType {
            get => materialType;
            set => SetProperty(ref materialType, value);
        }

        private string texturePath;
        public string TexturePath {
            get => texturePath;
            set => SetProperty(ref texturePath, value);
        }

        private string inventoryTexturePath;
        public string InventoryTexturePath {
            get => inventoryTexturePath;
            set => SetProperty(ref inventoryTexturePath, value);
        }

        private int lightLevel;
        public int LightLevel {
            get => lightLevel;
            set => SetProperty(ref lightLevel, Math.Clamp(value, 0, 15));
        }

        private float hardness;
        public float Hardness {
            get => hardness;
            set => SetProperty(ref hardness, value);
        }

        private float resistance;
        public float Resistance {
            get => resistance;
            set => SetProperty(ref resistance, value);
        }

        private bool shouldMakeCollision;
        public bool ShouldMakeCollision {
            get => shouldMakeCollision;
            set => SetProperty(ref shouldMakeCollision, value);
        }

        private KeyValuePair<string, int> harvestLevel;
        public KeyValuePair<string, int> HarvestLevel {
            get => harvestLevel;
            set => SetProperty(ref harvestLevel, value);
        }

        private SoundType soundType;
        public SoundType SoundType {
            get => soundType;
            set => SetProperty(ref soundType, value);
        }

        private string dropItem;
        public string DropItem {
            get => dropItem;
            set => SetProperty(ref dropItem, value);
        }
    }
}
