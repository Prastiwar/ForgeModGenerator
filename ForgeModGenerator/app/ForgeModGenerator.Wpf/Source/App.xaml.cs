using CommonServiceLocator;
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

        public static bool IsDataDirty {
            get => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose;
            set => ServiceLocator.Current.GetInstance<ISessionContextService>().AskBeforeClose = value;
        }

        protected override Window CreateShell() => (Window)Container.Resolve(typeof(MainWindow));

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);

            containerRegistry.Register<NavigationMenuViewModel>();
            containerRegistry.Register<MainWindowViewModel>();
            containerRegistry.Register<ISessionContextService, SessionContextService>();
            containerRegistry.Register<IWorkspaceSetupService, WorkspaceSetupService>();
            containerRegistry.Register<IModBuildService, ModBuildService>();
            containerRegistry.Register<IDialogService, DialogService>();
            containerRegistry.Register<ISnackbarService, SnackbarService>();

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
