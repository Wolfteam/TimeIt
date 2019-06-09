using System;
using TimeIt.Enums;
using TimeIt.Interfaces;
using TimeIt.UWP.Implementations;

[assembly: Xamarin.Forms.Dependency(typeof(BackgroundTaskService))]
namespace TimeIt.UWP.Implementations
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        public void CancelAllSoundNotification()
        {

        }

        public void ScheduleSoundNotification(CountdownSoundType soundType, DateTime deliveryOn, int volume = 100)
        {

        }
    }
}
