using System;
using Xamarin.Forms;

namespace TimeIt.Models
{
    public class Interval
    {
        public int IntervalID { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        //public int TimeLeft { get; set; }
        public Color Color { get; set; }
        //public bool IsRunning { get; set; }
        public int Position { get; set; }

        //public int ElapsedTime
        //{
        //    get => Math.Abs(TimeLeft - Duration);
        //}

        public int TimerID { get; set; }
        public Timer Timer { get; set; }
    }
}
