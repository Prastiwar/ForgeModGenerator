using ForgeModGenerator.ApplicationModule.ViewModels;
using ForgeModGenerator.ApplicationModule.Views;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.CommandGenerator;
using ForgeModGenerator.CommandGenerator.Models;
using ForgeModGenerator.CommandGenerator.Validation;
using ForgeModGenerator.Models;
using ForgeModGenerator.ModGenerator;
using ForgeModGenerator.ModGenerator.Validation;
using ForgeModGenerator.RecipeGenerator;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.RecipeGenerator.Validation;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using ForgeModGenerator.SoundGenerator.Validation;
using ForgeModGenerator.Validation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Caching.Memory;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Unity;

namespace ForgeModGenerator
{
    public partial class App : PrismApplication
    {
        public App() =>
#if !DEBUG
            DispatcherUnhandledException += App_DispatcherUnhandledException;
#endif
            AppDomain.CurrentDomain.ProcessExit += OnExit;

        private readonly MemoryCache globalCache = new MemoryCache(new MemoryCacheOptions());

        private void OnExit(object sender, EventArgs e) => globalCache.Dispose();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppCenter.Start("7f5f53af-bce9-4a02-a037-b676b4ee97c0", typeof(Analytics), typeof(Crashes));
        }

        protected override Window CreateShell() => (Window)Container.Resolve(typeof(MainWindow));

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            SetProvider(containerRegistry);
            DialogService dialogService = new DialogService();
            NLogLoggerFactory fac = new NLogLoggerFactory();
            Log.Initialize(dialogService, fac.CreateLogger("ErrorLog"), fac.CreateLogger("InfoLog"));
            fac.Dispose();
            containerRegistry.RegisterInstance<IMemoryCache>(globalCache);
            SourceCodeLocator.Initialize(globalCache);

            containerRegistry.RegisterInstance<IDialogService>(dialogService);
            RegisterServices(containerRegistry);

            containerRegistry.RegisterInstance<ISynchronizeInvoke>(SyncInvokeObject.Default);
            containerRegistry.Register<IFileSystem, FileSystemWin>();
            containerRegistry.Register<ICodeGenerationService, CodeGeneratorService>();

            RegisterWorkspaceSetups(containerRegistry);
            RegisterSerializers(containerRegistry);
            RegisterValidators(containerRegistry);

            ObservableCollection<WorkspaceSetup> workspaceSetups = new ObservableCollection<WorkspaceSetup>();
            foreach (IContainerRegistration registry in containerRegistry.GetContainer().Registrations)
            {
                if (registry.RegisteredType == typeof(WorkspaceSetup))
                {
                    WorkspaceSetup setup = (WorkspaceSetup)containerRegistry.GetContainer().Resolve(typeof(WorkspaceSetup), registry.Name);
                    workspaceSetups.Add(setup);
                }
            }
            containerRegistry.RegisterInstance(workspaceSetups);

