using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using ForgeModGenerator.Utility;
using Microsoft.Extensions.Caching.Memory;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ForgeModGenerator.ModGenerator
{
    public class ModEditorFormFactory : IEditorFormFactory<Mod>
    {
        public ModEditorFormFactory(IMemoryCache cache, IDialogService dialogService, ISessionContextService sessionContext)
        {
            this.cache = cache;
            this.dialogService = dialogService;
            this.sessionContext = sessionContext;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;
        private readonly ISessionContextService sessionContext;

        public IEditorForm<Mod> Create() => new EditorForm<Mod>(cache, dialogService) {
            Form = new Controls.ModForm() {
                AddForgeVersionCommand = new DelegateCommand(sessionContext.DownloadNewForgeVersion),
                Setups = new ObservableCollection<WorkspaceSetup>(ReflectionHelper.EnumerateSubclasses<WorkspaceSetup>()),
                ForgeVersions = sessionContext.ForgeVersions,
                Sides = Enum.GetValues(typeof(ModSide)).Cast<ModSide>()
            }
        };
    }
}
