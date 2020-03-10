using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator
{
    public class ModelEditorFormFactory<TModel> : IEditorFormFactory<TModel> where TModel : ICopiable, IDirty
    {
        public ModelEditorFormFactory(IMemoryCache cache, IDialogService dialogService, ModelFormProvider<TModel> uiProvider)
        {
            this.cache = cache;
            this.dialogService = dialogService;
            this.uiProvider = uiProvider;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;
        private readonly ModelFormProvider<TModel> uiProvider;

        public IEditorForm<TModel> Create() => new EditorForm<TModel>(cache, dialogService, uiProvider);
    }
}
