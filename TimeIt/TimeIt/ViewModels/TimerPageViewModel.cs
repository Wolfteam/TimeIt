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
using TimeIt.Interfaces;
using TimeIt.Models;
using Xamarin.Forms;

namespace TimeIt.ViewModels
{
    public class TimerPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ITimeItDataService _timeItDataService;
        private readonly IMapper _mapper;
        private readonly IMessenger _messenger;
        private readonly ICustomDialogService _dialogService;
        private int _timerID;
        private string _timerName;
        private int _repetitions;

        private ObservableCollection<IntervalListItemViewModel> _intervals =
            new ObservableCollection<IntervalListItemViewModel>();

        public string TimerName
        {
            get => _timerName;
            set => Set(ref _timerName, value);
        }

        public int Repetitions
        {
            get => _repetitions;
            set
            {
                Set(ref _repetitions, value);
                RaisePropertyChanged(() => IntervalToTalTime);
            }
        }

        public ObservableCollection<IntervalListItemViewModel> Intervals
        {
            get => _intervals;
            set => Set(ref _intervals, value);
        }

        public string IntervalToTalTime
        {
            get
            {
                float totalTime = Intervals.Sum(i => i.Duration) * Repetitions;
                return $"Total: {TimeSpan.FromSeconds(totalTime):c}";
            }
        }

        public ICommand SaveTimerCommand { get; private set; }

        public ICommand DiscardChangesCommand { get; private set; }

        public ICommand RemoveTimerCommand { get; private set; }

        public ICommand AddIntervalCommand { get; private set; }

        public ICommand EditIntervalCommand { get; private set; }

        public ICommand RemoveIntervalCommand { get; private set; }

