using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TimeIt.Delegates;
using TimeIt.Enums;
using TimeIt.Helpers;
using TimeIt.Interfaces;
using TimeIt.Models;

namespace TimeIt.ViewModels
{
    public class TimerItemViewModel
    {
        #region Fields
        private readonly IAppSettingsService _appSettings;
        private readonly ICustomDialogService _dialogService;
        private readonly IMessenger _messenger;
        private int _elapsedRepetitions;

        //TODO: REMOVE THIS TWO AND MOVE IT TO A CONSTANTS FILE
        private const float _fps = 1f;

        public CustomTimer CustomTimer;
        public bool RequestReDraw;
        #endregion

        #region Properties

        public int TimerID { get; set; }

        public string Name { get; set; }

        public int Repetitions { get; set; }

        public int RemainingRepetitions
            => Repetitions - ElapsedRepetitions;

        public int ElapsedRepetitions
        {
            get => _elapsedRepetitions;
            set
            {
                _elapsedRepetitions = value;
                _messenger.Send(RemainingRepetitions, $"{MessageType.MP_REMAINING_REPETITIONS_CHANGED}");
            }
        }

        public float RemainingTime
            => TotalTime - ElapsedTime;

        public float ElapsedTime
        {
            get
            {
                float cet = Intervals?.Sum(i => i.ElapsedTime) ?? 0;
                float ctt = Intervals?.Sum(i => i.Duration) ?? 0;
                return ctt * ElapsedRepetitions + cet;
            }
        }

        public float TotalTime
        {
            get
            {
                float ctt = Intervals?.Sum(i => i.Duration) ?? 0;
                return ctt * Repetitions;
            }
        }

        public ObservableCollection<IntervalItemViewModel> Intervals { get; set; }
            = new ObservableCollection<IntervalItemViewModel>();
        #endregion

        #region Events

        public InvalidateSurfaceEvent InvalidateSurfaceEvent { get; set; }

        #endregion

        public TimerItemViewModel(
            IAppSettingsService appSettings,
            ICustomDialogService dialogService,
            IMessenger messenger)
        {
            _appSettings = appSettings;
            _dialogService = dialogService;
            _messenger = messenger;
            SetCommands();
            RegisterMessages();
        }

        #region Methods
        private void SetCommands()
        {
        }

        private void RegisterMessages()
        {
            _messenger.Register<bool>(
                this,
                $"{MessageType.ON_NAVIGATED_BACK}",
                OnNavigatedBack);
        }

        public void UnregisterMessages()
        {
            _messenger.Unregister<bool>(
                this,
                $"{MessageType.ON_NAVIGATED_BACK}",
                OnNavigatedBack);
        }

        public void StartTimer()
        {
            //just a sanity check
            if (CustomTimer?.IsRunning == true || CustomTimer?.IsPaused == true)
                return;

            ResetIntervals(true);

            _messenger.Send(TotalTime, $"{MessageType.MP_TOTAL_TIME_CHANGED}");
            _messenger.Send(false, $"{MessageType.MP_START_BUTTON_IS_ENABLED}");

            InitCustomTimer();

            CustomTimer.Start();
            //i need to be able to schedule all the notifications
            //this notifications may be of type text - text + voice
            //if i pause the timer, i need to cancel all the scheduled notifications but only
            //when the app is not slept, you could add a parameter to the pause method
            //when the timer calls the stop method, you should cancel all the notifications
        }

        public void PauseTimer()
        {
            if (CustomTimer is null)
                return;

            if (CustomTimer.IsRunning)
            {
                CustomTimer.Stop();
                CustomTimer.IsPaused = true;
            }
            else
            {
                CustomTimer.Start();
                CustomTimer.IsPaused = false;
            }
        }

