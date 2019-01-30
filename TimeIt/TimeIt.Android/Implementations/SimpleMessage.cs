
using Android.App;
using Android.Widget;
using TimeIt.Droid.Implementations;
using TimeIt.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(SimpleMessage))]
namespace TimeIt.Droid.Implementations
{
    public class SimpleMessage : ISimpleMessage
    {
        public void ShowMessage(string message, bool longDelay = false)
        {
            Toast.MakeText(Application.Context, message, longDelay ? ToastLength.Long : ToastLength.Short).Show();
        }
    }
}