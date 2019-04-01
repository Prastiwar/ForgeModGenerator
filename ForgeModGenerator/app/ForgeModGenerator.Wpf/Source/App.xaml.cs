using ForgeModGenerator.AchievementGenerator.ViewModels;
using ForgeModGenerator.AchievementGenerator.Views;
using ForgeModGenerator.ApplicationModule.ViewModels;
using ForgeModGenerator.ApplicationModule.Views;
using ForgeModGenerator.BlockGenerator.ViewModels;
using ForgeModGenerator.BlockGenerator.Views;
using ForgeModGenerator.CommandGenerator.ViewModels;
using ForgeModGenerator.CommandGenerator.Views;
using ForgeModGenerator.ItemGenerator.ViewModels;
using ForgeModGenerator.ItemGenerator.Views;
using ForgeModGenerator.ModGenerator.ViewModels;
using ForgeModGenerator.ModGenerator.Views;
using ForgeModGenerator.RecipeGenerator.ViewModels;
using ForgeModGenerator.RecipeGenerator.Views;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator.ViewModels;
using ForgeModGenerator.SoundGenerator.Views;
using ForgeModGenerator.TextureGenerator.ViewModels;
using ForgeModGenerator.TextureGenerator.Views;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace ForgeModGenerator
{
    public partial class App : PrismApplication
    {
        public App()
        {
#if !DEBUG
            DispatcherUnhandledException += App_DispatcherUnhandledException;
#endif
        }

        protected override Window CreateShell() => (Window)Container.Resolve(typeof(MainWindow));

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);

            DialogService dialogService = new DialogService();
            containerRegistry.RegisterInstance<IDialogService>(dialogService);

            NLogLoggerFactory fac = new NLogLoggerFactory();
            Log.Initialize(dialogService, fac.CreateLogger("ErrorLog"), fac.CreateLogger("InfoLog"));

            containerRegistry.RegisterInstance<ISessionContextService>(WpfSessionContextService.Instance);
            containerRegistry.Register<ISnackbarService, SnackbarService>();
            containerRegistry.Register<IFileSystem, FileSystemWin>();

            containerRegistry.Register<IWorkspaceSetupService, WorkspaceSetupService>();
            containerRegistry.Register<IModBuildService, ModBuildService>();

            containerRegistry.Register<NavigationMenuViewModel>();
            containerRegistry.Register<MainWindowViewModel>();

            containerRegistry.RegisterForNavigation<DashboardPage, DashboardViewModel>(Pages.Dashboard);
            containerRegistry.RegisterForNavigation<BuildConfigurationPage, BuildConfigurationViewModel>(Pages.BuildConfiguration);
            containerRegistry.RegisterForNavigation<ModGeneratorPage, ModGeneratorViewModel>(Pages.ModGenerator);
            containerRegistry.RegisterForNavigation<TextureGeneratorPage, TextureGeneratorViewModel>(Pages.TextureGenerator);
            containerRegistry.RegisterForNavigation<BlockGeneratorPage, BlockGeneratorViewModel>(Pages.BlockGenerator);
            containerRegistry.RegisterForNavigation<ItemGeneratorPage, ItemGeneratorViewModel>(Pages.ItemGenerator);
            containerRegistry.RegisterForNavigation<SoundGeneratorPage, SoundGeneratorViewModel>(Pages.SoundGenerator);
            containerRegistry.RegisterForNavigation<CommandGeneratorPage, CommandGeneratorViewModel>(Pages.CommandGenerator);
            containerRegistry.RegisterForNavigation<AchievementGeneratorPage, AchievementGeneratorViewModel>(Pages.AchievementGenerator);
            containerRegistry.RegisterForNavigation<RecipeGeneratorPage, RecipeGeneratorViewModel>(Pages.RecipeGenerator);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(Pages.Settings);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            new ApplicationModule.BugReporter(e.Exception).Show();
            MainWindow.Close();
            e.Handled = true;
        }
    }
}
