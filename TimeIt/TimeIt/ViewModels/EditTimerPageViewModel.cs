using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TimeIt.Models;

namespace TimeIt.ViewModels
{
    public class EditTimerPageViewModel : ViewModelBase
    {
        private string _timerName;
        private int _repetitions;


        public string TimerName
        {
            get => _timerName;
            set => Set(ref _timerName, value);
        }

        public int Repetitions
        {
            get => _repetitions;
            set => Set(ref _repetitions, value);
        }

        public List<Interval> Intervals
        {
            get => new List<Interval>
            {
                new Interval
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1
                },
                new Interval
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3
                },
                new Interval
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2
                },
                new Interval
                {
                    Duration = 10,
                    Name = "Intervalo con un nombre muy largo",
                    Position = 4
                },
                                new Interval
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1
                },
                new Interval
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3
                },
                new Interval
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2
                },
                new Interval
                {
                    Duration = 10,
                    Name = "Intervalo con un nombre muy largo",
                    Position = 4
                },
                                new Interval
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1
                },
                new Interval
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3
                },
                new Interval
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2
                },
                new Interval
                {
                    Duration = 10,
                    Name = "Intervalo con un nombre muy largo",
                    Position = 4
                },
                                new Interval
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1
                },
                new Interval
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3
                },
                new Interval
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2
                },
                new Interval
                {
                    Duration = 10,
                    Name = "Intervalo con un nombre muy largo",
                    Position = 4
                },
                                new Interval
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1
                },
                new Interval
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3
                },
                new Interval
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2
                },
                new Interval
                {
                    Duration = 10,
                    Name = "Intervalo con un nombre muy largo",
                    Position = 4
                }
            };
        }


        public string IntervalToTalTime
        {
            get
            {
                int totalTime = Intervals.Sum(i => i.Duration);
                return $"Total: {TimeSpan.FromSeconds(totalTime).ToString("c")}";
            }
        }

        public ICommand SaveTimerCommand { get; set; }

        public EditTimerPageViewModel()
        {
            SaveTimerCommand = new RelayCommand(() => System.Diagnostics.Debug.WriteLine($"The interval {TimerName} was saved"));
        }


    }
}
