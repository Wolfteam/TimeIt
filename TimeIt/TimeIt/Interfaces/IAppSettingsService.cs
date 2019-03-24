using TimeIt.Enums;

namespace TimeIt.Interfaces
{
    public interface IAppSettingsService
    {
        NotificationType CurrentNotificationType { get; set; }
        bool AreNotificationsEnabled { get; set; }
        bool NotifyWhenIntervalStarts { get; set; }
        int SecondsBeforeIntervalEnds { get; set; }
        bool NotifyWhenIntervalIsAboutToEnd { get; set; }
        bool NotifyWhenARepetitionCompletes { get; set; }
        int Volumne { get; set; }
    }
}
