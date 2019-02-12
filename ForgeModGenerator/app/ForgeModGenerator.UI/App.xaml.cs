using CommonServiceLocator;
using ForgeModGenerator.Services;
using System.Windows;

namespace ForgeModGenerator
{
    public partial class App : Application
    {
        public App() => DispatcherUnhandledException += App_DispatcherUnhandledException;

        public static bool IsDataDirty {
            get => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose;
            set => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose = value;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            new ApplicationModule.BugReporter(e.Exception).Show();
            MainWindow.Close();
            e.Handled = true;
        }
    }
}