            RegisterFactories(containerRegistry);
            RegisterPages(containerRegistry);
        }

        private void RegisterWorkspaceSetups(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(WorkspaceSetup), typeof(EmptyWorkspace), "None");
            containerRegistry.Register(typeof(WorkspaceSetup), typeof(VSCodeWorkspace), "VSCode");
            containerRegistry.Register(typeof(WorkspaceSetup), typeof(IntelliJIDEAWorkspace), "IntelliJIDEA");
            containerRegistry.Register(typeof(WorkspaceSetup), typeof(EclipseWorkspace), "Eclipse");
        }

        private void SetProvider(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => {
                string coreAssemblyName = typeof(MainWindowViewModel).Assembly.FullName;
                string moduleName = viewType.FullName.Replace(".Views.", ".ViewModels.").Replace("Page", "");
                string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", moduleName, coreAssemblyName);
                return Type.GetType(viewModelName);
            });
            ViewModelLocationProvider.SetDefaultViewModelFactory(type => containerRegistry.GetContainer().TryResolve(type));
        }

        private void RegisterValidators(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IUniqueValidator<SoundEvent>, SoundEventValidator>();
            containerRegistry.Register<IUniqueValidator<Recipe>, RecipeValidator>();
            containerRegistry.Register<IUniqueValidator<Command>, CommandValidator>();
            containerRegistry.Register<IValidator<McMod>, ModValidator>();
        }

        private void RegisterSerializers(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISerializer, JsonSerializer>();
            containerRegistry.Register(typeof(ISerializer<>), typeof(JsonSerializer<>));

            Assembly presentationAssembly = Assembly.GetAssembly(typeof(MainWindow));
            Assembly coreAssembly = Assembly.GetAssembly(typeof(MainWindowViewModel));
            IEnumerable<string> serializedModelNames = presentationAssembly.ExportedTypes.Where(x => x.Name.EndsWith("Serializer")).Select(x => x.Name.Replace("Serializer", ""));
            foreach (string serializedItemName in serializedModelNames)
            {
                string modelName = serializedItemName;
                Type model = coreAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName);
                bool isNamedPlural = modelName.EndsWith("s");
                if (model == null && isNamedPlural)
                {
                    modelName = modelName.Remove(modelName.Length - 1, 1);
                    model = coreAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName);
                }
                if (model != null)
                {
                    Type serializer = presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + "Serializer")
                                     ?? presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + "sSerializer");
                    if (serializer != null)
                    {
                        // Predicate to exclude ISerializer, take all that ends with "Serializer" including generic versions
                        Func<Type, bool> isSerializer = x => x.Name != nameof(ISerializer) && (x.Name.EndsWith("Serializer") || x.Name.Remove(x.Name.Length - 2, 2).EndsWith("Serializer"));
                        foreach (Type serializationInterface in serializer.GetInterfaces().Where(isSerializer))
                        {
                            containerRegistry.Register(serializationInterface, serializer);
                        }
                    }
                }
            }
        }

        private void RegisterFactories(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IJsonUpdaterFactory<>), typeof(JsonUpdaterFactory<>));
            containerRegistry.Register(typeof(IJsonUpdaterFactory<,>), typeof(CollectionJsonUpdaterFactory<,>));
            containerRegistry.Register<ISoundJsonUpdaterFactory, SoundJsonUpdaterFactory>();

            containerRegistry.Register<IFoldersFactory<SoundEvent, Sound>, SoundEventsFactory>();
            containerRegistry.Register<IFoldersFinder<SoundEvent, Sound>, SoundEventsFinder>();
            containerRegistry.Register<IFolderSynchronizerFactory<SoundEvent, Sound>, SoundEventsSynchronizerFactory>();
            containerRegistry.Register(typeof(IEditorFormFactory<Sound>), typeof(SoundEditorFormFactory));

            containerRegistry.Register(typeof(IFoldersFactory<,>), typeof(WpfFoldersFactory<,>));
            containerRegistry.Register(typeof(IFolderSynchronizerFactory<,>), typeof(FolderSynchronizerFactory<,>));
            containerRegistry.Register(typeof(IFoldersExplorerFactory<,>), typeof(FoldersExplorerFactory<,>));
            containerRegistry.Register(typeof(IFoldersFinder<,>), typeof(DefaultFoldersFinder<,>));

            containerRegistry.Register(typeof(IFoldersFactory<ObservableFolder<Recipe>, Recipe>), typeof(RecipesFactory));

            containerRegistry.Register(typeof(IEditorFormFactory<McMod>), typeof(ModEditorFormFactory));
            containerRegistry.Register(typeof(IEditorFormFactory<Command>), typeof(CommandEditorFormFactory));
            containerRegistry.Register(typeof(IEditorFormFactory<RecipeCreator>), typeof(RecipeEditorFormFactory));
            containerRegistry.Register(typeof(IEditorFormFactory<>), typeof(EditorFormFactory<>));
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISessionContextService, WpfSessionContextService>();
            containerRegistry.Register<INavigationService, PrismRegionNavigationBridge>();
            containerRegistry.Register<ISnackbarService, SnackbarService>();
            containerRegistry.Register<IModBuildService, ModBuildService>();
            containerRegistry.Register<IPreferenceService, PreferenceService>();
        }

        private void RegisterPages(IContainerRegistry containerRegistry)
        {
            Assembly presentationAssembly = Assembly.GetAssembly(typeof(MainWindow));
            Assembly coreAssembly = Assembly.GetAssembly(typeof(MainWindowViewModel));
            IEnumerable<string> moduleNames = coreAssembly.ExportedTypes.Where(x => x.Name.EndsWith("ViewModel")).Select(x => x.Name.Replace("ViewModel", ""));
            foreach (string module in moduleNames)
            {
                Type viewModel = coreAssembly.ExportedTypes.First(x => x.Name.StartsWith(module) && x.Name.EndsWith("ViewModel"));
                Type page = presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name.StartsWith(module) && x.Name.EndsWith("Page"));
                if (page != null)
                {
                    string navName = (string)typeof(Pages).GetField(module, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
                    containerRegistry.RegisterForNavigation(page, navName);
                }
            }
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
            regionBehaviors.AddIfMissing(nameof(DisposeClosedViewsBehavior), typeof(DisposeClosedViewsBehavior));
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            new ApplicationModule.BugReporter(e.Exception).Show();
            MainWindow.Close();
            e.Handled = true;
            Crashes.TrackError(e.Exception);
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
        }
    }
}
