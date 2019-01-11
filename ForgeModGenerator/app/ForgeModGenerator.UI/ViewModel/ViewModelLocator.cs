using CommonServiceLocator;
using ForgeModGenerator.Service;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;

namespace ForgeModGenerator.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                NavigationService navigation = new NavigationService();
                KeyValuePair<string, System.Type>[] pages = App.Pages.GetAllPagesInfo();
                for (int i = 0; i < pages.Length; i++)
                {
                    navigation.Configure(pages[i].Key, pages[i].Value);
                }
                SimpleIoc.Default.Register<INavigationService>(() => navigation);
            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NavigationMenuViewModel>();
            SimpleIoc.Default.Register<DashboardViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public NavigationMenuViewModel NavigationMenu => ServiceLocator.Current.GetInstance<NavigationMenuViewModel>();
        public DashboardViewModel Dashboard => ServiceLocator.Current.GetInstance<DashboardViewModel>();

        public static void Cleanup()
        {
        }
    }
}