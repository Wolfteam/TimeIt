using GalaSoft.MvvmLight;
using System;

namespace TimeIt.ViewModels
{
    public class IntervalItemViewModel : ViewModelBase
    {
        private string _name;
        private float _duration;
        private string _color;
        private int _position;

        public int IntervalID { get; set; }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public float Duration
        {
            get => _duration;
            set
            {
                Set(ref _duration, value);
                //RaisePropertyChanged(nameof(DurationString));
            }
        }

        public float TimeLeft { get; set; }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public bool IsRunning { get; set; }

        public int Position
        {
            get => _position;
            set => Set(ref _position, value);
        }


        public float ElapsedTime
            => Math.Abs(TimeLeft - Duration);
    }
}
