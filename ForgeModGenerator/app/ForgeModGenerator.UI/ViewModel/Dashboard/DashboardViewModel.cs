using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;

namespace ForgeModGenerator.ViewModel
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
