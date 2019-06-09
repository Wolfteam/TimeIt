using System;

namespace TimeIt.ViewModels
{
    public class IntervalListItemViewModel
    {
        public int IntervalID { get; set; }

        public string InMemoryIntervalID { get; set; }

        public string Name { get; set; }

        public float Duration { get; set; }

        public string Color { get; set; }

        public int Position { get; set; }

        public string DurationString
            => $"Duration: {TimeSpan.FromSeconds(Duration).ToString("hh\\:mm\\:ss")}";
    }
}
