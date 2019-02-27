using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeIt.Enums;
using TimeIt.Helpers;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Members

        private readonly INavigationService _navigationService;
        private readonly ITimeItDataService _timeItDataService;
        private readonly IMapper _mapper;
        private readonly IMessenger _messenger;
        private readonly ICustomDialogService _dialogService;
        private string _currentTimerName;
        private int _remainingRepetitions;
        private string _totalTimeText;
        private string _elapsedTimeText;
        private bool _startButtonEnabled = true;
        private int _currentPage = 0;
        private bool _canNavigate = true;
        private ObservableCollection<TimerItemViewModel> _timers = new ObservableCollection<TimerItemViewModel>();

        public bool Navigated;
        private bool _initialized;

        #endregion

        #region Properties

        public string CurrentTimerName
        {
            get => _currentTimerName;
            set => Set(ref _currentTimerName, value);
        }

        public int RemainingRepetitions
        {
            get => _remainingRepetitions;
            set => Set(ref _remainingRepetitions, value);
        }

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

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Position changed. New position = {value}. OldPosition = {_currentPage}");
                Set(ref _currentPage, value);
                if (_currentPage >= 0)
                    CurrentSelectedTimerChanged();
            }
        }

        public ObservableCollection<TimerItemViewModel> Timers
        {
            get => _timers;
            set => Set(ref _timers, value);
        }

        public bool CanNavigate
        {
            get => _canNavigate;
            set
            {
                Set(ref _canNavigate, value);
                Timers[CurrentPage].InvalidateSurfaceEvent.Invoke();
            }
        }

        #endregion

        #region Commands

        public ICommand OnAppearingCommand { get; private set; }

        public ICommand AddTimerCommand { get; private set; }

        public ICommand EditTimerCommand { get; private set; }

        public ICommand RemoveTimerCommand { get; private set; }

        public ICommand OpenSettingsCommand { get; private set; }

        public ICommand StartTimerCommand { get; private set; }

        public ICommand PauseTimerCommand { get; private set; }

        public ICommand StopTimerCommand { get; private set; }

        #endregion

        public MainPageViewModel(
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

            SetCommands();
            RegisterMessages();
        }

        private void SetCommands()
        {
            OnAppearingCommand = new RelayCommand(() =>
            {
                System.Diagnostics.Debug.WriteLine("On appearing...");
                if (!Navigated && !ViewModelLocator.WasAppInForeground)
                    return;
                ViewModelLocator.WasAppInForeground = false;
                _messenger.Send(true, $"{MessageType.ON_NAVIGATED_BACK}");
            });

            AddTimerCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo($"{AppPages.TIMER}");
                _messenger.Send(0, $"{MessageType.ADD_TIMER}");
                Navigated = true;
            });

            EditTimerCommand = new RelayCommand(() =>
            {
                var currentTimer = Timers[CurrentPage];
                _navigationService.NavigateTo($"{AppPages.TIMER}");
                _messenger.Send(currentTimer.TimerID, $"{MessageType.ADD_TIMER}");
                Navigated = true;
            });

            RemoveTimerCommand = new RelayCommand
                (async () => await RemomveCurrentTimerAsync());

            OpenSettingsCommand = new RelayCommand(() => { });

            StartTimerCommand = new RelayCommand(() =>
            {
                //CanNavigate = false;
                if (Timers.Count > 0)
                {
                    Timers[CurrentPage].StartTimer();
                }
            });

            PauseTimerCommand = new RelayCommand(() =>
            {
                if (Timers.Count > 0)
                    Timers[CurrentPage].PauseTimer();
            });

            StopTimerCommand = new RelayCommand(() =>
            {
                if (Timers.Count > 0)
                    Timers[CurrentPage].StopTimer();
                //CanNavigate = true;
            });
        }

        private void RegisterMessages()
        {
            _messenger.Register<bool>(
                this,
                $"{MessageType.MP_START_BUTTON_IS_ENABLED}",
                isEnabled => StartButtonEnabled = isEnabled);

            _messenger.Register<float>(
                this,
                $"{MessageType.MP_ELAPSED_TIME_CHANGED}",
                seconds => ElapsedTimeText = TimeSpan.FromSeconds(seconds).ToString(Constans.DefaultTimeSpanFormat));

            _messenger.Register<float>(
                this,
                $"{MessageType.MP_TOTAL_TIME_CHANGED}",
                seconds => TotalTimeText = TimeSpan.FromSeconds(seconds).ToString(Constans.DefaultTimeSpanFormat));

            _messenger.Register<int>(
                this,
                $"{MessageType.MP_REMAINING_REPETITIONS_CHANGED}",
                repetitions => RemainingRepetitions = repetitions);

            _messenger.Register<TimerItemViewModel>(
                this,
                $"{MessageType.MP_TIMER_CREATED}",
                timer => OnTimerModified(OperationType.CREATED, timer));

            _messenger.Register<TimerItemViewModel>(
                this,
                $"{MessageType.MP_TIMER_UPDATED}",
                timer => OnTimerModified(OperationType.UPDATED, timer));

            _messenger.Register<int>(
                this,
                $"{MessageType.MP_TIMER_REMOVED}",
                timerID => OnTimerModified(OperationType.DELETED, null, timerID));
        }

        public async Task Init()
        {
            if (_initialized)
                return;

            var timers = await _timeItDataService.GetAllTimers();
            var vms = new List<TimerItemViewModel>();
            foreach (var timer in timers)
            {
                var vm = new TimerItemViewModel(_messenger);
                _mapper.Map(timer, vm);
                vm.SetDefaultTimeLeft();
                vms.Add(vm);
            }

            Timers = new ObservableCollection<TimerItemViewModel>(vms);

            if (Device.RuntimePlatform == Device.Android)
                CurrentSelectedTimerChanged();

            _initialized = true;
        }

        private void OnTimerModified(OperationType operation, TimerItemViewModel timer, int? timerID = null)
        {
            switch (operation)
            {
                case OperationType.CREATED:
                    Timers.Add(timer);
                    CurrentPage = Timers.Count - 1;
                    break;
                case OperationType.UPDATED:
                    var currentTimer = Timers.FirstOrDefault(t => t.TimerID == timer.TimerID);
                    if (currentTimer is null)
                        throw new NullReferenceException(
                            $"The timer that was updated doesnt exists in the view. TimerID = {timer.TimerID}");
                    int index = Timers.IndexOf(currentTimer);
                    Timers.RemoveAt(index);
                    Timers.Insert(index, timer);
                    CurrentPage = index;
                    break;
                case OperationType.DELETED:
                    TimerItemViewModel timerToRemove;
                    if (timerID.HasValue)
                        timerToRemove = Timers.FirstOrDefault(t => t.TimerID == timerID);
                    else
                        timerToRemove = Timers.FirstOrDefault(t => t.TimerID == timer.TimerID);
                    Timers.Remove(timerToRemove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation,
                        "The provided OperationType doesnt exists");
            }

        }

        private void CurrentSelectedTimerChanged()
        {
            if (Timers.Count == 0)
            {
                StartButtonEnabled = false;
                CurrentTimerName = string.Empty;
                RemainingRepetitions = 0;
                TotalTimeText = string.Empty;
                ElapsedTimeText = string.Empty;
                return;
            }

            var currentTimer = Timers[CurrentPage];
            StartButtonEnabled = true;
            CurrentTimerName = currentTimer.Name;
            RemainingRepetitions = currentTimer.RemainingRepetitions;
            TotalTimeText = TimeSpan.FromSeconds(currentTimer.TotalTime).ToString(Constans.DefaultTimeSpanFormat);
            ElapsedTimeText = TimeSpan.FromSeconds(currentTimer.ElapsedTime).ToString(Constans.DefaultTimeSpanFormat);
        }

        private async Task RemomveCurrentTimerAsync()
        {
            var currentTimer = Timers[CurrentPage];
            bool deleteTimer = await _dialogService
                .ShowConfirmationDialogAsync("Confirm", $"Are you sure you want to delete timer {currentTimer.Name} ?");

            if (!deleteTimer)
                return;

            bool wasRemoved = await _timeItDataService.RemoveTimer(currentTimer.TimerID);
            if (!wasRemoved)
            {
                _dialogService.ShowSimpleMessage($"An error occurred while trying to delete timer {currentTimer.Name}");
                return;
            }
            Timers.Remove(currentTimer);
            _dialogService.ShowSimpleMessage($"Timer {currentTimer.Name} was successfully removed");
        }
    }
}