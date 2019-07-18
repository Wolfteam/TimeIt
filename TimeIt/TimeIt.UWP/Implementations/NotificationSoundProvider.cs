using System;
using System.Collections.Generic;
using System.IO;
using TimeIt.Enums;
using TimeIt.Helpers;
using TimeIt.Interfaces;
using TimeIt.UWP.Implementations;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationSoundProvider))]
namespace TimeIt.UWP.Implementations
{
    public class NotificationSoundProvider : INotificationSoundProvider
    {
        private string BaseSoundPath
            => $"ms-appx:///Assets/Sounds";

        private IReadOnlyDictionary<SoundType, string> SoundPaths => new Dictionary<SoundType, string>
        {
            { SoundType.TEN_SECONDS, $"{BaseSoundPath}/countdown_10.wav" },
            { SoundType.NINE_SECONDS, $"{BaseSoundPath}/countdown_09.wav" },
            { SoundType.EIGHT_SECONDS, $"{BaseSoundPath}/countdown_08.wav" },
            { SoundType.SEVEN_SECONDS, $"{BaseSoundPath}/countdown_07.wav" },
            { SoundType.SIX_SECONDS, $"{BaseSoundPath}/countdown_06.wav" },
            { SoundType.FIVE_SECONDS, $"{BaseSoundPath}/countdown_05.wav" },
            { SoundType.FOUR_SECONDS, $"{BaseSoundPath}/countdown_04.wav" },
            { SoundType.THREE_SECONDS, $"{BaseSoundPath}/countdown_03.wav" }
        };

        public string GetSoundPath(SoundType soundType)
        {
            if (!SoundPaths.ContainsKey(soundType))
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, "The provided sound type does not exists");
            return SoundPaths[soundType];
        }

        public Stream GetSoundStream(SoundType soundType)
        {
            throw new NotImplementedException();
        }

        public void Play(SoundType soundType, int volume = 100)
        {
            throw new NotImplementedException();
        }

        public void Play(Stream soundStream, int volume = 100, Action onComplete = null)
        {
            throw new NotImplementedException();
        }

        public void Play(string soundPath, int volume = 100, Action onComplete = null)
        {
            throw new NotImplementedException();
        }
    }
}
