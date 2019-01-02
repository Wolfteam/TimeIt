using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;

namespace TimeIt.ViewModels
{
    public class TimerItemViewModel : ViewModelBase
    {
        private string _name;
        private int _elapsedRepetitions;
        private ObservableCollection<IntervalItemViewModel> _intervals = new ObservableCollection<IntervalItemViewModel>();

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public int Repetitions { get; set; }

        public int RemainingRepetitions
        {
            get => Repetitions - ElapsedRepetitions;
        }

        public int ElapsedRepetitions
        {
            get => _elapsedRepetitions;
            set
            {
                Set(ref _elapsedRepetitions, value);
                RaisePropertyChanged(nameof(RemainingRepetitions));
            }
        }

        public float RemainingTime
        {
            get => TotalTime - ElapsedTime;
        }

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

        public ObservableCollection<IntervalItemViewModel> Intervals
        {
            get => _intervals;
            set => Set(ref _intervals, value);
        }
    }
}
