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

        private IReadOnlyDictionary<CountdownSoundType, string> SoundPaths => new Dictionary<CountdownSoundType, string>
        {
            { CountdownSoundType.TEN_SECONDS, $"{BaseSoundPath}/countdown_10.wav" },
            { CountdownSoundType.NINE_SECONDS, $"{BaseSoundPath}/countdown_09.wav" },
            { CountdownSoundType.EIGHT_SECONDS, $"{BaseSoundPath}/countdown_08.wav" },
            { CountdownSoundType.SEVEN_SECONDS, $"{BaseSoundPath}/countdown_07.wav" },
            { CountdownSoundType.SIX_SECONDS, $"{BaseSoundPath}/countdown_06.wav" },
            { CountdownSoundType.FIVE_SECONDS, $"{BaseSoundPath}/countdown_05.wav" },
            { CountdownSoundType.FOUR_SECONDS, $"{BaseSoundPath}/countdown_04.wav" },
            { CountdownSoundType.THREE_SECONDS, $"{BaseSoundPath}/countdown_03.wav" }
        };

        public string GetSoundPath(CountdownSoundType soundType)
        {
            if (!SoundPaths.ContainsKey(soundType))
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, "The provided sound type does not exists");
            return SoundPaths[soundType];
        }

        public Stream GetSoundStream(CountdownSoundType soundType)
        {
            throw new NotImplementedException();
        }

        public void Play(CountdownSoundType soundType, int volume = 100)
        {
            SoundHelper.PlaySound("Sounds/countdown_10.wav", volume);
        }
    }
}
