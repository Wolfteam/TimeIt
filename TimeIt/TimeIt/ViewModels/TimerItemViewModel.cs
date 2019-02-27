using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TimeIt.Delegates;
using TimeIt.Enums;
using TimeIt.Helpers;

namespace TimeIt.ViewModels
{
    public class TimerItemViewModel
    {
        #region Fields

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

        public ICommand PositionChangedCommand { get; private set; }


        public TimerItemViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            SetCommands();
            RegisterMessages();
        }

        private void SetCommands()
        {
            PositionChangedCommand = new RelayCommand<int>
                (newPage => System.Diagnostics.Debug.WriteLine($"Position changed. New position = {newPage}"));
        }

        private void RegisterMessages()
        {
            _messenger.Register<bool>(
                this,
                $"{MessageType.ON_NAVIGATED_BACK}",
                OnNavigatedBack);
        }

        public void StartTimer()
        {
            //just a sanity check
            if (CustomTimer?.IsRunning == true || CustomTimer?.IsPaused == true)
                return;
            foreach (var interval in Intervals)
            {
                if (interval.TimeLeft <= 0)
                    interval.TimeLeft = interval.Duration;
                if (interval.Position == 1)
                    interval.IsRunning = true;
            }

            _messenger.Send(TotalTime, $"{MessageType.MP_TOTAL_TIME_CHANGED}");
            _messenger.Send(false, $"{MessageType.MP_START_BUTTON_IS_ENABLED}");

            CustomTimer = new CustomTimer(TimeSpan.FromSeconds(_fps), () =>
            {
                UpdateCurrentInterval();
                InvalidateSurfaceEvent.Invoke();
            });

            CustomTimer.Start();
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

            foreach (var interval in Intervals)
            {
                interval.IsRunning = false;
                interval.TimeLeft = interval.Duration;
            }

            InvalidateSurfaceEvent.Invoke();
            _messenger.Send(true, $"{MessageType.MP_START_BUTTON_IS_ENABLED}");
            ElapsedRepetitions = 0;
            _messenger.Send(0f, $"{MessageType.MP_ELAPSED_TIME_CHANGED}");
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
                        foreach (var interval in Intervals)
                        {
                            interval.IsRunning = false;
                            interval.TimeLeft = interval.Duration;
                            if (interval.Position == 1)
                                interval.IsRunning = true;
                        }

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

            _messenger.Send(ElapsedTime, $"{MessageType.MP_ELAPSED_TIME_CHANGED}");
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

        public float CalculateAngle(float time, float totalTime)
        {
            float intervalAngle = time * 360f / totalTime;
            return intervalAngle;
        }

        public void SetDefaultTimeLeft()
        {
            foreach (var interval in Intervals)
            {
                interval.TimeLeft = interval.Duration;
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
    }
}