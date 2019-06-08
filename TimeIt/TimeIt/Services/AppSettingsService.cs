using TimeIt.Enums;
using TimeIt.Interfaces;
using Xamarin.Essentials;

namespace TimeIt.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public bool ShowElapsedInsteadOfRemainingTime
        {
            get => Preferences.Get(nameof(ShowElapsedInsteadOfRemainingTime), false);
            set => Preferences.Set(nameof(ShowElapsedInsteadOfRemainingTime), value);
        }

        public NotificationType CurrentNotificationType
        {
            get => (NotificationType)Preferences.Get(nameof(CurrentNotificationType), (int)NotificationType.TOAST);
            set => Preferences.Set(nameof(CurrentNotificationType), (int)value);
        }

        public bool AreNotificationsEnabled
        {
            get => Preferences.Get(nameof(AreNotificationsEnabled), false);
            set => Preferences.Set(nameof(AreNotificationsEnabled), value);
        }

        public bool NotifyWhenIntervalStarts
        {
            get => Preferences.Get(nameof(NotifyWhenIntervalStarts), false);
            set => Preferences.Set(nameof(NotifyWhenIntervalStarts), value);
        }

        public bool NotifyWhenIntervalIsAboutToEnd
        {
            get => Preferences.Get(nameof(NotifyWhenIntervalIsAboutToEnd), false);
            set => Preferences.Set(nameof(NotifyWhenIntervalIsAboutToEnd), value);
        }

        public int SecondsBeforeIntervalEnds
        {
            get => Preferences.Get(nameof(SecondsBeforeIntervalEnds), 3);
            set => Preferences.Set(nameof(SecondsBeforeIntervalEnds), value);
        }

        public bool NotifyWhenARepetitionCompletes
        {
            get => Preferences.Get(nameof(NotifyWhenARepetitionCompletes), false);
            set => Preferences.Set(nameof(NotifyWhenARepetitionCompletes), value);
        }

        public int Volume
        {
            get => Preferences.Get(nameof(Volume), 70);
            set => Preferences.Set(nameof(Volume), value);
        }

        public bool ToastWithSound
        {
            get => Preferences.Get(nameof(ToastWithSound), false);
            set => Preferences.Set(nameof(ToastWithSound), value);
        }
    }
}
