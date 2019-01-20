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

        public TimerPageViewModel EditTimer
            => SimpleIoc.Default.GetInstance<TimerPageViewModel>();

        public IntervalPageViewModel EditInterval
            => SimpleIoc.Default.GetInstance<IntervalPageViewModel>();

        public ViewModelLocator()
        {
            if (SimpleIoc.Default.IsRegistered<INavigationService>())
                return;

            var navigation = new CustomNavigationService();
            navigation.Configure($"{AppPages.HOME}", typeof(MainPage));
            navigation.Configure($"{AppPages.TIMER}", typeof(TimerPage));
            navigation.Configure($"{AppPages.INTERVAL}", typeof(IntervalPage));

            SimpleIoc.Default.Register<INavigationService>(() => navigation);

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<TimerPageViewModel>();
            SimpleIoc.Default.Register<IntervalPageViewModel>();
        }

    }
}
