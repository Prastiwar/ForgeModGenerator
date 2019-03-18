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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Windows;

namespace ForgeModGenerator.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        // Automatically registered page
        public class PageInfo<T> : PageInfo where T : ViewModelBase
        {
            public PageInfo(string key, Type pageType) : base(key, pageType)
            {
                if (!SimpleIoc.Default.IsRegistered<T>())
                {
                    SimpleIoc.Default.Register<T>();
                }
            }
        }

        // Holds information about page key and page type(class)
        public class PageInfo
        {
            public readonly string key;
            public readonly Type type;

            public PageInfo(string key, Type type)
            {
                this.key = key;
                this.type = type;
            }
        }

        // Holds info about every page in application
        public struct Pages
        {
            public const string Dashboard = "Dashboard";
            public const string BuildConfiguration = "BuildConfiguration";
            public const string ModGenerator = "ModGenerator";
            public const string TextureGenerator = "TextureGenerator";
            public const string BlockGenerator = "BlockGenerator";
            public const string ItemGenerator = "ItemGenerator";
            public const string SoundGenerator = "SoundGenerator";
            public const string CommandGenerator = "CommandGenerator";
            public const string AchievementGenerator = "AchievementGenerator";
            public const string RecipeGenerator = "RecipeGenerator";
            public const string Settings = "Settings";

            public static PageInfo[] GetAllPagesInfo() => new PageInfo[] {
                new PageInfo<DashboardViewModel>(Dashboard, typeof(DashboardPage)),
                new PageInfo<BuildConfigurationViewModel>(BuildConfiguration, typeof(BuildConfigurationPage)),
                new PageInfo<ModGeneratorViewModel>(ModGenerator, typeof(ModGeneratorPage)),
                new PageInfo<TextureGeneratorViewModel>(TextureGenerator, typeof(TextureGeneratorPage)),
                new PageInfo<BlockGeneratorViewModel>(BlockGenerator, typeof(BlockGeneratorPage)),
                new PageInfo<ItemGeneratorViewModel>(ItemGenerator, typeof(ItemGeneratorPage)),
                new PageInfo<SoundGeneratorViewModel>(SoundGenerator, typeof(SoundGeneratorPage)),
                new PageInfo<CommandGeneratorViewModel>(CommandGenerator, typeof(CommandGeneratorPage)),
                new PageInfo<AchievementGeneratorViewModel>(AchievementGenerator, typeof(AchievementGeneratorPage)),
                new PageInfo<RecipeGeneratorViewModel>(RecipeGenerator, typeof(RecipeGeneratorPage)),
                new PageInfo<SettingsViewModel>(Settings, typeof(SettingsPage))
            };
        }

        public static readonly DependencyProperty GetDataContextProperty
            = DependencyProperty.RegisterAttached("GetDataContext", typeof(string), typeof(ViewModelLocator), new PropertyMetadata("Default", GetDataContextChanged));
        public static string GetGetDataContext(DependencyObject obj) => (string)obj.GetValue(GetDataContextProperty);
        public static void SetGetDataContext(DependencyObject obj, string value) => obj.SetValue(GetDataContextProperty, value);
        private static void GetDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                return;
            }
            FrameworkElement callOwner = d as FrameworkElement;
            string className = GetGetDataContext(d);
            if (className != null && className != "Default")
            {
                callOwner.DataContext = ServiceLocator.Current.GetInstance(Type.GetType(className));
            }
        }

        public ViewModelLocator()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                RegisterServices();
                RegisterViewModels();
            }
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<NavigationMenuViewModel>();
            SimpleIoc.Default.Register<MainWindowViewModel>();
            // pages ViewModels are generated with NavigationService
        }

        private void RegisterServices()
        {
            SimpleIoc.Default.Register<ISessionContextService, SessionContextService>();
            SimpleIoc.Default.Register<IWorkspaceSetupService, WorkspaceSetupService>();
            SimpleIoc.Default.Register<IModBuildService, ModBuildService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ISnackbarService, SnackbarService>();

            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                NavigationService service = new NavigationService();
                PageInfo[] pages = Pages.GetAllPagesInfo();
                for (int i = 0; i < pages.Length; i++)
                {
                    service.Configure(pages[i].key, pages[i].type); // also registers all pages
                }
                SimpleIoc.Default.Register<INavigationService>(() => service);
            }
        }

        public static void Cleanup()
        {
        }
    }
}