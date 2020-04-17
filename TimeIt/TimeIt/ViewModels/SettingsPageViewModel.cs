using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TimeIt.Enums;
using TimeIt.Helpers;
using TimeIt.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeIt.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Members
        private readonly IAppSettingsService _appSettings;
        private readonly ICustomDialogService _dialogService;
        private readonly IMessenger _messenger;
        #endregion

        #region Properties
        public List<NotificationType> NotificationTypes { get; private set; }
            = new List<NotificationType>();

        public NotificationType CurrentNotificationType
        {
            get => _appSettings.CurrentNotificationType;
            set
            {
                _appSettings.CurrentNotificationType = value;
                RaisePropertyChanged(() => IsNotificationVolumeVisible);
                RaisePropertyChanged(() => IsToastWithSoundVisible);
            }
        }

        public bool AreNotificationsEnabled
        {
            get => _appSettings.AreNotificationsEnabled;
            set
            {
                _appSettings.AreNotificationsEnabled = value;
                RaisePropertyChanged(() => AreNotificationsEnabled);
            }
        }

        public bool NotifyWhenIntervalStarts
        {
            get => _appSettings.NotifyWhenIntervalStarts;
            set => _appSettings.NotifyWhenIntervalStarts = value;
        }

        public bool NotifyWhenIntervalIsAboutToEnd
        {
            get => _appSettings.NotifyWhenIntervalIsAboutToEnd;
            set
            {
                _appSettings.NotifyWhenIntervalIsAboutToEnd = value;
                RaisePropertyChanged(() => NotifyWhenIntervalIsAboutToEnd);
            }
        }

        public int SecondsBeforeIntervalEnds
        {
            get => _appSettings.SecondsBeforeIntervalEnds;
            set
            {
                _appSettings.SecondsBeforeIntervalEnds = value;
                RaisePropertyChanged(() => SecondsBeforeIntervalEnds);
            }
        }

        public bool NotifyWhenARepetitionCompletes
        {
            get => _appSettings.NotifyWhenARepetitionCompletes;
            set => _appSettings.NotifyWhenARepetitionCompletes = value;
        }

        public int NotificationVolume
        {
            get => _appSettings.Volume;
            set
            {
                _appSettings.Volume = value;
                RaisePropertyChanged(() => NotificationVolume);
            }
        }

        public bool ShowElapsedInsteadOfRemainingTime
        {
            get => _appSettings.ShowElapsedInsteadOfRemainingTime;
            set
            {
                _appSettings.ShowElapsedInsteadOfRemainingTime = value;
                _messenger.Send(value, $"{MessageType.MP_SHOW_ELAPSED_TIME_SETTING_CHANGED}");
            }
        }

        public bool IsNotificationVolumeVisible
            => CurrentNotificationType == NotificationType.VOICE;

        public bool IsToastWithSoundVisible
            => CurrentNotificationType == NotificationType.TOAST;

        public bool ToastWithSound
        {
            get => _appSettings.ToastWithSound;
            set => _appSettings.ToastWithSound = value;
        }

        public string AppName
            => AppInfo.Name;

        public string AppVersion
            => AppInfo.VersionString;
        #endregion

        #region Commands
        public ICommand SecondsBeforeIntervalEndsCommand { get; private set; }
        public ICommand ChangeVolumeCommand { get; private set; }
        public ICommand OpenGithubCommand { get; private set; }
        #endregion

        public SettingsPageViewModel(
            IAppSettingsService appSettings,
            ICustomDialogService dialogService,
            IMessenger messenger)
        {
            _appSettings = appSettings;
            _dialogService = dialogService;
            _messenger = messenger;

            NotificationTypes.Add(NotificationType.TOAST);
            //if (Device.RuntimePlatform != Device.UWP)
            //{
            //    NotificationTypes.Add(NotificationType.VOICE);
            //}

            SetCommands();
        }

        private void SetCommands()
        {
            SecondsBeforeIntervalEndsCommand = new RelayCommand(async () =>
            {
                var result = await _dialogService.ShowSliderDialogAsync(
                    "Seconds before interval ends",
                    _appSettings.SecondsBeforeIntervalEnds,
                    AppConstants.MinSecondsBeforeIntervalsEnd,
                    AppConstants.MaxSecondsBeforeIntervalsEnd);

                if (result is null || result == _appSettings.SecondsBeforeIntervalEnds)
                    return;

                SecondsBeforeIntervalEnds = (int)result.Value;
            });

            ChangeVolumeCommand = new RelayCommand(async () =>
            {
                var result = await _dialogService
                    .ShowSliderDialogAsync("Notification volume", _appSettings.Volume);

                if (result is null || result == _appSettings.Volume)
                    return;

                NotificationVolume = (int)result.Value;
            });

            OpenGithubCommand = new RelayCommand<string>
                (async url => await Launcher.TryOpenAsync(new Uri(url)));
        }
    }
}
