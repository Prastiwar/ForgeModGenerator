namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class BlockMaterial : Material
    {
        private bool isLiquid;
        public bool IsLiquid {
            get => isLiquid;
            set => SetProperty(ref isLiquid, value);
        }

        private bool isSolid;
        public bool IsSolid {
            get => isSolid;
            set => SetProperty(ref isSolid, value);
        }

        private bool blocksLight;
        public bool BlocksLight {
            get => blocksLight;
            set => SetProperty(ref blocksLight, value);
        }

        private bool blocksMovement;
        public bool BlocksMovement {
            get => blocksMovement;
            set => SetProperty(ref blocksMovement, value);
        }

        private bool isTranslucent;
        public bool IsTranslucent {
            get => isTranslucent;
            set => SetProperty(ref isTranslucent, value);
        }

        private bool requiresNoTool;
        public bool RequiresNoTool {
            get => requiresNoTool;
            set => SetProperty(ref requiresNoTool, value);
        }

        private bool canBurn;
        public bool CanBurn {
            get => canBurn;
            set => SetProperty(ref canBurn, value);
        }

        private bool isReplaceable;
        public bool IsReplaceable {
            get => isReplaceable;
            set => SetProperty(ref isReplaceable, value);
        }

        private bool isAdventureModeExempt;
        public bool IsAdventureModeExempt {
            get => isAdventureModeExempt;
            set => SetProperty(ref isAdventureModeExempt, value);
        }

        private PushReaction mobilityFlag;
        public PushReaction MobilityFlag {
            get => mobilityFlag;
            set => SetProperty(ref mobilityFlag, value);
        }

        public override object DeepClone() => new BlockMaterial() {
            Name = Name,
            IsLiquid = IsLiquid,
            IsSolid = IsSolid,
            BlocksLight = BlocksLight,
            BlocksMovement = BlocksMovement,
            IsTranslucent = IsTranslucent,
            RequiresNoTool = RequiresNoTool,
            CanBurn = CanBurn,
            IsReplaceable = IsReplaceable,
            IsAdventureModeExempt = IsAdventureModeExempt,
            MobilityFlag = MobilityFlag
        };

        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is BlockMaterial material)
            {
                base.CopyValues(fromCopy);
                IsLiquid = material.IsLiquid;
                IsSolid = material.IsSolid;
                BlocksLight = material.BlocksLight;
                BlocksMovement = material.BlocksMovement;
                IsTranslucent = material.IsTranslucent;
                RequiresNoTool = material.RequiresNoTool;
                CanBurn = material.CanBurn;
                IsReplaceable = material.IsReplaceable;
                IsAdventureModeExempt = material.IsAdventureModeExempt;
                MobilityFlag = material.MobilityFlag;
                return true;
            }
            return false;
        }
    }
}
