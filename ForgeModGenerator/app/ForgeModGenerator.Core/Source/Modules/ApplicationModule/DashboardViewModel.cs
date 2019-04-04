using ForgeModGenerator.Services;
using Prism.Mvvm;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> Dashboard Business ViewModel </summary>
    public class DashboardViewModel : BindableBase
    {
        public ISessionContextService SessionContext { get; }

        public DashboardViewModel(ISessionContextService sessionContext) => SessionContext = sessionContext;
    }
}
