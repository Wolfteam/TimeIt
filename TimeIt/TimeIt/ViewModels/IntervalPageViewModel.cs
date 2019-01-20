using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace TimeIt.ViewModels
{
    public class IntervalPageViewModel : ViewModelBase
    {

        private readonly INavigationService _navigationService;

        private string _name;
        private Color _selectedColor;
        private int _position;

        private int _maximumPosition;
        private int _minimumPosition;
        private int _minimumSeconds;
        private int _minutes;
        private int _seconds;


        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set => Set(ref _selectedColor, value);
        }

        public int Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        public int MaximumPosition
        {
            get => _maximumPosition;
            set => Set(ref _maximumPosition, value);
        }

        public int MinimumPosition
        {
            get => _minimumPosition;
            set => Set(ref _minimumPosition, value);
        }

        public int MinimumSeconds
        {
            get => _minimumSeconds;
            set => Set(ref _minimumSeconds, value);
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                Set(ref _minutes, value);
                RaisePropertyChanged(nameof(DurationString));
                UpdateMinimumSeconds(value);
            }
        }

        public int Seconds
        {
            get => _seconds;
            set
            {
                Set(ref _seconds, value);
                RaisePropertyChanged(nameof(DurationString));
                UpdateMinimumSeconds(Minutes);
            }
        }

        public string DurationString
        {
            get
            {
                var timespan = TimeSpan.FromMinutes(Minutes) + TimeSpan.FromSeconds(Seconds);
                return $"Duration: {timespan.ToString("hh\\:mm\\:ss")}";
            }
        }


        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DiscardChangesCommand { get; private set; }


        //public ICommand MinutesChangedCommand { get; private set; }

        public IntervalPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            DiscardChangesCommand = new RelayCommand(() =>
            {
                _navigationService.GoBack();
            });

            Init(null);
            //MinutesChangedCommand = new RelayCommand<double>((value) =>
            //{
            //    System.Diagnostics.Debug.WriteLine($"Value is = {value}");
            //});
        }


        public void Init(int? id)
        {
            //TODO SEARCH HERE FOR THE CURRENT ID PASSED
            MinimumPosition = 1;
            MaximumPosition = Position = 4;
            Minutes = 2;
            Seconds = 45;
            SelectedColor = Color.Blue;
            Name = "Workout";
        }


        private void UpdateMinimumSeconds(int minutes)
        {
            if (minutes > 0 && MinimumSeconds != 0)
                MinimumSeconds = 0;
            else if (minutes == 0 && MinimumSeconds != 1)
                MinimumSeconds = 1;
        }
    }
}
