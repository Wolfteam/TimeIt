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
using TimeIt.Enums;
using TimeIt.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace TimeIt.Droid.Implementations
{
    public class NotificationService : INotificationService
    {
        private static string PackageName
            => Application.Context.PackageName;

        public static string GeneralChannelId
            => $"{PackageName}.general";

        public static string LowChannelId
            => $"{PackageName}.low";

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

        public void Show(string title, string body, SoundType? soundType)
        {
            Show(0, title, body, soundType);
        }

        public void Show(int id, string title, string body, SoundType? soundType)
        {
            var appIcon = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.appIcon);
            Bitmap bm = Bitmap.CreateScaledBitmap(appIcon,
                48,
                48,
                true);
            bool hasSound = soundType.HasValue;
            var soundProvider = Xamarin.Forms.DependencyService.Get<INotificationSoundProvider>();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var generalChannel = new NotificationChannel(GeneralChannelId, "Notifications", NotificationImportance.Default)
                {
                    LightColor = Resource.Color.colorAccent,
                    Description = "App notifications with sound"
                };
                var lowChannel = new NotificationChannel(LowChannelId, "General", NotificationImportance.Low)
                {
                    LightColor = Resource.Color.colorAccent,
                    Description = "App notifications without sound",
                };

                generalChannel.EnableLights(true);
                lowChannel.EnableLights(true);
                lowChannel.SetSound(null, null);

                NotifManager.CreateNotificationChannel(generalChannel);
                NotifManager.CreateNotificationChannel(lowChannel);

                // play sound
                if (hasSound)
                {
                    soundProvider.Play(soundType.Value);
                }
            }

            var soundUri = hasSound
                ? Android.Net.Uri.Parse(soundProvider.GetSoundPath(soundType.Value))
                : RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var audioAttributes = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.Alarm)
                .SetLegacyStreamType(Stream.Alarm)
                .Build();

            //we use the main because we dont want to show the splash again..
            //and because the splash produces a bug where the timer gets paused xd
            var resultIntent = new Intent(Application.Context, typeof(MainActivity));
            //resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            //resultIntent.SetAction(Intent.ActionMain);
            //resultIntent.AddCategory(Intent.CategoryLauncher);
            //resultIntent.SetFlags(ActivityFlags.SingleTop);
            //resultIntent.SetFlags(ActivityFlags.NewTask);

            //var pendingIntent = PendingIntent.GetActivity(Application.Context, 1, resultIntent, PendingIntentFlags.UpdateCurrent);
            var bundle = new Bundle();
            bundle.PutString(nameof(NotificationService), $"{id}");
            var pendingIntent = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context)
                .AddNextIntent(resultIntent)
                .GetPendingIntent(1, (int)PendingIntentFlags.UpdateCurrent, bundle);

            string channelId = hasSound ? LowChannelId : GeneralChannelId;
            var builder = new Notification.Builder(Application.Context, channelId)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.appIcon)
                .SetColor(Color.Red.ToArgb())
                .SetLargeIcon(bm)
                .SetContentIntent(pendingIntent);

            if (hasSound &&
                Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                //This are deprecated for android O +
#pragma warning disable CS0618 // Type or member is obsolete
                builder.SetSound(soundUri, audioAttributes);
                builder.SetPriority(NotificationCompat.PriorityDefault);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            var notification = builder.Build();

            NotifManager.Notify(id, notification);
        }

        public void Show(
            string title,
            string body,
            int id,
            DateTime deliveryOn,
            SoundType? soundType)
        {
            var now = DateTime.Now;
            bool showNow = deliveryOn <= now;

            if (showNow)
            {
                Show(id, title, body, soundType);
                return;
            }

            var intent = CreateSchedulerIntent(id);
            var localNotification = new LocalNotification
            {
                Title = title,
                Body = body,
                Id = id,
                IconId = Resource.Drawable.appIcon,
                SoundType = soundType
            };
            var serializedNotification = JsonConvert.SerializeObject(localNotification);
            intent.PutExtra(SchedulerReceiver.LocalNotificationKey, serializedNotification);

            var pendingIntent =
                PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
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