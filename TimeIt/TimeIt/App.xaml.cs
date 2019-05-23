using Newtonsoft.Json;
using Plugin.Iconize;
using System;
using System.Linq;
using TimeIt.Helpers;
using TimeIt.Models;
using TimeIt.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TimeIt
{
    public partial class App : Application
    {
        /// <summary>
        /// Key to save / retrieve the timer object that was running
        /// </summary>
        private const string TimerOnSleep = "TimerOnSleep";

        /// <summary>
        /// Used to indicate if a android intent launched the app
        /// </summary>
        public static bool WasLaunchedFromIntent;

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
            System.Diagnostics.Debug.WriteLine("------OnStart");

            if (WasLaunchedFromIntent)
            {
                return;
            }

            if (TimerWasRunning())
            {
                OnResume();
            }
        }

        protected override void OnSleep()
        {
            System.Diagnostics.Debug.WriteLine("------OnSleep");
            if (WasLaunchedFromIntent)
            {
                WasLaunchedFromIntent = false;
                return;
            }

            ViewModelLocator.WasAppInForeground = true;

            // Handle when your app sleeps
            var currentTimer = ViewModelLocator.MainStatic.CurrentRunningTimer;
            if (currentTimer is null)
                return;

            currentTimer.PauseTimer();

            CleanProperties();

            var interval = currentTimer.Intervals.First(i => i.IsRunning);
            SetProperties(
                currentTimer.TimerID, 
                currentTimer.ElapsedRepetitions, 
                currentTimer.RemainingRepetitions, 
                currentTimer.ElapsedTime, 
                interval.IntervalID, 
                interval.TimeLeft);
        }

        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine("------OnResume");
            // Handle when your app resumes
            if (!TimerWasRunning())
            {
                return;
            }
            var serialized = Current.Properties[TimerOnSleep] as string;
            var timer = JsonConvert.DeserializeObject<TimerOnSleep>(serialized);

            CleanProperties();

            var currentTimer = ViewModelLocator.MainStatic.Timers
                .FirstOrDefault(t => t.CustomTimer?.IsPaused == true || t.TimerID == timer.TimerID);

            //this heppens when the app was close / killed
            if (currentTimer is null)
            {
                ViewModelLocator.TimerOnSleep = timer;
                ViewModelLocator.WasAppInForeground = true;
                return;
            }

            currentTimer.OnResume(timer);
        }

        private void SetProperties(
            int timerID,
            int elapsedRepetitions,
            int remainingRepetitions,
            float elapsedTime,
            int intervalID,
            float intervalTimeLeft)
        {
            var timer = new TimerOnSleep
            {
                TimerID = timerID,
                RemainingRepetitions = remainingRepetitions,
                ElapsedRepetitions = elapsedRepetitions,
                ElapsedTime = elapsedTime,
                IntervalID = intervalID,
                IntervalTimeLeft = intervalTimeLeft,
                SleepOccurredOn = DateTimeOffset.UtcNow
            };

            var serialized = JsonConvert.SerializeObject(timer);

            Current.Properties.Add(TimerOnSleep, serialized);
        }

        private void CleanProperties()
        {
            Current.Properties.Remove(TimerOnSleep);
        }

        private bool TimerWasRunning()
        {
            return Current.Properties.ContainsKey(TimerOnSleep);
        }
    }
}
