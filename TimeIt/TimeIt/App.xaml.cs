using Plugin.Iconize;
using System;
using System.Linq;
using TimeIt.Helpers;
using TimeIt.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TimeIt
{
    public partial class App : Application
    {
        private const string SleepOccurredOn = "SleepOccurredOn";

        public App()
        {
            InitializeComponent();
            //TODO: I SHOULD REMOVE THE HARDCODED FONT SIZES FROM THE MAIN PAGE
            var page = new IconNavigationPage(new MainPage())
            {
                BarBackgroundColor = (Color)Current.Resources[AppConstants.AppBarBackgroundColorKey],
                BarTextColor = Color.White
            };

            MainPage = page;

            //this is to place the toolbar to the bottom on uwp
            MainPage.On<Xamarin.Forms.PlatformConfiguration.Windows>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            Iconize
                .With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                .With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule())
                .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            System.Diagnostics.Debug.WriteLine("OnStart");
            if (Current.Properties.ContainsKey(SleepOccurredOn))
            {
                OnResume();
            }
        }

        protected override void OnSleep()
        {
            System.Diagnostics.Debug.WriteLine("OnSleep");
            // Handle when your app sleeps
            var currentTimer = ViewModelLocator.MainStatic.Timers
                .FirstOrDefault(t => t.CustomTimer?.IsRunning == true);

            if (currentTimer is null)
                return;

            currentTimer.PauseTimer();

            var now = DateTimeOffset.UtcNow;
            Current.Properties.Remove(SleepOccurredOn);
            Current.Properties.Add(SleepOccurredOn, now.ToString());

            ViewModelLocator.WasAppInForeground = true;
        }

        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine("OnResume");
            // Handle when your app resumes
            if (!Current.Properties.ContainsKey(SleepOccurredOn))
            {
                return;
            }
            var now = DateTimeOffset.UtcNow;
            var sleepOn = DateTimeOffset.Parse(Current.Properties[SleepOccurredOn] as string);
            var diff = (int)now.Subtract(sleepOn).TotalSeconds;

            Current.Properties.Remove(SleepOccurredOn);

            var currentTimer = ViewModelLocator.MainStatic.Timers
                .FirstOrDefault(t => t.CustomTimer?.IsPaused == true);

            if (currentTimer is null)
            {
                return;
            }

            //we slept too much...
            if (diff >= currentTimer.RemainingTime)
            {
                currentTimer.StopTimer();
                return;
            }

            currentTimer.UpdateElapsedTime(diff);
            //if we can resume a timer...
            if (currentTimer.Intervals.Any(i => i.IsRunning))
            {
                currentTimer.PauseTimer();
            }
            //we cant resume a timer, so lets stop it
            else
            {
                currentTimer.StopTimer();
            }
        }
    }
}
