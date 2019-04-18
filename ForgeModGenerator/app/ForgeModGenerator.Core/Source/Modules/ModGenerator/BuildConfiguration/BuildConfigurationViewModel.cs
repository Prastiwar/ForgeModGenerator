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
        private readonly IModBuildService modBuilder;

        public ISessionContextService SessionContext { get; }
        protected ISerializer<Mod> ModSerializer { get; }

        public BuildConfigurationViewModel(ISessionContextService sessionContext, IModBuildService modBuilder, ISerializer<Mod> modSerializer)
        {
            this.modBuilder = modBuilder;
            ModSerializer = modSerializer;
            SessionContext = sessionContext;
        }

        private ICommand runClientCommand;
        public ICommand RunClientCommand => runClientCommand ?? (runClientCommand = new DelegateCommand<Mod>((mod) => { modBuilder.RunClient(mod); }));

        private ICommand runServerCommand;
        public ICommand RunServerCommand => runServerCommand ?? (runServerCommand = new DelegateCommand<Mod>((mod) => { modBuilder.RunServer(mod); }));

        private ICommand runBothCommand;
        public ICommand RunBothCommand => runBothCommand ?? (runBothCommand = new DelegateCommand<Mod>((mod) => {
            modBuilder.RunClient(mod);
            modBuilder.RunServer(mod);
        }));

        private ICommand compileCommand;
        public ICommand CompileCommand => compileCommand ?? (compileCommand = new DelegateCommand<Mod>((mod) => { modBuilder.Compile(mod); }));

        private ICommand toggleSelectCommand;
        public ICommand ToggleSelectCommand => toggleSelectCommand ?? (toggleSelectCommand = new DelegateCommand<Mod>(ToggleLaunchSelection));

        private void ToggleLaunchSelection(Mod mod)
        {
            bool isSelected = SessionContext.SelectedMods.Contains(mod);
            if (isSelected)
            {
                SessionContext.SelectedMods.Remove(mod);
                mod.PropertyChanged -= Mod_PropertyChanged;
            }
            else
            {
                SessionContext.SelectedMods.Add(mod);
                mod.PropertyChanged += Mod_PropertyChanged;
            }
        }

        private void Mod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.LaunchSetup))
            {
                ModHelper.ExportMod(ModSerializer, (Mod)sender);
            }
        }
    }
}
