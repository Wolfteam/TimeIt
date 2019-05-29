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

namespace TimeIt.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Members
        private readonly IAppSettingsService _appSettings;
        private readonly INavigationService _navigationService;
        private readonly ITimeItDataService _timeItDataService;
        private readonly IMapper _mapper;
        private readonly IMessenger _messenger;
        private readonly ICustomDialogService _dialogService;
        private string _currentTimerName;
        private int _remainingRepetitions;
        private string _totalTimeText;
        private string _elapsedOrRemainingTimeText;
        private bool _startButtonEnabled = true;
        private bool _mainButtonsAreVisible = true;
        private bool _isAddTimerButtonVisible = true;
        private bool _isEditTimerButtonVisible;
        private bool _isDeleteTimerButtonVisible;
        private int _currentPage = 0;
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

        public string ElapsedOrRemainingText
            => _appSettings.ShowElapsedInsteadOfRemainingTime
                ? "Elapsed"
                : "Remaining";

        public string PauseButtonText
            => Timers.Any(t => t.CustomTimer?.IsPaused == true)
                ? "Resume"
                : "Pause";

        public string ElapsedOrRemainingTimeText
        {
            get => _elapsedOrRemainingTimeText;
            set => Set(ref _elapsedOrRemainingTimeText, value);
        }

        public bool IsStartButtonEnabled
        {
            get => _startButtonEnabled;
            set => Set(ref _startButtonEnabled, value);
        }

        public bool MainButtonsAreVisible
        {
            get => _mainButtonsAreVisible;
            set => Set(ref _mainButtonsAreVisible, value);
        }

        public bool IsAddTimerButtonVisible
        {
            get => _isAddTimerButtonVisible;
            set => Set(ref _isAddTimerButtonVisible, value);
        }

        public bool IsEditTimerButtonVisible
        {
            get => _isEditTimerButtonVisible;
            set => Set(ref _isEditTimerButtonVisible, value);
        }

        public bool IsDeleteTimerButtonVisible
        {
            get => _isDeleteTimerButtonVisible;
            set => Set(ref _isDeleteTimerButtonVisible, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Position changed. New position = {value}. OldPosition = {_currentPage}");
                bool positionChanged = _currentPage != value;
                Set(ref _currentPage, value);
                if (positionChanged)
                    CurrentSelectedTimerChanged();
            }
        }

        public ObservableCollection<TimerItemViewModel> Timers
        {
            get => _timers;
            set => Set(ref _timers, value);
        }

        public bool IsTimerRunning
            => Timers.Any(t => t.CustomTimer?.IsRunning == true);

        public TimerItemViewModel CurrentRunningTimer 
            => Timers.FirstOrDefault(t => t.CustomTimer?.IsRunning == true);
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
            IAppSettingsService appSettings,
            INavigationService navigationService,
            ITimeItDataService timeItDataService,
            IMapper mapper,
            IMessenger messenger,
            ICustomDialogService dialogService)
        {
            _appSettings = appSettings;
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
            OnAppearingCommand = new RelayCommand(async () =>
            {
                System.Diagnostics.Debug.WriteLine("On appearing...");
                await Init();
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

            OpenSettingsCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo($"{AppPages.SETTINGS}");
                Navigated = true;
            });

            StartTimerCommand = new RelayCommand(() =>
            {
                //CanNavigate = false;
                if (Timers.Count > 0)
                    Timers[CurrentPage].StartTimer();
                RaisePropertyChanged(() => PauseButtonText);
            });

            PauseTimerCommand = new RelayCommand(() =>
            {
                if (Timers.Count > 0)
                    Timers[CurrentPage].PauseTimer();
                RaisePropertyChanged(() => PauseButtonText);
            });

            StopTimerCommand = new RelayCommand(() =>
            {
                if (Timers.Count > 0)
                    Timers[CurrentPage].StopTimer();
                RaisePropertyChanged(() => PauseButtonText);
                //CanNavigate = true;
            });
        }

        private void RegisterMessages()
        {
            _messenger.Register<bool>(
                this,
                $"{MessageType.MP_START_BUTTON_IS_ENABLED}",
                EnableStartAndAssociatedButtons);

            _messenger.Register<bool>(
                this,
                $"{MessageType.MP_SHOW_ELAPSED_TIME_SETTING_CHANGED}",
                ShowElapsedOrRemainingTimeSettingChanged);

            _messenger.Register<float>(
                this,
                $"{MessageType.MP_ELAPSED_TIME_CHANGED}",
                SetElapsedOrRemainingTimeText);

            _messenger.Register<float>(
                this,
                $"{MessageType.MP_TOTAL_TIME_CHANGED}",
                SetTotalTimeText);

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
                var vm = _mapper.Map<TimerItemViewModel>(timer);
                vm.SetDefaultTimeLeft();
                vms.Add(vm);
            }

            Timers = new ObservableCollection<TimerItemViewModel>(vms);

            CurrentSelectedTimerChanged();

            //if a timer was running before the app was killed..
            if (ViewModelLocator.TimerOnSleep != null)
            {
                var timer = Timers.First(t => t.TimerID == ViewModelLocator.TimerOnSleep.TimerID);
                //this is required to let the cards view do its things..
                await Task.Delay(1);
                CurrentPage = Timers.IndexOf(timer);
                timer.OnResume(ViewModelLocator.TimerOnSleep);
                ViewModelLocator.TimerOnSleep = null;
                ViewModelLocator.WasAppInForeground = false;
            }

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
                    var timerToRemove = Timers.First(t => t.TimerID == timerID);
                    timerToRemove.UnregisterMessages();
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
                IsEditTimerButtonVisible =
                    IsDeleteTimerButtonVisible = false;

                MainButtonsAreVisible = false;
                CurrentTimerName = "N/A";
                RemainingRepetitions = 0;
                SetTotalTimeText(0);
                SetElapsedOrRemainingTimeText(0);
                return;
            }

            if (CurrentPage < 0)
                return;

            EnableStartAndAssociatedButtons(true);
            MainButtonsAreVisible = true;

            var currentTimer = Timers[CurrentPage];
            CurrentTimerName = currentTimer.Name;
            RemainingRepetitions = currentTimer.RemainingRepetitions;
            SetTotalTimeText(currentTimer.TotalTime);
            SetElapsedOrRemainingTimeText(_appSettings.ShowElapsedInsteadOfRemainingTime
                ? currentTimer.ElapsedTime
                : currentTimer.RemainingTime);
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
            currentTimer.UnregisterMessages();
            Timers.Remove(currentTimer);
            _dialogService.ShowSimpleMessage($"Timer {currentTimer.Name} was successfully removed");
        }

        private void ShowElapsedOrRemainingTimeSettingChanged(bool showElapsed)
        {
            RaisePropertyChanged(() => ElapsedOrRemainingText);
            if (Timers.Any(t => t.CustomTimer?.IsRunning == true))
                return;
            var currentTimer = Timers[CurrentPage];
            SetElapsedOrRemainingTimeText(showElapsed ? currentTimer.ElapsedTime : currentTimer.RemainingTime);
        }

        private void SetTotalTimeText(float totalSeconds)
            => TotalTimeText = TimeSpan.FromSeconds(totalSeconds).ToString(AppConstants.DefaultTimeSpanFormat);

        private void SetElapsedOrRemainingTimeText(float seconds)
            => ElapsedOrRemainingTimeText = TimeSpan.FromSeconds(seconds).ToString(AppConstants.DefaultTimeSpanFormat);

        private void EnableStartAndAssociatedButtons(bool isEnabled)
        {
            IsStartButtonEnabled =
                IsAddTimerButtonVisible =
                    IsEditTimerButtonVisible =
                        IsDeleteTimerButtonVisible = isEnabled;
            RaisePropertyChanged(() => PauseButtonText);
        }
    }
}