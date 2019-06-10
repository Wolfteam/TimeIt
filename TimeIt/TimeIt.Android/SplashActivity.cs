using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Threading;

namespace TimeIt.Droid
{
    [Activity(Label = "TimeIt", Icon = "@mipmap/ic_launcher", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Thread.Sleep(500);
            StartActivity(typeof(MainActivity));
        }
    }
}