using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Newtonsoft.Json;
using System;
using TimeIt.Droid.Background;
using TimeIt.Droid.Implementations;
using TimeIt.Droid.Models;
using TimeIt.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace TimeIt.Droid.Implementations
{
    public class NotificationService : INotificationService
    {
        private string PackageName
            => Application.Context.PackageName;
        private string ChannelId
            => $"{PackageName}.general";
        private NotificationManager NotifManager
            => (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

        public void Cancel(int id)
        {
            var intent = CreateSchedulerIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                0,
                intent,
                PendingIntentFlags.CancelCurrent);

            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }

        public void Show(string title, string body, string soundNotificationPath = null)
        {
            Show(0, title, body, soundNotificationPath);
        }

        public void Show(int id, string title, string body, string soundNotificationPath = null)
        {
            var appIcon = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.appIcon);
            Bitmap bm = Bitmap.CreateScaledBitmap(appIcon,
               48,
               48,
               true);
            var builder = new Notification.Builder(Application.Context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetSubText("This is a subtext")
                .SetSmallIcon(Resource.Drawable.appIcon)
                .SetColor(Color.Red.ToArgb())
                .SetLargeIcon(bm);

            var soundUri = !string.IsNullOrEmpty(soundNotificationPath)
                ? Android.Net.Uri.Parse(soundNotificationPath)
                : null;

            var audioAttributes = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.Alarm)
                .SetLegacyStreamType(Stream.Alarm)
                .Build();

            if (!string.IsNullOrEmpty(soundNotificationPath))
            {
                //This are deprecated for android O +
#pragma warning disable CS0618 // Type or member is obsolete
                builder.SetSound(soundUri, audioAttributes);
                builder.SetPriority(NotificationCompat.PriorityDefault);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "General", NotificationImportance.Default);

                if (!string.IsNullOrEmpty(soundNotificationPath))
                    channel.SetSound(soundUri, audioAttributes);

                NotifManager.CreateNotificationChannel(channel);
            }


            var bundle = new Bundle();
            bundle.PutString(nameof(NotificationService), $"{id}");

            var resultIntent = GetLauncherActivity();
            //resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            //resultIntent.SetAction(Intent.ActionMain);
            //resultIntent.AddCategory(Intent.CategoryLauncher);
            //resultIntent.SetFlags(ActivityFlags.SingleTop);
            //resultIntent.SetFlags(ActivityFlags.NewTask);

            //var pendingIntent = PendingIntent.GetActivity(Application.Context, 1, resultIntent, PendingIntentFlags.UpdateCurrent);
            var pendingIntent = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context)
                .AddNextIntent(resultIntent)
                .GetPendingIntent(1, (int)PendingIntentFlags.UpdateCurrent, bundle);
            builder.SetContentIntent(pendingIntent);
            var notification = builder.Build();

            NotifManager.Notify(id, notification);
        }

        public void Show(
            string title,
            string body,
            int id,
            DateTime deliveryOn,
            string soundNotificationPath = null)
        {
            var now = DateTime.Now;
            bool showNow = deliveryOn <= now;

            if (showNow)
            {
                Show(id, title, body, soundNotificationPath);
                return;
            }

            var intent = CreateSchedulerIntent(id);
            var localNotification = new LocalNotification
            {
                Title = title,
                Body = body,
                Id = id,
                IconId = Resource.Drawable.appIcon
            };
            var serializedNotification = JsonConvert.SerializeObject(localNotification);
            intent.PutExtra(SchedulerReceiver.LocalNotificationKey, serializedNotification);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
            var future = (long)(deliveryOn - now).TotalMilliseconds;
            var milis = Java.Lang.JavaSystem.CurrentTimeMillis() + future;
            var alarmManager = GetAlarmManager();


            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                alarmManager.SetAlarmClock(new AlarmManager.AlarmClockInfo(milis, pendingIntent), pendingIntent);
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                alarmManager.SetExact(AlarmType.RtcWakeup, milis, pendingIntent);
            else
                alarmManager.Set(AlarmType.RtcWakeup, milis, pendingIntent);
        }

        public Intent GetLauncherActivity()
        {
            return Application.Context.PackageManager.GetLaunchIntentForPackage(PackageName);
        }

        private Intent CreateSchedulerIntent(int id)
        {
            return new Intent(Application.Context, typeof(SchedulerReceiver))
                .SetAction("LocalNotifierIntent" + id);
        }

        private AlarmManager GetAlarmManager()
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            return alarmManager;
        }
    }
}