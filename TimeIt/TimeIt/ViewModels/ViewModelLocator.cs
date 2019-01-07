using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using TimeIt.Enums;
using TimeIt.Pages;
using TimeIt.Services;

namespace TimeIt.ViewModels
{
    public class ViewModelLocator
    {
        public static bool WasAppInForeground { get; set; }

        public MainPageViewModel Main
            => SimpleIoc.Default.GetInstance<MainPageViewModel>();

        public EditTimerPageViewModel EditTimer
            => SimpleIoc.Default.GetInstance<EditTimerPageViewModel>();

        public ViewModelLocator()
        {
            if (SimpleIoc.Default.IsRegistered<INavigationService>())
                return;

            var navigation = new CustomNavigationService();
            navigation.Configure($"{AppPages.HOME}", typeof(MainPage));
            navigation.Configure($"{AppPages.ADD_TIMER}", typeof(EditTimerPage));

            SimpleIoc.Default.Register<INavigationService>(() => navigation);

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<EditTimerPageViewModel>();
        }

    }
}
