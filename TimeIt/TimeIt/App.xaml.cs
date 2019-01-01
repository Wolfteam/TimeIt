using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using TimeIt.Services;
using TimeIt.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TimeIt
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //var navigation = SimpleIoc.Default.GetInstance<INavigationService>() as CustomNavigationService;
            var page = new NavigationPage(new MainPage());
            //navigation.Initialize(page);
            page.BarBackgroundColor = Color.FromHex("#1FBED6");
            MainPage = page;

            //this is to place the toolbar to the bottom on uwp
            MainPage.On<Xamarin.Forms.PlatformConfiguration.Windows>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
