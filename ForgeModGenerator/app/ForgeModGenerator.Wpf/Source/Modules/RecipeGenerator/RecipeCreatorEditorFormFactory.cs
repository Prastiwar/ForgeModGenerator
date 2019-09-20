using ForgeModGenerator.RecipeGenerator.Controls;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace ForgeModGenerator.RecipeGenerator
{
    public class RecipeCreatorEditorFormFactory : IEditorFormFactory<RecipeCreator>
    {
        public RecipeCreatorEditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<RecipeCreator> Create() => new EditorForm<RecipeCreator>(cache, dialogService) {
            Form = new RecipeEditForm() {
                RecipeTypes = new Type[] { typeof(ShapedRecipe), typeof(ShapelessRecipe), typeof(SmeltingRecipe) }
            }
        };
    }
}
