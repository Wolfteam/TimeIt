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

        private IReadOnlyDictionary<SoundType, (int, string)> SoundPaths
            => new Dictionary<SoundType, (int, string)>
        {
            { SoundType.TEN_SECONDS, (Resource.Raw.countdown_10, $"{BaseSoundPath}/{Resource.Raw.countdown_10}") },
            { SoundType.NINE_SECONDS, (Resource.Raw.countdown_09, $"{BaseSoundPath}/{Resource.Raw.countdown_09}") },
            { SoundType.EIGHT_SECONDS, (Resource.Raw.countdown_08, $"{BaseSoundPath}/{Resource.Raw.countdown_08}") },
            { SoundType.SEVEN_SECONDS, (Resource.Raw.countdown_07, $"{BaseSoundPath}/{Resource.Raw.countdown_07}") },
            { SoundType.SIX_SECONDS, (Resource.Raw.countdown_06, $"{BaseSoundPath}/{Resource.Raw.countdown_06}") },
            { SoundType.FIVE_SECONDS, (Resource.Raw.countdown_05, $"{BaseSoundPath}/{Resource.Raw.countdown_05}") },
            { SoundType.FOUR_SECONDS, (Resource.Raw.countdown_04, $"{BaseSoundPath}/{Resource.Raw.countdown_04}") },
            { SoundType.THREE_SECONDS, (Resource.Raw.countdown_03, $"{BaseSoundPath}/{Resource.Raw.countdown_03}") }
        };


        public string GetSoundPath(SoundType soundType)
        {
            if (!SoundPaths.ContainsKey(soundType))
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, "The provided sound type does not exists");
            return SoundPaths[soundType].Item2;
        }

        public Stream GetSoundStream(SoundType soundType)
        {
            int resourceId = SoundPaths[soundType].Item1;
            return Application.Context.Resources.OpenRawResource(resourceId);
        }

        public void Play(SoundType soundType, int volume = 100)
        {
            using (var stream = GetSoundStream(soundType))
            {
                SoundHelper.PlaySound(stream, volume);
            }
        }

        public void Play(Stream soundStream, int volume = 100, Action onComplete = null)
        {
            using (soundStream)
            {
                SoundHelper.PlaySound(soundStream, volume, onComplete);
            }
        }

        public void Play(string soundPath, int volume = 100, Action onComplete = null)
        {
            SoundHelper.PlaySound(soundPath, volume, onComplete);
        }
    }
}