using System;
using Android.App;
using Android.Content;
using Android.OS;
using TimeIt.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;
using Color = Android.Graphics.Color;

namespace TimeIt.Droid.Background
{
    [Service]
    public class SoundService : Service
    {
        public const string SoundPath = "SoundPath";
        public const string StartPlayBack = "StartPlayback";
        public const string StopPlayBack = "StopPlayBack";

        private int _notifificationId = 9999;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notifManager = Application.Context.GetSystemService(NotificationService) as NotificationManager;
                var channelId = Implementations.NotificationService.LowChannelId;
                var channel = new NotificationChannel(channelId, "StickyNotifications", NotificationImportance.Low);
                channel.SetSound(null, null);
                notifManager.CreateNotificationChannel(channel);

                var builder =new Notification.Builder(Application.Context, channelId)
                    .SetContentTitle("Playing sound")
                    .SetContentText("Sound")
                    .SetAutoCancel(true)
                    .SetSmallIcon(Resource.Drawable.appIcon)
                    .SetColor(Color.Red.ToArgb());

                StartForeground(_notifificationId, builder.Build());
            }

            var action = intent.Action;
            var soundPath = intent.GetStringExtra(SoundPath);
            if (string.IsNullOrEmpty(soundPath) && 
                !action.Equals(StopPlayBack, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentNullException(nameof(soundPath), "The sound path cannot be null");

            switch (action)
            {
                case StartPlayBack:
                    StartSound(soundPath);
                    break;
                case StopPlayBack:
                    StopSound();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "The provided action is not valid");
            }

            // service will not be recreated if abnormally terminated
            return StartCommandResult.NotSticky;
        }


        private void StartSound(string soundPath)
        {
            var soundProvider = DependencyService.Get<INotificationSoundProvider>();
            
            soundProvider.Play(soundPath, onComplete: StopSound);
        }

        private void StopSound()
        {
            StopSelf();
            StopForeground(true);
        }
    }
}