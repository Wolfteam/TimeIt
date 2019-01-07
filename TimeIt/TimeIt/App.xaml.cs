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
            //TODO: I SHOULD REMOVE THE HARDCODED FONT SIZES FROM THE MAIN PAGE
            //TODO: I SHOULD MOVE THIS COLORS
            var page = new NavigationPage(new MainPage());
            string barBgColor = "#1d1a1a";
            page.BarBackgroundColor = Color.FromHex(barBgColor);
            page.BarTextColor = Color.White;

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
            ViewModelLocator.WasAppInForeground = true;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
