using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;

namespace ForgeModGenerator.BlockGenerator.Models
{
    public class Block : ObservableModel
    {
        private BlockType type;
        public BlockType Type {
            get => type;
            set => SetProperty(ref type, value);
        }

        private StringGetter materialType;
        public StringGetter MaterialType {
            get => materialType;
            set => SetProperty(ref materialType, value);
        }

        private string textureName;
        public string TextureName {
            get => textureName;
            set => SetProperty(ref textureName, value);
        }

        private string inventoryTexturePath;
        public string InventoryTextureName {
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

        private HarvestToolType harvestLevelTool;
        public HarvestToolType HarvestLevelTool {
            get => harvestLevelTool;
            set => SetProperty(ref harvestLevelTool, value);
        }

        private int harvestLevel;
        public int HarvestLevel {
            get => harvestLevel;
            set => SetProperty(ref harvestLevel, value);
        }

        private StringGetter soundType;
        public StringGetter SoundType {
            get => soundType;
            set => SetProperty(ref soundType, value);
        }

        private StringGetter dropItem;
        public StringGetter DropItem {
            get => dropItem;
            set => SetProperty(ref dropItem, value);
        }
    }
}
