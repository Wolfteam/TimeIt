﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TimeIt.Enums;
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
        #endregion

        #region Properties
        public List<NotificationType> NotificationTypes { get; set; } = new List<NotificationType>
        {
            NotificationType.VOICE,
            NotificationType.TOAST,
            NotificationType.BOTH
        };

        public NotificationType CurrentNotificationType
        {
            get => _appSettings.CurrentNotificationType;
            set => _appSettings.CurrentNotificationType = value;
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
            get => _appSettings.Volumne;
            set
            {
                _appSettings.Volumne = value;
                RaisePropertyChanged(() => NotificationVolume);
            }
        }

        public string AppName
            => AppInfo.Name;

        public string AppVersion
            => AppInfo.VersionString;
        #endregion

        public ICommand SecondsBeforeIntervalEndsCommand { get; private set; }
        public ICommand ChangeVolumeCommand { get; private set; }
        public ICommand OpenGithubCommand { get; private set; }

        public SettingsPageViewModel(IAppSettingsService appSettings, ICustomDialogService dialogService)
        {
            _appSettings = appSettings;
            _dialogService = dialogService;
            SetCommands();
        }

        private void SetCommands()
        {
            SecondsBeforeIntervalEndsCommand = new RelayCommand(async () =>
            {
                var result = await _dialogService
                    .ShowSliderDialogAsync("Seconds before interval ends", _appSettings.SecondsBeforeIntervalEnds, 1, 5);

                if (result is null || result == _appSettings.SecondsBeforeIntervalEnds)
                    return;

                SecondsBeforeIntervalEnds = (int)result.Value;
            });

            ChangeVolumeCommand = new RelayCommand(async () =>
            {
                var result = await _dialogService
                    .ShowSliderDialogAsync("Notification volume", _appSettings.Volumne);

                if (result is null || result == _appSettings.Volumne)
                    return;

                NotificationVolume = (int)result.Value;
            });

            OpenGithubCommand = new RelayCommand<string>
                ((url) => Device.OpenUri(new Uri(url)));
        }
    }
}
