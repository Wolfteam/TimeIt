using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Plugin.Toasts;
using TimeIt.Enums;
using TimeIt.Interfaces;
using TimeIt.Models;
using TimeIt.Pages;
using TimeIt.Services;
using Xamarin.Forms;

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

        public SettingsPageViewModel Settings
            => SimpleIoc.Default.GetInstance<SettingsPageViewModel>();

        public ViewModelLocator()
        {
            if (SimpleIoc.Default.IsRegistered<INavigationService>())
                return;

            var navigation = new CustomNavigationService();
            navigation.Configure($"{AppPages.HOME}", typeof(MainPage));
            navigation.Configure($"{AppPages.TIMER}", typeof(TimerPage));
            navigation.Configure($"{AppPages.INTERVAL}", typeof(IntervalPage));
            navigation.Configure($"{AppPages.SETTINGS}", typeof(SettingsPage));
            SimpleIoc.Default.Register<INavigationService>(() => navigation);

            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfiles(GetType().Assembly);
            });
            SimpleIoc.Default.Register(() => mapperConfig.CreateMapper());

            SimpleIoc.Default.Register<IMessenger, Messenger>();
            SimpleIoc.Default.Register<ITimeItDataService, TimeItDataService>();
            SimpleIoc.Default.Register<ICustomDialogService, DialogService>();
            SimpleIoc.Default.Register<IAppSettingsService, AppSettingsService>();

            SimpleIoc.Default.Register(() => DependencyService.Get<IToastNotificator>());
            SimpleIoc.Default.Register(() => DependencyService.Get<ISimpleMessage>());
            SimpleIoc.Default.Register(() => new TimeItDbContext());


            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<TimerPageViewModel>();
            SimpleIoc.Default.Register<IntervalPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
        }

    }
}
