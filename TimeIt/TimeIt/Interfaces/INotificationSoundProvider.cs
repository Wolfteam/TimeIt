using System;
using System.IO;
using TimeIt.Enums;

namespace TimeIt.Interfaces
{
    public interface INotificationSoundProvider
    {
        string GetSoundPath(SoundType soundType);
        Stream GetSoundStream(SoundType soundType);
        void Play(SoundType soundType, int volume = 100);

        void Play(Stream soundStream, int volume = 100, Action onComplete = null);

        void Play(string soundPath, int volume = 100, Action onComplete = null);
    }
}
