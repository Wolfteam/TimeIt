using Android.App;
using System;
using System.Collections.Generic;
using System.IO;
using TimeIt.Droid.Implementations;
using TimeIt.Enums;
using TimeIt.Helpers;
using TimeIt.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationSoundProvider))]
namespace TimeIt.Droid.Implementations
{
    public class NotificationSoundProvider : INotificationSoundProvider
    {
        private string PackageName
            => Application.Context.PackageName;

        private string BaseSoundPath
            => $"android.resource://{PackageName}";

        private IReadOnlyDictionary<CountdownSoundType, (int, string)> SoundPaths
            => new Dictionary<CountdownSoundType, (int, string)>
        {
            { CountdownSoundType.TEN_SECONDS, (Resource.Raw.countdown_10, $"{BaseSoundPath}/{Resource.Raw.countdown_10}") },
            { CountdownSoundType.NINE_SECONDS, (Resource.Raw.countdown_09, $"{BaseSoundPath}/{Resource.Raw.countdown_09}") },
            { CountdownSoundType.EIGHT_SECONDS, (Resource.Raw.countdown_08, $"{BaseSoundPath}/{Resource.Raw.countdown_08}") },
            { CountdownSoundType.SEVEN_SECONDS, (Resource.Raw.countdown_07, $"{BaseSoundPath}/{Resource.Raw.countdown_07}") },
            { CountdownSoundType.SIX_SECONDS, (Resource.Raw.countdown_06, $"{BaseSoundPath}/{Resource.Raw.countdown_06}") },
            { CountdownSoundType.FIVE_SECONDS, (Resource.Raw.countdown_05, $"{BaseSoundPath}/{Resource.Raw.countdown_05}") },
            { CountdownSoundType.FOUR_SECONDS, (Resource.Raw.countdown_04, $"{BaseSoundPath}/{Resource.Raw.countdown_04}") },
            { CountdownSoundType.THREE_SECONDS, (Resource.Raw.countdown_03, $"{BaseSoundPath}/{Resource.Raw.countdown_03}") }
        };


        public string GetSoundPath(CountdownSoundType soundType)
        {
            if (!SoundPaths.ContainsKey(soundType))
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, "The provided sound type does not exists");
            return SoundPaths[soundType].Item2;
        }

        public Stream GetSoundStream(CountdownSoundType soundType)
        {
            int resourceId = SoundPaths[soundType].Item1;
            return Application.Context.Resources.OpenRawResource(resourceId);
        }

        public void Play(CountdownSoundType soundType, int volume = 100)
        {
            using (var stream = GetSoundStream(soundType))
            {
                SoundHelper.PlaySound(stream, volume);
            }
        }
    }
}