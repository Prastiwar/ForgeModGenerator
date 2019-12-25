using ForgeModGenerator.ApplicationModule.ViewModels;
using ForgeModGenerator.ApplicationModule.Views;
using ForgeModGenerator.CodeGeneration;
using ForgeModGenerator.Models;
using ForgeModGenerator.RecipeGenerator;
using ForgeModGenerator.RecipeGenerator.Models;
using ForgeModGenerator.Serialization;
using ForgeModGenerator.Services;
using ForgeModGenerator.SoundGenerator;
using ForgeModGenerator.SoundGenerator.Models;
using ForgeModGenerator.SoundGenerator.Serialization;
using ForgeModGenerator.Validation;
using ForgeModGenerator.ViewModels;
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
        public App()
        {
#if !DEBUG
            DispatcherUnhandledException += App_DispatcherUnhandledException;
#endif
            AppDomain.CurrentDomain.ProcessExit += OnExit;
            ;
        }

        private readonly MemoryCache globalCache = new MemoryCache(new MemoryCacheOptions());
        private readonly Assembly presentationAssembly = Assembly.GetAssembly(typeof(MainWindow));
        private readonly Assembly coreAssembly = Assembly.GetAssembly(typeof(MainWindowViewModel));

        private void OnExit(object sender, EventArgs e) => globalCache.Dispose();

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            new ApplicationModule.BugReporter(e.Exception).Show();
            MainWindow.Close();
            e.Handled = true;
            Crashes.TrackError(e.Exception);
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppCenter.Start("7f5f53af-bce9-4a02-a037-b676b4ee97c0", typeof(Analytics), typeof(Crashes));
        }

        protected override Window CreateShell() => (Window)Container.Resolve(typeof(MainWindow));

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
            regionBehaviors.AddIfMissing(nameof(DisposeClosedViewsBehavior), typeof(DisposeClosedViewsBehavior));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);

            SetProvider(containerRegistry);

            RegisterInstances(containerRegistry);

            InitializeStatics(containerRegistry);

            RegisterServices(containerRegistry);

            containerRegistry.Register<IFileSystem, FileSystemWin>();
            containerRegistry.Register(typeof(IUniqueValidator<>), typeof(GenericUniqueValidator<>));
            containerRegistry.Register(typeof(GeneratorContext<>), typeof(GeneratorContext<>));
            containerRegistry.Register(typeof(ModelFormProvider<>), typeof(NoneModelFormProvider<>));

            RegisterModels(containerRegistry);

            containerRegistry.Register<ISerializer, JsonSerializer>();
            containerRegistry.Register(typeof(ISerializer<>), typeof(JsonSerializer<>));

            RegisterWorkspaceSetups(containerRegistry);
            RegisterFactories(containerRegistry);
            RegisterPages(containerRegistry);
        }

        private void InitializeStatics(IContainerRegistry containerRegistry)
        {
            NLogLoggerFactory fac = new NLogLoggerFactory();
            Log.Initialize(containerRegistry.GetContainer().Resolve<DialogService>(), fac.CreateLogger("ErrorLog"), fac.CreateLogger("InfoLog"));
            fac.Dispose();

            SourceCodeLocator.Initialize(globalCache);
        }

        private void RegisterInstances(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IMemoryCache>(globalCache);
            containerRegistry.RegisterInstance<IDialogService>(new DialogService());
            containerRegistry.RegisterInstance<ISynchronizeInvoke>(SyncInvokeObject.Default);

            foreach (Type chooseCollectionType in coreAssembly.ExportedTypes.Where(x => x.Name.EndsWith("ChooseCollection")))
            {
                object instance = Activator.CreateInstance(chooseCollectionType);
                containerRegistry.RegisterInstance(chooseCollectionType, instance);
            }
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

        private void RegisterModels(IContainerRegistry containerRegistry)
        {
            IEnumerable<Type> models = coreAssembly.ExportedTypes.Where(x => x.FullName.Contains(".Models.") && !x.IsEnum);
            foreach (Type model in models)
            {
                RegisterModel(containerRegistry, model);
            }
            RegisterModel(containerRegistry, typeof(PreferenceData));
        }

        private void RegisterModel(IContainerRegistry containerRegistry, Type model)
        {
            bool IsSerializerRegistered = TryRegisterInterfaceForModel(containerRegistry, model, "Serializer");
            bool IsValidatorRegistered = TryRegisterInterfaceForModel(containerRegistry, model, "Validator");
            bool IsFormProviderRegistered = TryRegisterClassForModel(containerRegistry, model, "ModelFormProvider");
            bool IsEditorFormFactoryRegistered = TryRegisterInterfaceForModel(containerRegistry, model, "EditorFormFactory");
        }

        private bool TryRegisterClassForModel(IContainerRegistry containerRegistry, Type model, string name, Type defaultClassType = null)
        {
            bool registered = false;
            string modelName = model.Name;
            Type classType = presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + name)
                                         ?? presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + "s" + name);
            classType = classType ?? defaultClassType;
            if (classType != null)
            {
                // Predicate to take all types that ends with nameof(name) including generic versions
                bool isNamed(Type x) => x.Name.EndsWith(name) || x.Name.Remove(x.Name.Length - 2, 2).EndsWith(name);
                Type type = classType.BaseType;
                while (type != null && isNamed(type))
                {
                    containerRegistry.Register(type, classType);
                    registered = true;
                    type = type.BaseType;
                }
            }
            return registered;
        }

        private bool TryRegisterInterfaceForModel(IContainerRegistry containerRegistry, Type model, string name, Type defaultClassType = null)
        {
            bool registered = false;
            string modelName = model.Name;
            Type classType = presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + name)
                                         ?? presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == modelName + "s" + name);
            classType = classType ?? defaultClassType;
            if (classType != null)
            {
                // Predicate to take all types that ends with nameof(name) including generic versions
                bool isNamed(Type x) => x.Name.EndsWith(name) || x.Name.Remove(x.Name.Length - 2, 2).EndsWith(name);
                foreach (Type interfaceType in classType.GetInterfaces().Where(isNamed))
                {
                    containerRegistry.Register(interfaceType, classType);
                    registered = true;
                }
            }
            return registered;
        }

        private void RegisterWorkspaceSetups(IContainerRegistry containerRegistry)
        {
            ObservableCollection<WorkspaceSetup> workspaceSetups = new ObservableCollection<WorkspaceSetup>();
            IEnumerable<Type> workspaces = coreAssembly.ExportedTypes.Where(x => x.Name.EndsWith("Workspace"));
            foreach (Type workspace in workspaces)
            {
                string registryName = workspace.Name.Replace("Workspace", "");
                containerRegistry.Register(typeof(WorkspaceSetup), workspace, registryName);
                WorkspaceSetup setup = containerRegistry.GetContainer().Resolve<WorkspaceSetup>(registryName);
                workspaceSetups.Add(setup);
            }
            containerRegistry.RegisterInstance(workspaceSetups);
        }

        private void RegisterPages(IContainerRegistry containerRegistry)
        {
            IEnumerable<string> moduleNames = coreAssembly.ExportedTypes.Where(x => x.Name.EndsWith("ViewModel")).Select(x => x.Name.Replace("ViewModel", ""));
            foreach (string module in moduleNames)
            {
                Type viewModel = coreAssembly.ExportedTypes.First(x => x.Name == module + "ViewModel");
                Type page = presentationAssembly.ExportedTypes.FirstOrDefault(x => x.Name == module + "Page");
                if (page != null)
                {
                    containerRegistry.RegisterForNavigation(page, module);
                }
            }
        }

        private void RegisterFactories(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IJsonUpdaterFactory<>), typeof(JsonUpdaterFactory<>));
            containerRegistry.Register(typeof(IJsonUpdaterFactory<,>), typeof(CollectionJsonUpdaterFactory<,>));
            containerRegistry.Register(typeof(IFoldersFactory<,>), typeof(WpfFoldersFactory<,>));
            containerRegistry.Register(typeof(IFolderSynchronizerFactory<,>), typeof(FolderSynchronizerFactory<,>));
            containerRegistry.Register(typeof(IFoldersExplorerFactory<,>), typeof(FoldersExplorerFactory<,>));
            containerRegistry.Register(typeof(IFoldersFinder<,>), typeof(DefaultFoldersFinder<,>));
            containerRegistry.Register(typeof(IEditorFormFactory<>), typeof(ModelEditorFormFactory<>));

            containerRegistry.Register<ISoundJsonUpdaterFactory, SoundJsonUpdaterFactory>();
            containerRegistry.Register<IFoldersFactory<SoundEvent, Sound>, SoundEventsFactory>();
            containerRegistry.Register<IFoldersFinder<SoundEvent, Sound>, SoundEventsFinder>();
            containerRegistry.Register<IFolderSynchronizerFactory<SoundEvent, Sound>, SoundEventsSynchronizerFactory>();

            containerRegistry.Register(typeof(IFoldersFactory<ObservableFolder<Recipe>, Recipe>), typeof(RecipesFactory));
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISessionContextService, WpfSessionContextService>();
            containerRegistry.Register<INavigationService, PrismRegionNavigationBridge>();
            containerRegistry.Register<ISnackbarService, SnackbarService>();
            containerRegistry.Register<IModBuildService, ModBuildService>();
            containerRegistry.Register<IPreferenceService, PreferenceService>();
            containerRegistry.Register<ICodeGenerationService, CodeGeneratorService>();
        }
    }
}
