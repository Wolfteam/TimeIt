using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Linq;
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

            var soundUri = !string.IsNullOrEmpty(soundNotificationPath)
                ? Android.Net.Uri.Parse(soundNotificationPath)
                : RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var audioAttributes = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.Alarm)
                .SetLegacyStreamType(Stream.Alarm)
                .Build();

            string channelId = ChannelId;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                bool createNewChannel = true;
                var existingChannel = NotifManager.NotificationChannels.FirstOrDefault();
                if (existingChannel != null)
                {
                    bool hasDiffSounud = existingChannel.Sound.EncodedSchemeSpecificPart != soundUri.EncodedSchemeSpecificPart;
                    if (hasDiffSounud)
                    {
                        NotifManager.DeleteNotificationChannel(existingChannel.Id);
                    }
                    else
                    {
                        channelId = existingChannel.Id;
                        createNewChannel = false;
                    }
                }
                //Once a notif. channel is created, you cannot edid it via code...
                //so you have to create a new one in order to set a diff sound
                //the problem with this approach is that in the app notif., it will
                //appear a msg that says that a category was deleted
                if (createNewChannel)
                {
                    channelId = ChannelId + UUID.RandomUUID().ToString();
                    var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
                    channel.SetSound(soundUri, audioAttributes);
                    channel.EnableLights(true);
                    channel.LightColor = Resource.Color.colorAccent;
                    NotifManager.CreateNotificationChannel(channel);
                }
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

            var builder = new Notification.Builder(Application.Context, channelId)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.appIcon)
                .SetColor(Color.Red.ToArgb())
                .SetLargeIcon(bm)
                .SetContentIntent(pendingIntent);

            if (!string.IsNullOrEmpty(soundNotificationPath) &&
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
                IconId = Resource.Drawable.appIcon,
                SoundPath = soundNotificationPath
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