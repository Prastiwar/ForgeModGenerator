using ForgeModGenerator.BlockGenerator.Controls;
using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator.BlockGenerator
{
    public class BlockEditorFormFactory : IEditorFormFactory<Block>
    {
        public BlockEditorFormFactory(IMemoryCache cache,
                                      IDialogService dialogService,
                                      SoundTypeChooseCollection soundTypes,
                                      ItemChooseCollection dropItems,
                                      BlockMaterialChooseCollection materialTypes)
        {
            this.cache = cache;
            this.dialogService = dialogService;
            this.soundTypes = soundTypes;
            this.dropItems = dropItems;
            this.materialTypes = materialTypes;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;
        private readonly SoundTypeChooseCollection soundTypes;
        private readonly ItemChooseCollection dropItems;
        private readonly BlockMaterialChooseCollection materialTypes;

        public IEditorForm<Block> Create() => new EditorForm<Block>(cache, dialogService) {
            Form = new BlockEditForm() {
                SoundTypes = soundTypes,
                DropItems = dropItems,
                MaterialTypes = materialTypes
            }
        };
    }
}
