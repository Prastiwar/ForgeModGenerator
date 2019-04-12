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
using ForgeModGenerator.Models;
using ForgeModGenerator.ModGenerator.Validations;
using ForgeModGenerator.ModGenerator.ViewModels;
using ForgeModGenerator.ModGenerator.Views;
using ForgeModGenerator.RecipeGenerator.ViewModels;
using ForgeModGenerator.RecipeGenerator.Views;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Validations;
using ForgeModGenerator.SoundGenerator.ViewModels;
using ForgeModGenerator.SoundGenerator.Views;
using ForgeModGenerator.TextureGenerator.ViewModels;
using ForgeModGenerator.TextureGenerator.Views;
using ForgeModGenerator.Validation;
using Microsoft.Extensions.Caching.Memory;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

            SetProvider(containerRegistry);

            DialogService dialogService = new DialogService();
            NLogLoggerFactory fac = new NLogLoggerFactory();
            Log.Initialize(dialogService, fac.CreateLogger("ErrorLog"), fac.CreateLogger("InfoLog"));

            containerRegistry.RegisterInstance<IDialogService>(dialogService);
            RegisterServices(containerRegistry);

            containerRegistry.RegisterInstance<ISynchronizeInvoke>(SyncInvokeObject.Default);
            containerRegistry.RegisterInstance<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            containerRegistry.Register<IFileSystem, FileSystemWin>();

            RegisterSerializers(containerRegistry);
            RegisterValidators(containerRegistry);
            RegisterFactories(containerRegistry);
            RegisterPages(containerRegistry);
        }

        private void SetProvider(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => {
                string viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                string coreAssemblyName = typeof(MainWindowViewModel).Assembly.FullName;
                string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, coreAssemblyName);
                return Type.GetType(viewModelName);
            });

            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => {
                return containerRegistry.GetContainer().TryResolve(type);
            });
        }

        private void RegisterValidators(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IUniqueValidator<SoundEvent>, SoundEventValidator>();
            containerRegistry.Register<IValidator<Mod>, ModValidator>();
        }

        private void RegisterSerializers(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISerializer, JsonSerializer>();
            containerRegistry.Register<ISerializer<PreferenceData>, PreferenceDataSerializer>();

            containerRegistry.Register<ISerializer<IEnumerable<SoundEvent>, SoundEvent>, SoundEventsSerializer>();
            containerRegistry.Register<ISoundEventsSerializer, SoundEventsSerializer>();
        }

        private void RegisterFactories(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISoundJsonUpdaterFactory, SoundJsonUpdaterFactory>();

            containerRegistry.Register<IFoldersFactory<SoundEvent, Sound>, SoundEventsFactory>();
            containerRegistry.Register<IFoldersFinder<SoundEvent, Sound>, SoundEventsFinder>();
            containerRegistry.Register<IFolderSynchronizerFactory<SoundEvent, Sound>, SoundEventsSynchronizerFactory>();

            containerRegistry.Register(typeof(IFoldersFactory<,>), typeof(WpfFoldersFactory<,>));
            containerRegistry.Register(typeof(IFolderSynchronizerFactory<,>), typeof(FolderSynchronizerFactory<,>));
            containerRegistry.Register(typeof(IFoldersExplorerFactory<,>), typeof(FoldersExplorerFactory<,>));
            containerRegistry.Register(typeof(IFoldersFinder<,>), typeof(DefaultFoldersFinder<,>));

            containerRegistry.Register(typeof(IEditorFormFactory<>), typeof(EditorFormFactory<>));
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ISessionContextService>(WpfSessionContextService.Instance);
            containerRegistry.Register<INavigationService, PrismRegionNavigationBridge>();
            containerRegistry.Register<ISnackbarService, SnackbarService>();
            containerRegistry.Register<IModBuildService, ModBuildService>();
            containerRegistry.Register<IPreferenceService, PreferenceService>();
        }

        private void RegisterPages(IContainerRegistry containerRegistry)
        {
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
