using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;

namespace ForgeModGenerator.ViewModel
{
    /// <summary> ModGenerator Business ViewModel </summary>
    public class ModGeneratorViewModel : ViewModelBase
    {
        private readonly ISessionContextService sessionContext;

        public ModGeneratorViewModel(ISessionContextService sessionContext)
        {
            this.sessionContext = sessionContext;
        }
    }
}
