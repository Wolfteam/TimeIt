﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TimeIt.Enums;
using TimeIt.Extensions;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.ViewModels
{
    public class IntervalPageViewModel : ViewModelBase
    {
        #region Fields
        private readonly INavigationService _navigationService;
        private readonly ITimeItDataService _timeItDataService;
        private readonly IMapper _mapper;
        private readonly IMessenger _messenger;
        private readonly ICustomDialogService _dialogService;

        private bool _isInEditMode;
        private int _intervalID;
        private string _inMemoryIntervalID;
        private string _name;
        private Color _selectedColor;
        private double _position;

        private double _maximumPosition;
        private double _minimumPosition;
        private int _minimumSeconds;
        private int _minutes;
        private int _seconds;
        private bool _isPositionEnabled;

        private static Random _rnd = new Random();
        private static FieldInfo[] _colors = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public);
        #endregion

        #region Properties
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

        public double Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;
                Set(ref _position, value);
            }
        }

        public double MaximumPosition
        {
            get => _maximumPosition;
            set => Set(ref _maximumPosition, value);
        }

        public double MinimumPosition
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
                var timespan = GetIntervalDuration();
                return $"Duration: {timespan.ToString("hh\\:mm\\:ss")}";
            }
        }

        public bool IsPositionEnabled
        {
            get => _isPositionEnabled;
            set => Set(ref _isPositionEnabled, value);
        }
        #endregion

        #region Commands
        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DiscardChangesCommand { get; private set; }
        #endregion

        public IntervalPageViewModel(
            INavigationService navigationService,
            ITimeItDataService timeItDataService,
            IMapper mapper,
            IMessenger messenger,
            ICustomDialogService dialogService)
        {
            _navigationService = navigationService;
            _timeItDataService = timeItDataService;
            _mapper = mapper;
            _messenger = messenger;
            _dialogService = dialogService;

            //to avoid a crash... minimum must be set before maximum for a stepper
            CleanUp();
            SetCommands();
            RegisterMessages();
        }

        public void SetCommands()
        {
            SaveCommand = new RelayCommand
                (SaveInterval);

            DiscardChangesCommand = new RelayCommand(() =>
            {
                _navigationService.GoBack();
            });
        }

        public void RegisterMessages()
        {
            _messenger.Register<int>(
                this,
                $"{MessageType.ADD_INTERVAL}",
                 Init);
            _messenger.Register<(IntervalListItemViewModel, int)>(
                this,
                $"{MessageType.EDIT_INTERVAL}",
                 tuple => Init(tuple.Item1, tuple.Item2));
        }

        public void Init(int numberOfItems)
        {
            CleanUp();
            _isInEditMode = false;

            //this happens when there are no items added in the list
            if (numberOfItems == 0)
            {
                Position = 1;
                IsPositionEnabled = false;
                return;
            }
            MaximumPosition = numberOfItems + 1;
            Position = numberOfItems + 1;
        }

        public void Init(IntervalListItemViewModel interval, int numberOfItems)
        {
            CleanUp();
            _isInEditMode = true;
            _intervalID = interval.IntervalID;
            _inMemoryIntervalID = interval.InMemoryIntervalID;

            Name = interval.Name;
            SelectedColor = Color.FromHex(interval.Color);
            SetMinutesAndSeconds(interval.Duration);
            //this happens when there are no items added in the list
            if (numberOfItems == 1)
            {
                numberOfItems++;
                IsPositionEnabled = false;
            }
            MaximumPosition = numberOfItems;
            Position = interval.Position;
            System.Diagnostics.Debug.WriteLine($"Init: Position = {Position} - Max = {MaximumPosition}");
        }

        private void SaveInterval()
        {
            if (string.IsNullOrEmpty(Name))
            {
                _dialogService.ShowSimpleMessage("You need to provide an Interval name");
                return;
            }
            var interval = GetIntervalData();
            var operation = !_isInEditMode ? OperationType.CREATED : OperationType.UPDATED;
            _navigationService.GoBack();
            _messenger.Send((operation, interval), $"{MessageType.INTERVAL_ADDED_EDITED}");
        }

        private void CleanUp()
        {
            _intervalID = 0;
            Name = string.Empty;

            if (!IsPositionEnabled)
                IsPositionEnabled = true;

            if (MaximumPosition != 2)
                MaximumPosition = 2;

            if (MinimumPosition != 1)
                MinimumPosition = 1;

            if (Position != 2)
                Position = 2;

            SetMinutesAndSeconds(45);
            SelectedColor = (Color)_colors[_rnd.Next(_colors.Count())].GetValue(Color.Red);
        }

        private void UpdateMinimumSeconds(int minutes)
        {
            if (minutes > 0 && MinimumSeconds != 0)
                MinimumSeconds = 0;
            else if (minutes == 0 && MinimumSeconds != 1)
                MinimumSeconds = 1;
        }

        private TimeSpan GetIntervalDuration()
            => TimeSpan.FromMinutes(Minutes) + TimeSpan.FromSeconds(Seconds);

        private IntervalListItemViewModel GetIntervalData()
        {
            bool shouldGenerateNewGuid = string.IsNullOrEmpty(_inMemoryIntervalID);
            string guid = !_isInEditMode || (_isInEditMode && _intervalID == 0 && shouldGenerateNewGuid) ?
                Guid.NewGuid().ToString() :
                 _inMemoryIntervalID;
            var interval = new IntervalListItemViewModel
            {
                Color = SelectedColor.ToHexString(),
                Duration = (float)GetIntervalDuration().TotalSeconds,
                IntervalID = _intervalID,
                Name = Name,
                Position = (int)Position,
                InMemoryIntervalID = guid
            };

            return interval;
        }

        private void SetMinutesAndSeconds(double duration)
        {
            Minutes = TimeSpan.FromSeconds(duration).Minutes;
            Seconds = TimeSpan.FromSeconds(duration).Seconds;
        }
    }
}
