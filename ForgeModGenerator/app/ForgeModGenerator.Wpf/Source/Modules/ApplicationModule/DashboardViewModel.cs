using ForgeModGenerator.Services;
using GalaSoft.MvvmLight;

namespace ForgeModGenerator.ApplicationModule.ViewModels
{
    /// <summary> Dashboard Business ViewModel </summary>
    public class DashboardViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }

        public DashboardViewModel(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
        }
    }
}