        public void StopTimer()
        {
            if (CustomTimer is null)
                return;

            CustomTimer.Stop();
            CustomTimer.IsPaused = false;

            ResetIntervals();

            InvalidateSurfaceEvent?.Invoke();
            _messenger.Send(true, $"{MessageType.MP_START_BUTTON_IS_ENABLED}");
            ElapsedRepetitions = 0;
            _messenger.Send(
                _appSettings.ShowElapsedInsteadOfRemainingTime
                    ? 0f
                    : RemainingTime,
                $"{MessageType.MP_ELAPSED_TIME_CHANGED}");
        }

        public void OnResume(TimerOnSleep timer)
        {
            //TODO: THIS CRAP IS BUGGED, SOMETIMES IT SHOWS A NEGATIVE
            //INTERVAL LOL AND REMAINING KEEPS SUMING INSTEAD OF RESTING
            var now = DateTimeOffset.UtcNow;
            var diff = (int)now.Subtract(timer.SleepOccurredOn).TotalSeconds;

            if (diff >= RemainingTime)
            {
                StopTimer();
                return;
            }

            UpdateElapsedTime(timer, diff);

            //if its null, its because the app was killed
            if (CustomTimer is null)
                InitCustomTimer();

            //if we can resume the timer...
            if (Intervals.Any(i => i.IsRunning))
            {
                _messenger.Send(false, $"{MessageType.MP_START_BUTTON_IS_ENABLED}");
                PauseTimer();
            }
            //we cant resume it, so lets stop it
            else
            {
                StopTimer();
            }
        }

        private void InitCustomTimer()
        {
            CustomTimer = new CustomTimer(TimeSpan.FromSeconds(_fps), () =>
            {
                UpdateCurrentInterval();
                InvalidateSurfaceEvent?.Invoke();
            });
        }

        private void UpdateCurrentInterval()
        {
            var currentInterval = Intervals.FirstOrDefault(t => t.IsRunning);
            if (currentInterval is null)
                throw new NullReferenceException("There arent 0 running intervals");
            System.Diagnostics.Debug.WriteLine("Vm - Updating current interval started");
            System.Diagnostics.Debug.WriteLine(
                $"--------------Vm - Interval = {currentInterval.Name}, time left = {currentInterval.TimeLeft}");

            //if we are starting a fresh interval...
            bool intervalStarted = currentInterval.TimeLeft == currentInterval.Duration;
            if (intervalStarted)
                ShowIntervalStartedNotification(currentInterval.Name);

            //if the time left for this interval == seconds before interval ends
            //we must notify it
            if (currentInterval.TimeLeft <= AppConstants.MaxSecondsBeforeIntervalsEnd &&
                currentInterval.TimeLeft - 1 == _appSettings.SecondsBeforeIntervalEnds)
            {
                ShowEndOfIntervalNotification(currentInterval.Name);
            }

            if (currentInterval.TimeLeft <= 0)
            {
                currentInterval.IsRunning = false;
                var nextInterval = Intervals.FirstOrDefault(t => t.Position == currentInterval.Position + 1);
                if (nextInterval is null)
                {
                    //if we completed a repetition...
                    ShowEndOfRepetitionNotification(ElapsedRepetitions + 1);

                    if (Repetitions == ElapsedRepetitions + 1)
                    {
                        ElapsedRepetitions = 0;
                        StopTimer();
                    }
                    else
                    {
                        ElapsedRepetitions++;
                        ResetIntervals(true);

                        RequestReDraw = true;
                    }
                }
                else
                {
                    ShowIntervalStartedNotification(nextInterval.Name);
                    nextInterval.IsRunning = true;
                    nextInterval.TimeLeft -= _fps;
                }
            }
            else
            {
                currentInterval.TimeLeft -= _fps;
            }
            _messenger.Send(
                _appSettings.ShowElapsedInsteadOfRemainingTime
                    ? ElapsedTime
                    : RemainingTime,
                $"{MessageType.MP_ELAPSED_TIME_CHANGED}");
            System.Diagnostics.Debug.WriteLine("Vm - Updating current interval completed");
        }

