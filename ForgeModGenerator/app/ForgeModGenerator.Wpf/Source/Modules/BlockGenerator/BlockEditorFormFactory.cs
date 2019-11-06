using ForgeModGenerator.BlockGenerator.Controls;
using ForgeModGenerator.BlockGenerator.Models;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator.BlockGenerator
{
    public class BlockEditorFormFactory : IEditorFormFactory<Block>
    {
        public BlockEditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<Block> Create() => new EditorForm<Block>(cache, dialogService) {
            Form = new BlockEditForm()
        };
    }
}
