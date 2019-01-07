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

        public List<IntervalItemViewModel> Intervals
        {
            get => new List<IntervalItemViewModel>
            {
                new IntervalItemViewModel
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1,
                    Color = "#58ff00",
                },
                new IntervalItemViewModel
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3,
                    Color = "#0c00ff",
                },
                new IntervalItemViewModel
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2,
                    Color = "#ff0000",
                },
                new IntervalItemViewModel
                {
                    Duration = 10,
                    Name = "IntervalItemViewModelo con un nombre muy largo",
                    Position = 4,
                    Color = "#feff00",
                }
            };
        }


        public string IntervalToTalTime
        {
            get
            {
                float totalTime = Intervals.Sum(i => i.Duration);
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
