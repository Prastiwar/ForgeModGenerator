using ForgeModGenerator.Models;
using ForgeModGenerator.Services;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace ForgeModGenerator.ModGenerator
{
    public class McModModelFormProvider : ModelFormProvider<McMod>
    {
        public McModModelFormProvider(ISessionContextService sessionContext, ObservableCollection<WorkspaceSetup> setups)
        {
            this.sessionContext = sessionContext;
            this.setups = setups;
        }

        private readonly ISessionContextService sessionContext;
        private readonly ObservableCollection<WorkspaceSetup> setups;

        public override IUIElement GetUIElement() => new Controls.ModForm() {
            AddForgeVersionCommand = new DelegateCommand(sessionContext.DownloadNewForgeVersion),
            Setups = setups,
            ForgeVersions = sessionContext.ForgeVersions
        };
    }
}
