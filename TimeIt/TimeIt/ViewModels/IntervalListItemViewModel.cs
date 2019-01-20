using System;

namespace TimeIt.ViewModels
{
    public class IntervalListItemViewModel
    {
        public string Name { get; set; }

        public float Duration { get; set; }

        public string Color { get; set; }

        public int Position { get; set; }

        public string DurationString
            => $"Duration: {TimeSpan.FromSeconds(Duration).ToString("hh\\:mm\\:ss")}";
    }
}
