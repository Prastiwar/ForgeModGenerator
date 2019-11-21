using ForgeModGenerator.BlockGenerator.Controls;
using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;

namespace ForgeModGenerator.BlockGenerator
{

    public class BlockModelFormProvider : ModelFormProvider<Block>
    {
        public BlockModelFormProvider(SoundTypeChooseCollection soundTypes,
                                       ItemChooseCollection dropItems,
                                       BlockMaterialChooseCollection materialTypes)
        {
            this.soundTypes = soundTypes;
            this.dropItems = dropItems;
            this.materialTypes = materialTypes;
        }

        private readonly SoundTypeChooseCollection soundTypes;
        private readonly ItemChooseCollection dropItems;
        private readonly BlockMaterialChooseCollection materialTypes;

        public override IUIElement GetUIElement() => new BlockEditForm() {
            SoundTypes = soundTypes,
            DropItems = dropItems,
            MaterialTypes = materialTypes
        };
    }
}
