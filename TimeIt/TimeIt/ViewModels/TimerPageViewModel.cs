using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeIt.Enums;
using TimeIt.Interfaces;
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
        private ObservableCollection<IntervalListItemViewModel> _intervals = new ObservableCollection<IntervalListItemViewModel>();

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

        public ObservableCollection<IntervalListItemViewModel> Intervals
        {
            get => _intervals;
            set => Set(ref _intervals, value);
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

        public ICommand AddIntervalCommand { get; set; }

        public ICommand EditIntervalCommand { get; set; }

        public ICommand RemoveIntervalCommand { get; set; }

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

        public void SetCommands()
        {
            SaveTimerCommand = new RelayCommand
                (() => System.Diagnostics.Debug.WriteLine($"The interval {TimerName} was saved"));

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

            RemoveIntervalCommand = new RelayCommand<IntervalListItemViewModel>(async (interval) =>
            {
                bool result = await _dialogService.ShowConfirmationDialogAsync(
                    "Confirmation", 
                    $"Are you sure you wanna delete interval {interval.Name} ?");
                //TODO: COMPLETE THE REMOVE INTERVAL LOGIC
            });
        }

        public void RegisterMessages()
        {
            _messenger.Register<int>(
                this,
                $"{MessageType.ADD_TIMER}",
                async (timerID) => await Init(timerID)
            );

            _messenger.Register<(OperationType, IntervalListItemViewModel)>(
                this,
                $"{MessageType.INTERVAL_ADDED_EDITED}",
                (tuple) => OnIntervalModified(tuple.Item1, tuple.Item2)
            );
        }

        public async Task Init(int timerID)
        {
            //Create
            if (timerID == 0)
            {
                _timerID =
                    Repetitions = 0;
                TimerName = string.Empty;
                Intervals = new ObservableCollection<IntervalListItemViewModel>();
            }
            //edit
            else
            {
                var timer = await _timeItDataService.GetTimer(timerID);
                _timerID = timer.TimerID;
                TimerName = timer.Name;
                Repetitions = timer.Repetitions;
                Intervals = _mapper.Map<ObservableCollection<IntervalListItemViewModel>>(timer.Intervals);
            }
        }

        private void OnIntervalModified(OperationType operation, IntervalListItemViewModel interval)
        {
            switch (operation)
            {
                case OperationType.CREATED:
                    Intervals.Insert(interval.Position - 1, interval);
                    break;
                case OperationType.UPDATED:
                    IntervalListItemViewModel oldInterval;
                    if (interval.IntervalID == 0)
                    {
                        oldInterval = Intervals.FirstOrDefault(i => i.InMemoryIntervalID == interval.InMemoryIntervalID);
                    }
                    else
                    {
                        oldInterval = Intervals.FirstOrDefault(i => i.IntervalID == interval.IntervalID);
                    }

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
            }
            RaisePropertyChanged(() => IntervalToTalTime);
        }

    }
}
