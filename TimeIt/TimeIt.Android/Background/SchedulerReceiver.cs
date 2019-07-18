using Android.Content;
using Newtonsoft.Json;
using System;
using TimeIt.Droid.Models;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.Droid.Background
{
    [BroadcastReceiver(Enabled = true, Label = "Notification Broadcast Receiver")]
    public class SchedulerReceiver : BroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";

        public override void OnReceive(Context context, Intent intent)
        {
            string extra = intent.GetStringExtra(LocalNotificationKey);
            var notif = JsonConvert.DeserializeObject<LocalNotification>(extra);
            var notifService = DependencyService.Get<INotificationService>();
            notifService.Show(notif.Title, notif.Body, notif.Id, DateTime.Now, notif.SoundType);
        }
    }
}