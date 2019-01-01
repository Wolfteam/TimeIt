using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;

namespace TimeIt.ViewModels
{
    public class TimerItemViewModel : ViewModelBase
    {
        private string _name;
        private int _repetitions;
        private ObservableCollection<IntervalItemViewModel> _intervals = new ObservableCollection<IntervalItemViewModel>();

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public int Repetitions
        {
            get => _repetitions;
            set => Set(ref _repetitions, value);
        }

        public float RemainingTime
        {
            get => TotalTime - ElapsedTime;
        }

        public float ElapsedTime
        {
            get => Intervals?.Sum(i => i.ElapsedTime) ?? 0;
        }

        public float TotalTime
        {
            get => Intervals?.Sum(i => i.Duration) ?? 0;
        }

        public ObservableCollection<IntervalItemViewModel> Intervals
        {
            get => _intervals;
            set => Set(ref _intervals, value);
        }
    }
}
