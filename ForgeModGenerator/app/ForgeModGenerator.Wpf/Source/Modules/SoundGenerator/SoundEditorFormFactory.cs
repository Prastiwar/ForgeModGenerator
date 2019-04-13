using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.Controls;
using ForgeModGenerator.SoundGenerator.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator.SoundGenerator
{
    public class SoundEditorFormFactory : IEditorFormFactory<Sound>
    {
        public SoundEditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<Sound> Create() => new EditorForm<Sound>(cache, dialogService) { Form = new SoundEditForm() };
    }
}
