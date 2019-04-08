using ForgeModGenerator.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ForgeModGenerator.Services
{
    public interface IWorkspaceSetupService : INotifyPropertyChanged
    {
        ObservableCollection<WorkspaceSetup> Setups { get; }
    }
}