        public float GetTimerCycleTotalTime()
        {
            return Intervals.Sum(i => i.Duration);
        }

        public float GetTimerCycleTotalElapsedTime()
        {
            float totalElapsedTime = Intervals.Sum(t => t.ElapsedTime);
            return totalElapsedTime;
        }

        public void SetDefaultTimeLeft()
        {
            foreach (var interval in Intervals)
            {
                interval.TimeLeft = interval.Duration;
            }
        }

        public void UpdateElapsedTime(TimerOnSleep timer, float elapsedSeconds)
        {
            var currentInterval = Intervals.FirstOrDefault(i => i.IsRunning || i.IntervalID == timer.IntervalID);
            currentInterval.TimeLeft = timer.IntervalTimeLeft;
            var intervalsToUpdate = Intervals
                .Where(i => i.Position >= currentInterval.Position)
                .OrderBy(i => i.Position);

            if (elapsedSeconds >= TotalTime - timer.ElapsedTime)
            {
                ElapsedRepetitions = timer.ElapsedRepetitions;
                return;
            }

            for (int i = 0; i <= timer.RemainingRepetitions; i++)
            {
                if (elapsedSeconds <= 0)
                    break;

                foreach (var interval in intervalsToUpdate)
                {
                    float diff = interval.TimeLeft - elapsedSeconds;
                    if (diff < 0)
                    {
                        elapsedSeconds -= interval.TimeLeft;
                        interval.IsRunning = false;
                        interval.TimeLeft = 0;
                    }
                    else if (diff == 0)
                    {
                        interval.IsRunning = false;
                        interval.TimeLeft = 0;
                        var nextInterval = Intervals.FirstOrDefault(t => t.Position == interval.Position + 1);
                        if (nextInterval != null)
                        {
                            nextInterval.IsRunning = true;
                            nextInterval.TimeLeft -= _fps;
                        }
                        elapsedSeconds = 0;
                        break;
                    }
                    else
                    {
                        interval.IsRunning = true;
                        interval.TimeLeft -= elapsedSeconds;
                        elapsedSeconds = 0;
                        break;
                    }
                }

                if (elapsedSeconds > 0)
                {
                    timer.ElapsedRepetitions++;
                    ResetIntervals();
                }
            }

            ElapsedRepetitions = timer.ElapsedRepetitions;
        }

        private void ResetIntervals(bool enableFirstOne = false)
        {
            foreach (var interval in Intervals)
            {
                interval.IsRunning = false;
                interval.TimeLeft = interval.Duration;
                if (enableFirstOne && interval.Position == 1)
                    interval.IsRunning = true;
            }
        }

        private void OnNavigatedBack(bool onNavigatedBack)
        {
            if (!onNavigatedBack)
                return;
            RequestReDraw = true;
            InvalidateSurfaceEvent?.Invoke();
        }

        private void ShowIntervalStartedNotification(string intervalName)
        {
            if (_appSettings.AreNotificationsEnabled &&
                _appSettings.NotifyWhenIntervalStarts)
            {
                _dialogService.ShowNotification("Interval started", $"{intervalName} has just started.");
            }
        }

        private void ShowEndOfIntervalNotification(string intervalName)
        {
            if (_appSettings.AreNotificationsEnabled &&
                _appSettings.NotifyWhenIntervalIsAboutToEnd)
            {
                _dialogService.ShowNotification("Interval started", $"{intervalName} is about to end.");
            }
        }

        private void ShowEndOfRepetitionNotification(int current)
        {
            if (_appSettings.AreNotificationsEnabled &&
                _appSettings.NotifyWhenARepetitionCompletes)
            {
                _dialogService.ShowNotification("Repetition completed", $"Repetition = {current} completed.");
            }
        }

        //TODO: PROVIDE A SERVICE FOR THIS
        public bool IsDarkTheme() => true;

        #endregion
    }
}