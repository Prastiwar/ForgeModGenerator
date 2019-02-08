using CommonServiceLocator;
using ForgeModGenerator.Services;
using System.Windows;

namespace ForgeModGenerator
{
    public partial class App : Application
    {
        public static bool IsDataDirty {
            get => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose;
            set => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose = value;
        }
    }
}
