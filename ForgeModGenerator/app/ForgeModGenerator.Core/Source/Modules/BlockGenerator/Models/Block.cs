using ForgeModGenerator.Models;
using ForgeModGenerator.Validation;
using System.ComponentModel;

namespace ForgeModGenerator.BlockGenerator.Models
{
    public class Block : ObservableDirtyObject, ICopiable, IDataErrorInfo, IValidable<Block>
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

        public virtual object Clone() => MemberwiseClone();
        public virtual object DeepClone() => new Block() {
            Name = Name,
            Type = Type,
            MaterialType = MaterialType,
            TextureName = TextureName,
            InventoryTextureName = InventoryTextureName,
            LightLevel = LightLevel,
            Hardness = Hardness,
            Resistance = Resistance,
            ShouldMakeCollision = ShouldMakeCollision,
            HarvestLevelTool = HarvestLevelTool,
            HarvestLevel = HarvestLevel,
            SoundType = SoundType,
            DropItem = DropItem
        };

        public virtual bool CopyValues(object fromCopy)
        {
            if (fromCopy is Block block)
            {
                Name = block.Name;
                Type = block.Type;
                MaterialType = block.MaterialType;
                TextureName = block.TextureName;
                InventoryTextureName = block.InventoryTextureName;
                LightLevel = block.LightLevel;
                Hardness = block.Hardness;
                Resistance = block.Resistance;
                ShouldMakeCollision = block.ShouldMakeCollision;
                HarvestLevelTool = block.HarvestLevelTool;
                HarvestLevel = block.HarvestLevel;
                SoundType = block.SoundType;
                DropItem = block.DropItem;
                return true;
            }
            return false;
        }

        public ValidateResult Validate()
        {
            string errorString = OnValidate(nameof(Name));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(Type));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(MaterialType));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(TextureName));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(InventoryTextureName));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(LightLevel));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(Hardness));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(Resistance));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(ShouldMakeCollision));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(HarvestLevelTool));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(HarvestLevel));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(SoundType));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            errorString = OnValidate(nameof(DropItem));
            if (!string.IsNullOrEmpty(errorString))
            {
                return new ValidateResult(false, errorString);
            }
            return ValidateResult.Valid;
        }

        public event PropertyValidationEventHandler<Block> ValidateProperty;
        string IDataErrorInfo.Error => null;
        string IDataErrorInfo.this[string propertyName] => OnValidate(propertyName);
        private string OnValidate(string propertyName) => ValidateHelper.OnValidateError(ValidateProperty, this, propertyName);
    }
}
