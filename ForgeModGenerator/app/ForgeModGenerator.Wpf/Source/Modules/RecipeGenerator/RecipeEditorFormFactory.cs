using ForgeModGenerator.RecipeGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeEditorFormFactory : IEditorFormFactory<Recipe>
    {
        public RecipeEditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<Recipe> Create() => new EditorForm<Recipe>(cache, dialogService) { Form = new RecipeEditForm() };
    }
}
