using CommonServiceLocator;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Windows;

namespace ForgeModGenerator.ViewModel
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
            = DependencyProperty.RegisterAttached("GetDataContext", typeof(string), typeof(ViewModelLocator), new PropertyMetadata("", GetDataContextChanged));
        public static string GetGetDataContext(DependencyObject obj) => (string)obj.GetValue(GetDataContextProperty);
        public static void SetGetDataContext(DependencyObject obj, string value) => obj.SetValue(GetDataContextProperty, value);
        private static void GetDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement callOwner = d as FrameworkElement;
            string className = (string)d.GetValue(GetDataContextProperty);
            if (className != null)
            {
                callOwner.DataContext = ServiceLocator.Current.GetInstance(Type.GetType(className));
            }
        }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                NavigationService navigation = new NavigationService();
                PageInfo[] pages = Pages.GetAllPagesInfo();
                for (int i = 0; i < pages.Length; i++)
                {
                    navigation.Configure(pages[i].key, pages[i].type);
                }
                SimpleIoc.Default.Register<INavigationService>(() => navigation);
            }
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<NavigationMenuViewModel>();
        }

        public static void Cleanup()
        {
        }
    }
}