using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator
{
    public class EditorFormFactory<TItem> : IEditorFormFactory<TItem> where TItem : ICopiable, IDirty
    {
        public EditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<TItem> Create() => new EditorForm<TItem>(cache, dialogService);
    }
}
