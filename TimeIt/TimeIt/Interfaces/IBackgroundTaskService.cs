using System;
using TimeIt.Enums;

namespace TimeIt.Interfaces
{
    public interface IBackgroundTaskService
    {
        void CancelAllSoundNotification();
        void ScheduleSoundNotification(CountdownSoundType soundType, DateTime deliveryOn, int volume = 100);
    }
}
