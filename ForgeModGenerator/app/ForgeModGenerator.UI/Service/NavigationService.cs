using ForgeModGenerator.Miscellaneous;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace ForgeModGenerator.Service
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();

        private Frame _frame;
        public Frame NavFrame {
            get {
                if (_frame == null)
                {
                    _frame = Util.GetDescendantFromName(System.Windows.Application.Current.MainWindow, "PageFrame") as Frame;
                }
                return _frame;
            }
            set { _frame = value; }
        }

        public string CurrentPageKey { get; protected set; }

        public NavigationService(Frame frame = null)
        {
            NavFrame = frame;
        }

        public void GoBack() => NavFrame.GoBack();

        public bool CanGoBack() => NavFrame.CanGoBack;

        public void NavigateTo(string pageKey) => NavigateTo(pageKey, null);

        public void NavigateTo(string pageKey, object parameter) => NavigateTo(pageKey, parameter);

        private void NavigateTo(string pageKey, params object[] parameters)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"No such page: {pageKey}. Did you forget to call NavigationService.Configure?", nameof(pageKey));
                }
                Type type = _pagesByKey[pageKey];
                ConstructorInfo constructor;
                constructor = GetConstructor(type, parameters);
                if (constructor == null)
                {
                    throw new InvalidOperationException($"No suitable constructor found for page {pageKey}");
                }
                Page page = constructor.Invoke(parameters) as Page;
                NavFrame.Navigate(page);
            }
        }

        private ConstructorInfo GetConstructor(Type type, object[] parameters)
        {
            int parameterCount = parameters != null ? parameters.Length : 0;
            ConstructorInfo constructor;
            if (parameterCount > 0)
            {
                constructor = type.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => {
                    ParameterInfo[] p = c.GetParameters();
                    return p.Count() == parameterCount && p[parameterCount - 1].ParameterType == parameters[parameterCount - 1].GetType();
                });
            }
            else
            {
                constructor = type.GetTypeInfo()
                                .DeclaredConstructors
                                .FirstOrDefault(c => !c.GetParameters().Any());
            }
            return constructor;
        }

        public void Configure(string pageKey, Type pageType)
        {
            lock (_pagesByKey)
            {
                _pagesByKey[pageKey] = pageType;
            }
        }
    }
}
