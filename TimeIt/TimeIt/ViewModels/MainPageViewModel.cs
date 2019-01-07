using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TimeIt.Delegates;
using TimeIt.Enums;
using TimeIt.Helpers;

namespace TimeIt.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Members
        private readonly INavigationService _navigationService;
        private string _totalTimeText;
        private string _elapsedTimeText;
        private bool _startButtonEnabled = true;
        private TimerItemViewModel _timer;

        private const float _fps = 1f;
        private const string _timeSpanFormat = "hh\\:mm\\:ss";

        public CustomTimer customTimer;
        public bool requestReDraw;
        public bool navigated;
        #endregion

        #region Properties
        public string TotalTimeText
        {
            get => _totalTimeText;
            set => Set(ref _totalTimeText, value);
        }

        public string ElapsedTimeText
        {
            get => _elapsedTimeText;
            set => Set(ref _elapsedTimeText, value);
        }

        public bool StartButtonEnabled
        {
            get => _startButtonEnabled;
            set => Set(ref _startButtonEnabled, value);
        }

        public TimerItemViewModel Timer
        {
            get => _timer;
            set => Set(ref _timer, value);
        }
        #endregion

        #region Events
        public InvalidateSurfaceEvent InvalidateSurfaceEvent { get; set; }
        #endregion


        #region Commands
        public ICommand AddTimerCommand { get; set; }

        public ICommand OpenSettingsCommand { get; set; }

        public ICommand StartTimerCommand { get; set; }

        public ICommand PauseTimerCommand { get; set; }

        public ICommand StopTimerCommand { get; set; }
        #endregion


        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            //TODO: Load a default timer in case there are not any saved
            Timer = new TimerItemViewModel
            {
                Intervals = new ObservableCollection<IntervalItemViewModel>
                {
                    new IntervalItemViewModel
                    {
                        Duration = 6,
                        Color = "#58ff00",
                        Name = "Workout",
                        Position = 1
                    },
                    new IntervalItemViewModel
                    {
                        Duration = 3,
                        Color = "#0c00ff",
                        Name = "Rest you motherfucker",
                        Position = 3
                    },
                    new IntervalItemViewModel
                    {
                        Duration = 7,
                        Color = "#ff0000",
                        Name = "Excercise",
                        Position = 2
                    },
                    new IntervalItemViewModel
                    {
                        Duration = 5,
                        Color = "#feff00",
                        Name = "Intervalo con un nombre muy largo",
                        Position = 4
                    }
                },
                Name = "Tibia",
                Repetitions = 5
            };

            foreach (var interval in Timer.Intervals)
            {
                interval.TimeLeft = interval.Duration;
            }

            TotalTimeText = TimeSpan.FromSeconds(Timer.TotalTime).ToString(_timeSpanFormat);
            ElapsedTimeText = TimeSpan.FromSeconds(0).ToString(_timeSpanFormat);

            AddTimerCommand = new RelayCommand(() =>
            {
                System.Diagnostics.Debug.WriteLine("Navigating to x page");
                _navigationService.NavigateTo($"{AppPages.ADD_TIMER}");
                requestReDraw = true;
                navigated = true;
            });

            OpenSettingsCommand = new RelayCommand
                (() => _navigationService.NavigateTo($"{AppPages.SETTINGS}"));

            StartTimerCommand = new RelayCommand(StartTimer);

            PauseTimerCommand = new RelayCommand(PauseTimer);

            StopTimerCommand = new RelayCommand(StopTimer);
        }


        public void StartTimer()
        {
            //just a sanity check
            if (customTimer?.IsRunning == true || customTimer?.IsPaused == true)
                return;
            foreach (var interval in Timer.Intervals)
            {
                if (interval.TimeLeft <= 0)
                    interval.TimeLeft = interval.Duration;
                if (interval.Position == 1)
                    interval.IsRunning = true;
            }
            TotalTimeText = TimeSpan.FromSeconds(Timer.TotalTime).ToString(_timeSpanFormat);
            customTimer = new CustomTimer(TimeSpan.FromSeconds(_fps), () =>
            {
                UpdateCurrentInterval();
                InvalidateSurfaceEvent.Invoke();
            });

            customTimer.Start();
            StartButtonEnabled = false;
        }

        public void PauseTimer()
        {
            if (customTimer is null)
                return;

            if (customTimer.IsRunning)
            {
                customTimer.Stop();
                customTimer.IsPaused = true;
            }
            else
            {
                customTimer.Start();
                customTimer.IsPaused = false;
            }
        }

        public void StopTimer()
        {
            if (customTimer is null)
                return;

            customTimer.Stop();
            customTimer.IsPaused = false;

            foreach (var interval in Timer.Intervals)
            {
                interval.IsRunning = false;
                interval.TimeLeft = interval.Duration;
            }
            InvalidateSurfaceEvent.Invoke();
            StartButtonEnabled = true;
            Timer.ElapsedRepetitions = 0;
            ElapsedTimeText = TimeSpan.FromSeconds(0).ToString(_timeSpanFormat);
        }

        public void UpdateCurrentInterval()
        {
            var currentInterval = Timer.Intervals.FirstOrDefault(t => t.IsRunning);
            if (currentInterval is null)
                throw new NullReferenceException("There arent 0 running intervals");
            System.Diagnostics.Debug.WriteLine("--------------Updating current interval started");
            System.Diagnostics.Debug.WriteLine($"Interval = {currentInterval.Name}, time left = {currentInterval.TimeLeft}");

            if (currentInterval.TimeLeft <= 0)
            {
                currentInterval.IsRunning = false;
                var nextInterval = Timer.Intervals.FirstOrDefault(t => t.Position == currentInterval.Position + 1);
                if (nextInterval is null)
                {
                    if (Timer.Repetitions == Timer.ElapsedRepetitions + 1)
                    {
                        Timer.ElapsedRepetitions = 0;
                        StopTimer();
                    }
                    else
                    {
                        Timer.ElapsedRepetitions++;
                        foreach (var interval in Timer.Intervals)
                        {
                            interval.IsRunning = false;
                            interval.TimeLeft = interval.Duration;
                            if (interval.Position == 1)
                                interval.IsRunning = true;
                        }
                        requestReDraw = true;
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
            ElapsedTimeText = TimeSpan.FromSeconds(Timer.ElapsedTime).ToString(_timeSpanFormat);
            System.Diagnostics.Debug.WriteLine("--------------Updating current interval completed");
        }

        public float GetTimerCycleTotalTime()
        {
            return Timer.Intervals.Sum(i => i.Duration);
        }

        public float GetTimerCycleTotalElapsedTime()
        {
            float totalElapsedTime = Timer.Intervals.Sum(t => t.ElapsedTime);
            return totalElapsedTime;
        }

        public float CalculateAngle(float time, float totalTime)
        {
            float intervalAngle = time * 360f / totalTime;
            return intervalAngle;
        }

        public bool IsDarkTheme() => true;
    }
}
