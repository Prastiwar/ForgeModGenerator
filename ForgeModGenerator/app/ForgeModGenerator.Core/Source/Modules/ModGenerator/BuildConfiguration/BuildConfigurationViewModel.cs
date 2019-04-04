using ForgeModGenerator.Models;
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

        public BuildConfigurationViewModel(ISessionContextService sessionContext, IModBuildService modBuilder)
        {
            this.modBuilder = modBuilder;
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
            }
            else
            {
                SessionContext.SelectedMods.Add(mod);
            }
        }
    }
}
