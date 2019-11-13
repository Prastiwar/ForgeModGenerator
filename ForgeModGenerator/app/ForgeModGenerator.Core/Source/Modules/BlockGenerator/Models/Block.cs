using ForgeModGenerator.Models;

namespace ForgeModGenerator.BlockGenerator.Models
{
    public class Block : ObservableModel
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

        private string harvestLevelTool;
        public string HarvestLevelTool {
            get => harvestLevelTool;
            set => SetProperty(ref harvestLevelTool, value);
        }

        private int harvestLevel;
        public int HarvestLevel {
            get => harvestLevel;
            set => SetProperty(ref harvestLevel, value);
        }

        private string soundType;
        public string SoundType {
            get => soundType;
            set => SetProperty(ref soundType, value);
        }

        private string dropItem;
        public string DropItem {
            get => dropItem;
            set => SetProperty(ref dropItem, value);
        }

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Block fromModel)
            {
                Name = fromModel.Name;
                Type = fromModel.Type;
                MaterialType = fromModel.MaterialType;
                TextureName = fromModel.TextureName;
                InventoryTextureName = fromModel.InventoryTextureName;
                LightLevel = fromModel.LightLevel;
                Hardness = fromModel.Hardness;
                Resistance = fromModel.Resistance;
                ShouldMakeCollision = fromModel.ShouldMakeCollision;
                HarvestLevelTool = fromModel.HarvestLevelTool;
                HarvestLevel = fromModel.HarvestLevel;
                SoundType = fromModel.SoundType;
                DropItem = fromModel.DropItem;
                return true;
            }
            return false;
        }

        public override object DeepClone()
        {
            Block block = new Block();
            block.CopyValues(this);
            return block;
        }
    }
}
