using AndroidX.Work;
using System;
using TimeIt.Droid.Background;
using TimeIt.Droid.Implementations;
using TimeIt.Enums;
using TimeIt.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(BackgroundTaskService))]
namespace TimeIt.Droid.Implementations
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private const string SoundNotifTag = "SoundNotification";

        public void CancelAllSoundNotification()
        {
            WorkManager.Instance.CancelAllWorkByTag(SoundNotifTag);
        }

        public void ScheduleSoundNotification(CountdownSoundType soundType, DateTime deliveryOn, int volume = 100)
        {
            var now = DateTime.Now;
            var diff = (long)deliveryOn.Subtract(now).TotalMilliseconds;
            var data = new Data.Builder()
                .PutInt(SoundWorker.SoundTypeKey, (int)soundType)
                .PutInt(SoundWorker.SoundVolumeKey, volume)
                .Build();

            var soundWorkerRequest = new OneTimeWorkRequest.Builder(typeof(SoundWorker))
                .SetInputData(data)
                .SetInitialDelay(diff, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .AddTag(SoundNotifTag)
                .Build();

            WorkManager.Instance.Enqueue(soundWorkerRequest);
        }
    }
}