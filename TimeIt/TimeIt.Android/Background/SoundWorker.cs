using Android.Content;
using AndroidX.Work;
using TimeIt.Enums;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.Droid.Background
{
    public class SoundWorker : Worker
    {
        public const string SoundTypeKey = "SoundType";
        public const string SoundVolumeKey = "SoundVolumeKey";

        public SoundWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            var soundType = (CountdownSoundType)InputData.GetInt(SoundTypeKey, 0);
            var volume = InputData.GetInt(SoundVolumeKey, 0);
            var service = DependencyService.Get<INotificationSoundProvider>();
            service.Play(soundType, volume);
            return Result.InvokeSuccess();
        }
    }
}