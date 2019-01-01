using System;

namespace TimeIt.ViewModels
{
    public class IntervalItemViewModel
    {
        public string Name { get; set; }
        public float Duration { get; set; }
        public float TimeLeft { get; set; }
        public string Color { get; set; }
        public bool IsRunning { get; set; }
        public int Position { get; set; }

        public float ElapsedTime
        {
            get => Math.Abs(TimeLeft - Duration);
        }
    }
}
