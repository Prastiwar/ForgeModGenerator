﻿using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using Microsoft.Extensions.Caching.Memory;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ForgeModGenerator.ModGenerator
{
    public class McModEditorFormFactory : IEditorFormFactory<McMod>
    {
        public McModEditorFormFactory(IMemoryCache cache, IDialogService dialogService, ISessionContextService sessionContext, ObservableCollection<WorkspaceSetup> setups)
        {
            this.cache = cache;
            this.dialogService = dialogService;
            this.sessionContext = sessionContext;
            this.setups = setups;
        }

        private readonly IMemoryCache cache;
        private readonly IDialogService dialogService;
        private readonly ISessionContextService sessionContext;
        private readonly ObservableCollection<WorkspaceSetup> setups;

        public IEditorForm<McMod> Create() => new EditorForm<McMod>(cache, dialogService) {
            Form = new Controls.ModForm() {
                AddForgeVersionCommand = new DelegateCommand(sessionContext.DownloadNewForgeVersion),
                Setups = setups,
                ForgeVersions = sessionContext.ForgeVersions,
                Sides = Enum.GetValues(typeof(ModSide)).Cast<ModSide>()
            }
        };
    }
}