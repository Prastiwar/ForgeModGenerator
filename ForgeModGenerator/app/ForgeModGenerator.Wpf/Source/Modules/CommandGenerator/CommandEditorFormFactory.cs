using ForgeModGenerator.CommandGenerator.Controls;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ForgeModGenerator.CommandGenerator
{
    public class CommandEditorFormFactory : IEditorFormFactory<Command>
    {
        public CommandEditorFormFactory(IMemoryCache cache, IDialogService dialogService)
        {
            this.cache = cache;
            this.dialogService = dialogService;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;

        public IEditorForm<Command> Create() => new EditorForm<Command>(cache, dialogService) { Form = new CommandEditForm() };
    }
}
