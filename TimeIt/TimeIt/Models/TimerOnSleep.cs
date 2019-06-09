using System;

namespace TimeIt.Models
{
    public class TimerOnSleep
    {
        public int TimerID { get; set; }
        public float ElapsedTime { get; set; }
        public int RemainingRepetitions { get; set; }
        public int ElapsedRepetitions { get; set; }

        public int IntervalID { get; set; }
        public float IntervalTimeLeft { get; set; }


        public DateTimeOffset SleepOccurredOn { get; set; }
    }
}
