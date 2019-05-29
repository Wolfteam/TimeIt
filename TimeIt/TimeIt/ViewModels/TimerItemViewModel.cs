using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
        private readonly INotificationService _notificationService;
        private readonly INotificationSoundProvider _notificationSoundProvider;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private int _elapsedRepetitions;
        private readonly List<int> _notificationIds = new List<int>();

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
            IMessenger messenger,
            INotificationService notificationService,
            INotificationSoundProvider notificationSoundProvider,
            IBackgroundTaskService backgroundTaskService)
        {
            _appSettings = appSettings;
            _dialogService = dialogService;
            _messenger = messenger;
            _notificationService = notificationService;
            _notificationSoundProvider = notificationSoundProvider;
            _backgroundTaskService = backgroundTaskService;
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

            //i need to be able to schedule all the notifications
            //this notifications may be of type text - text + voice
            //if i pause the timer, i need to cancel all the scheduled notifications but only
            //when the app is not slept, you could add a parameter to the pause method
            //when the timer calls the stop method, you should cancel all the notifications
            ScheduleNotifications();

            CustomTimer.Start();
        }

        public void PauseTimer()
        {
            if (CustomTimer is null)
                return;

            if (CustomTimer.IsRunning)
            {
                //We only need to cancel / reschedule notific. when the app is running
                if (!ViewModelLocator.WasAppInForeground)
                    CancelScheduledNotifications();
                CustomTimer.Stop();
                CustomTimer.IsPaused = true;
            }
            else
            {
                //We only need to cancel / reschedule notific. when the app is running
                if (!ViewModelLocator.WasAppInForeground)
                    ScheduleNotifications(true);
                CustomTimer.Start();
                CustomTimer.IsPaused = false;
            }
        }

        public void StopTimer()
        {
            CancelScheduledNotifications();
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

        public void ScheduleNotifications(bool reschedule = false)
        {
            if (!_appSettings.AreNotificationsEnabled)
                return;
            int secondsBefore = _appSettings.SecondsBeforeIntervalEnds;
            bool whenStarts = _appSettings.NotifyWhenIntervalStarts;
            bool whenIsAboutToEnd = _appSettings.NotifyWhenIntervalIsAboutToEnd;
            bool whenRepetitionCompletes = _appSettings.NotifyWhenARepetitionCompletes;

            if (!whenStarts && !whenIsAboutToEnd && !whenRepetitionCompletes)
                return;

            string soundPath = _notificationSoundProvider
                .GetSoundPath((CountdownSoundType)secondsBefore);

            float secondsToAdd = 0;
            int rep = reschedule ? RemainingRepetitions : Repetitions;

            System.Diagnostics.Debug.WriteLine($"---Scheduling notifications in {(reschedule ? "Reschedule" : "Normal")} mode...");
            System.Diagnostics.Debug.WriteLine($"---Repetitions = {rep}");

            for (int i = 1; i <= rep; i++)
            {
                var now = DateTime.Now;
                var intervals = reschedule
                    ? Intervals
                        .Where(x => x.Position >= Intervals.First(y => y.IsRunning).Position)
                        .OrderBy(x => x.Position)
                    : Intervals.OrderBy(p => p.Position);

                System.Diagnostics.Debug.WriteLine($"---Current repetition = {i}");

                foreach (var interval in intervals)
                {
                    var secondsToUse = reschedule ? interval.TimeLeft : interval.Duration;

                    if (whenStarts &&
                        ((reschedule && !interval.IsRunning) || !reschedule))
                    {
                        var id = (int)secondsToAdd;
                        System.Diagnostics.Debug.WriteLine($"---Scheduled whenStarts for = {interval.Name} in = {id} seconds");
                        _notificationService.Show(
                            $"{Name} - {interval.Name} started",
                            $"Time left = {TimeSpan.FromSeconds(interval.Duration).ToString(AppConstants.DefaultTimeSpanFormat)}",
                            id,
                            now.AddSeconds(id));

                        _notificationIds.Add(id);
                    }

                    bool allowed = reschedule
                        ? secondsToUse - secondsBefore > 0
                        : secondsToUse - secondsBefore >= 0;

                    if (whenIsAboutToEnd && allowed)
                    {
                        var diff = (int)(secondsToAdd + secondsToUse - secondsBefore);
                        System.Diagnostics.Debug.WriteLine($"---Scheduled whenIsAboutToEnd for = {interval.Name} in = {diff} seconds");
                        _notificationService.Show(
                            $"{Name} - {interval.Name} is about to end",
                            $"{secondsBefore} second(s) left",
                            diff,
                            now.AddSeconds(diff),
                            soundPath);

                        _notificationIds.Add(diff);
                    }

                    secondsToAdd += secondsToUse;
                }

                if (whenRepetitionCompletes)
                {
                    System.Diagnostics.Debug.WriteLine($"---Scheduled whenRepetitionCompletes in = {secondsToAdd} seconds");
                    _notificationService.Show(
                        $"Repetition completed",
                        $"Repetition number = {i} completed for timer = {Name}",
                        (int)secondsToAdd,
                        now.AddSeconds(secondsToAdd));
                    _notificationIds.Add((int)secondsToAdd);
                }

                //the things that depends on this variable are only needed once
                reschedule = false;
            }
        }

        public void CancelScheduledNotifications()
        {
            _backgroundTaskService.CancelAllSoundNotification();

            foreach (int id in _notificationIds)
            {
                _notificationService.Cancel(id);
            }
            _notificationIds.Clear();

            //Here we try to cancel every possible case,
            //note that this is for the case when the app was killed
            float secondsToAdd = 0;
            float duration = Intervals.Sum(i => i.Duration);

            for (int i = 1; i <= Repetitions; i++)
            {
                foreach (var interval in Intervals)
                {
                    float secondsBefore = secondsToAdd + interval.Duration - _appSettings.SecondsBeforeIntervalEnds;

                    _notificationService.Cancel((int)secondsToAdd);
                    _notificationService.Cancel((int)secondsBefore);
                    secondsToAdd += interval.Duration;
                }
                _notificationService.Cancel((int)(duration * i));
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

            if (currentInterval.TimeLeft <= 0)
            {
                currentInterval.IsRunning = false;
                var nextInterval = Intervals.FirstOrDefault(t => t.Position == currentInterval.Position + 1);
                if (nextInterval is null)
                {
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
            if (elapsedSeconds >= TotalTime - timer.ElapsedTime)
            {
                ElapsedRepetitions = timer.ElapsedRepetitions;
                return;
            }

            var currentInterval = Intervals.First(i => i.IntervalID == timer.IntervalID);
            currentInterval.TimeLeft = timer.IntervalTimeLeft;
            bool firstTime = true;

            for (int i = 0; i <= timer.RemainingRepetitions; i++)
            {
                if (elapsedSeconds <= 0)
                    break;

                var intervalsToUpdate = firstTime
                    ? Intervals.Where(x => x.Position >= currentInterval.Position).OrderBy(x => x.Position)
                    : Intervals.OrderBy(x => x.Position);

                if (firstTime)
                {
                    foreach (var interval in Intervals.Where(x => x.Position < currentInterval.Position))
                    {
                        interval.IsRunning = false;
                        interval.TimeLeft = 0;
                    }
                }

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

                firstTime = false;
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

        //TODO: PROVIDE A SERVICE FOR THIS
        public bool IsDarkTheme() => true;

        #endregion
    }
}