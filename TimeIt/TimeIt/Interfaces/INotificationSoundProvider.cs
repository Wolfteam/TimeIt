using System.IO;
using TimeIt.Enums;

namespace TimeIt.Interfaces
{
    public interface INotificationSoundProvider
    {
        string GetSoundPath(CountdownSoundType soundType);
        Stream GetSoundStream(CountdownSoundType soundType);
        void Play(CountdownSoundType soundType);
    }
}
