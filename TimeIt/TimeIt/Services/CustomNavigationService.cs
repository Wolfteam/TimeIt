using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace TimeIt.Services
{
    public class CustomNavigationService : INavigationService
    {
        // Dictionary with registered pages in the app:
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        // Navigation page where MainPage is hosted:
        //private NavigationPage _navigation;

        public string CurrentPageKey
        {
            get
            {
                lock (_pagesByKey)
                {
                    var navigation = GetNavigationPage();
                    if (navigation.CurrentPage == null)
                    {
                        return null;
                    }

                    var pageType = navigation.CurrentPage.GetType();

                    return _pagesByKey.ContainsValue(pageType) ?
                        _pagesByKey.First(p => p.Value == pageType).Key.ToString() :
                        null;
                }
            }
        }

        public void GoBack()
        {
            var navigation = GetNavigationPage();
            Device.BeginInvokeOnMainThread(async () => await navigation.PopAsync());
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(pageKey))
                {
                    var type = _pagesByKey[pageKey];
                    ConstructorInfo constructor;
                    object[] parameters;

                    if (parameter == null)
                    {
                        constructor = type.GetTypeInfo()
                            .DeclaredConstructors
                            .FirstOrDefault(c => !c.GetParameters().Any());

                        parameters = new object[] { };
                    }
                    else
                    {
                        constructor = type.GetTypeInfo()
                            .DeclaredConstructors
                            .FirstOrDefault(c =>
                            {
                                var p = c.GetParameters();
                                return p.Count() == 1
                                        && p[0].ParameterType == parameter.GetType();
                            });

                        parameters = new[] { parameter };
                    }

                    if (constructor == null)
                    {
                        throw new InvalidOperationException($"No suitable constructor found for page {pageKey}");
                    }

                    var page = constructor.Invoke(parameters) as Page;
                    var navigation = GetNavigationPage();
                    Device.BeginInvokeOnMainThread(async () => await navigation.PushAsync(page));
                }
                else
                {
                    throw new ArgumentException($"The page = {pageKey} does not exists ", nameof(pageKey));
                }
            }
        }

        public void Configure(string pageKey, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(pageKey))
                {
                    _pagesByKey[pageKey] = pageType;
                }
                else
                {
                    _pagesByKey.Add(pageKey, pageType);
                }
            }
        }

        public void Initialize(NavigationPage navigation)
        {
            //_navigation = navigation;
        }

        private NavigationPage GetNavigationPage()
            => Application.Current.MainPage as NavigationPage;
    }
}
