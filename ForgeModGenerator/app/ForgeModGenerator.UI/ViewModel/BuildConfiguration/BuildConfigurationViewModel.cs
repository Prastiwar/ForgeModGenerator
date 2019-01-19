using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> BuildConfiguration Business ViewModel </summary>
    public class BuildConfigurationViewModel : ViewModelBase
    {
        public ISessionContextService SessionContext { get; }

        public BuildConfigurationViewModel(ISessionContextService sessionContext)
        {
            SessionContext = sessionContext;
        }
    }
}
