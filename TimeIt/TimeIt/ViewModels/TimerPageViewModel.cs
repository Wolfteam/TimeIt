using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TimeIt.Enums;
using TimeIt.Models;

namespace TimeIt.ViewModels
{
    public class TimerPageViewModel : ViewModelBase
    {

        private readonly INavigationService _navigationService;
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

        public List<IntervalListItemViewModel> Intervals
        {
            get => new List<IntervalListItemViewModel>
            {
                new IntervalListItemViewModel
                {
                    Duration = 13,
                    Name = "Workout",
                    Position = 1,
                    Color = "#58ff00",
                },
                new IntervalListItemViewModel
                {
                    Duration = 6,
                    Name = "Rest you motherfucker",
                    Position = 3,
                    Color = "#0c00ff",
                },
                new IntervalListItemViewModel
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2,
                    Color = "#ff0000",
                },
                new IntervalListItemViewModel
                {
                    Duration = 10,
                    Name = "IntervalListItemViewModel con un nombre muy largo",
                    Position = 4,
                    Color = "#feff00",
                },
                new IntervalListItemViewModel
                {
                    Duration = 15,
                    Name = "Excercise",
                    Position = 2,
                    Color = "#ff0000",
                },
                new IntervalListItemViewModel
                {
                    Duration = 10,
                    Name = "IntervalListItemViewModel con un nombre muy largo",
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

        public ICommand AddNewIntervalCommand { get; set; }

        public TimerPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveTimerCommand = new RelayCommand(() => System.Diagnostics.Debug.WriteLine($"The interval {TimerName} was saved"));
            AddNewIntervalCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo($"{AppPages.INTERVAL}");
            });
        }


    }
}