        public TimerPageViewModel(
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
            SaveTimerCommand = new RelayCommand(async () =>
            {
                if (_timerID == 0)
                    await SaveTimerAsync();
                else
                    await UpdateTimerAsync();
            });

            DiscardChangesCommand = new RelayCommand
                (() => _navigationService.GoBack());

            RemoveTimerCommand = new RelayCommand
                (() => _dialogService.ShowSimpleMessage("not implemented yet !"));

            AddIntervalCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo($"{AppPages.INTERVAL}");
                _messenger.Send(Intervals.Count, $"{MessageType.ADD_INTERVAL}");
            });

            EditIntervalCommand = new RelayCommand<IntervalListItemViewModel>((interval) =>
            {
                _navigationService.NavigateTo($"{AppPages.INTERVAL}");
                _messenger.Send((interval, Intervals.Count), $"{MessageType.EDIT_INTERVAL}");
            });

            RemoveIntervalCommand = new RelayCommand<IntervalListItemViewModel>
                (async (interval) => await DeleteIntervalAsync(interval));
        }

        private void RegisterMessages()
        {
            _messenger.Register<int>(
                this,
                $"{MessageType.ADD_TIMER}",
                async timerID => await InitAsync(timerID)
            );

            _messenger.Register<(OperationType, IntervalListItemViewModel)>(
                this,
                $"{MessageType.INTERVAL_ADDED_EDITED}",
                tuple => OnIntervalModified(tuple.Item1, tuple.Item2)
            );
        }

        public async Task InitAsync(int timerID)
        {
            //Create
            _timerID = timerID;
            if (timerID == 0)
            {
                Repetitions = 1;
                TimerName = string.Empty;
                Intervals = new ObservableCollection<IntervalListItemViewModel>();
            }
            //edit
            else
            {
                var timer = await _timeItDataService.GetTimer(timerID);
                TimerName = timer.Name;
                Repetitions = timer.Repetitions;
                Intervals = _mapper.Map<ObservableCollection<IntervalListItemViewModel>>(timer.Intervals);
            }
            RaisePropertyChanged(() => IntervalToTalTime);
        }

        public async Task DeleteIntervalAsync(IntervalListItemViewModel interval)
        {
            bool delete = await _dialogService.ShowConfirmationDialogAsync(
                "Confirmation",
                $"Are you sure you wanna delete interval {interval.Name} ?");
            if (!delete)
                return;

            //it was created in memory
            if (interval.IntervalID != 0)
            {
                bool wasRemoved = await _timeItDataService.RemoveInterval(_timerID, interval.IntervalID);
                if (!wasRemoved)
                {
                    _dialogService.ShowSimpleMessage(
                        $"An error occurred while trying to delete interval = {interval.Name}");
                    return;
                }
            }

            Intervals.Remove(interval);
            RaisePropertyChanged(() => IntervalToTalTime);
            _dialogService.ShowSimpleMessage($"{interval.Name} was deleted");
        }

        public async Task SaveTimerAsync()
        {
            if (Intervals.Count == 0)
            {
                _dialogService.ShowSimpleMessage("You need to add at least one interval");
                return;
            }

            var timer = new Timer
            {
                Name = TimerName,
                Repetitions = Repetitions,
                Intervals = _mapper.Map<List<Interval>>(Intervals)
            };
            await _timeItDataService.AddTimer(timer);

            var vm = new TimerItemViewModel(_messenger);
            var createdTimer = _mapper.Map(timer, vm);
            createdTimer.SetDefaultTimeLeft();

            _messenger.Send(createdTimer, $"{MessageType.MP_TIMER_CREATED}");
            _navigationService.GoBack();
        }

        public async Task UpdateTimerAsync()
        {
            if (Intervals.Count == 0)
            {
                _dialogService.ShowSimpleMessage("You need to add at least one interval");
                return;
            }

            var timerToUpdate = await _timeItDataService.GetTimer(_timerID);
            timerToUpdate.Name = TimerName;
            timerToUpdate.Repetitions = Repetitions;

            var newIntervals = _mapper.Map<List<Interval>>(Intervals.Where(i => i.IntervalID == 0));

            var updatedIntervals = _mapper.Map<List<Interval>>(Intervals.Where(i => i.IntervalID != 0));

            var intervalsToUpdate = timerToUpdate.Intervals
                .Where(i => updatedIntervals.Any(ui => ui.IntervalID == i.IntervalID));
            foreach (var interval in intervalsToUpdate)
            {
                var ui = updatedIntervals.FirstOrDefault(i => i.IntervalID == interval.IntervalID);
                //just in case...
                if (ui is null)
                    continue;
                interval.Color = ui.Color;
                interval.Duration = ui.Duration;
                interval.Name = ui.Name;
                interval.Position = ui.Position;
            }

            foreach (var interval in newIntervals)
                timerToUpdate.Intervals.Add(interval);

            await _timeItDataService.UpdateTimer(timerToUpdate);

            var vm = new TimerItemViewModel(_messenger);
            var updatedTimer = _mapper.Map(timerToUpdate, vm);
            updatedTimer.SetDefaultTimeLeft();

            _messenger.Send(updatedTimer, $"{MessageType.MP_TIMER_UPDATED}");
            _navigationService.GoBack();
        }

        private void OnIntervalModified(OperationType operation, IntervalListItemViewModel interval)
        {
            switch (operation)
            {
                case OperationType.CREATED:
                    Intervals.Insert(interval.Position - 1, interval);
                    break;
                case OperationType.UPDATED:
                    var oldInterval = interval.IntervalID == 0
                        ? Intervals.FirstOrDefault(i => i.InMemoryIntervalID == interval.InMemoryIntervalID)
                        : Intervals.FirstOrDefault(i => i.IntervalID == interval.IntervalID);

                    if (oldInterval is null)
                        throw new NullReferenceException(
                            $"The interval that was updated doesnt exists in the view. IntervalID = {interval.IntervalID}");

                    int index = Intervals.IndexOf(oldInterval);
                    if (oldInterval.Position == interval.Position)
                    {
                        //for some reasone the normal way is not working on uwp...
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                Intervals[index] = interval;
                                break;
                            default:
                                Intervals.Remove(oldInterval);
                                Intervals.Insert(interval.Position - 1, interval);
                                break;
                        }
                    }
                    else
                    {
                        Intervals.Remove(oldInterval);
                        Intervals.Insert(interval.Position - 1, interval);
                    }

                    break;
                case OperationType.DELETED:
                    var intervalToRemove = Intervals.FirstOrDefault(i => i.IntervalID == interval.IntervalID);
                    Intervals.Remove(intervalToRemove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation,
                        "The provided OperationType doesnt exists");
            }

            RaisePropertyChanged(() => IntervalToTalTime);
        }
    }
}