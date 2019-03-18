using ForgeModGenerator.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ForgeModGenerator.Services
{
    public interface IWorkspaceSetupService : INotifyPropertyChanged
    {
        ObservableCollection<WorkspaceSetup> Setups { get; }
    }

    public class WorkspaceSetupService : IWorkspaceSetupService
    {
        public WorkspaceSetupService() => Setups = new ObservableCollection<WorkspaceSetup>() {
                                                        WorkspaceSetup.NONE,
                                                        new EclipseWorkspace(),
                                                        new IntelliJIDEAWorkspace(),
                                                        new VSCodeWorkspace()
                                                   };

        private ObservableCollection<WorkspaceSetup> setups;
        public ObservableCollection<WorkspaceSetup> Setups {
            get => setups;
            protected set => Set(ref setups, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if ((field != null && field.Equals(newValue))
                || (field == null && newValue == null))
            {
                return false;
            }
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
