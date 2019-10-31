using ForgeModGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace ForgeModGenerator.ModGenerator.ViewModels
{
    /// <summary> BuildConfiguration Business ViewModel </summary>
    public class BuildConfigurationViewModel : BindableBase
    {
        public BuildConfigurationViewModel(ISessionContextService sessionContext, IModBuildService modBuilder, ISerializer<McMod> modSerializer)
        {
            this.modBuilder = modBuilder;
            ModSerializer = modSerializer;
            SessionContext = sessionContext;
        }

        private readonly IModBuildService modBuilder;

        public ISessionContextService SessionContext { get; }

        protected ISerializer<McMod> ModSerializer { get; }

        private ICommand runClientCommand;
        public ICommand RunClientCommand => runClientCommand ?? (runClientCommand = new DelegateCommand<McMod>((mcMod) => modBuilder.RunClient(mcMod)));

        private ICommand runServerCommand;
        public ICommand RunServerCommand => runServerCommand ?? (runServerCommand = new DelegateCommand<McMod>((mcMod) => modBuilder.RunServer(mcMod)));

        private ICommand runBothCommand;
        public ICommand RunBothCommand => runBothCommand ?? (runBothCommand = new DelegateCommand<McMod>((mcMod) => {
            modBuilder.RunClient(mcMod);
            modBuilder.RunServer(mcMod);
        }));

        private ICommand compileCommand;
        public ICommand CompileCommand => compileCommand ?? (compileCommand = new DelegateCommand<McMod>((mod) => { modBuilder.Compile(mod); }));

        private ICommand toggleSelectCommand;
        public ICommand ToggleSelectCommand => toggleSelectCommand ?? (toggleSelectCommand = new DelegateCommand<McMod>(ToggleLaunchSelection));

        private void ToggleLaunchSelection(McMod mcMod)
        {
            bool isSelected = SessionContext.SelectedMods.Contains(mcMod);
            if (isSelected)
            {
                SessionContext.SelectedMods.Remove(mcMod);
                mcMod.PropertyChanged -= Mod_PropertyChanged;
            }
            else
            {
                SessionContext.SelectedMods.Add(mcMod);
                mcMod.PropertyChanged += Mod_PropertyChanged;
            }
        }

        private void Mod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(McMod.LaunchSetup))
            {
                ModHelper.ExportMod(ModSerializer, (McMod)sender);
            }
        }
    }
}
