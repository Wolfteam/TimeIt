using System;
using System.Collections.Generic;
using System.Text;

namespace TimeIt.Models
{
    public class Timer
    {
        public string Name { get; set; }
        public int Repetitions { get; set; }
        public int RemainingTime { get; set; }
        public int TotalTime { get; set; }
        public ICollection<Interval> Intervals { get; set; }
        //public bool IsRunning { get; set; }
    }
}
