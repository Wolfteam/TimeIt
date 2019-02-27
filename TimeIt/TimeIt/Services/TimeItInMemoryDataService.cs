using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeIt.Interfaces;
using TimeIt.Models;
using Xamarin.Forms;

namespace TimeIt.Services
{
    public class TimeItInMemoryDataService : ITimeItDataService
    {
        private static List<Timer> timers = new List<Timer>
        {
            new Timer
            {
                Intervals = new List<Interval>
                {
                    new Interval
                    {
                        Duration = 6,
                        Color = Color.OrangeRed,
                        Name = "Workout",
                        Position = 1,
                        IntervalID = 2
                    },
                    new Interval
                    {
                        Duration = 3,
                        Color = Color.Olive,
                        Name = "Rest you motherfucker",
                        Position = 3,
                        IntervalID = 1
                    },
                    new Interval
                    {
                        Duration = 7,
                        Color = Color.Brown,
                        Name = "Excercise",
                        Position = 2,
                        IntervalID = 4
                    },
                    new Interval
                    {
                        Duration = 5,
                        Color = Color.Blue,
                        Name = "Intervalo con un nombre muy largo",
                        Position = 4,
                        IntervalID = 5
                    }
                },
                Name = "Tibia",
                Repetitions = 5,
                TimerID = 1
            },
            new Timer
            {
                Intervals = new List<Interval>
                {
                    new Interval
                    {
                        Duration = 6,
                        Color = Color.OrangeRed,
                        Name = "Agua hirviendo",
                        Position = 1,
                        IntervalID = 1
                    },
                    new Interval
                    {
                        Duration = 3,
                        Color = Color.Olive,
                        Name = "Carne cocida",
                        Position = 3,
                        IntervalID = 2
                    },
                    new Interval
                    {
                        Duration = 7,
                        Color = Color.Brown,
                        Name = "Salsa cocida",
                        Position = 2,
                        IntervalID = 3
                    },
                    new Interval
                    {
                        Duration = 5,
                        Color = Color.Blue,
                        Name = "Pasta cocida",
                        Position = 4,
                        IntervalID = 4
                    }
                },
                Name = "Pasta con carne molida",
                Repetitions = 1,
                TimerID = 2
            }
        };

        public TimeItInMemoryDataService()
        {
            TimeItDbContext.Init();
        }

        public async Task<Timer> AddTimer(Timer timer)
        {
            await Task.Delay(1000);
            int id = timers.Max(t => t.TimerID);
            timer.TimerID = id;
            timers.Add(timer);
            return await Task.FromResult(timer);
        }

        public async Task<Interval> AddInterval(int timerID, Interval interval)
        {
            await Task.Delay(1000);
            var timer = await GetTimer(timerID);
            await Task.Delay(1000);
            int id = timer.Intervals.Max(t => t.IntervalID);
            interval.IntervalID = id;
            timer.Intervals.Add(interval);
            return await Task.FromResult(interval);
        }


        public Task<IEnumerable<Timer>> GetAllTimers()
        {
            return Task.FromResult<IEnumerable<Timer>>(timers);
        }

        public Task<IEnumerable<Interval>> GetIntervals(int timerID)
        {
            var timer = timers.FirstOrDefault(t => t.TimerID == timerID);

            if (timer is null)
                return Task.FromResult(Enumerable.Empty<Interval>());

            return Task.FromResult<IEnumerable<Interval>>(timer.Intervals);
        }

        public Task<Timer> GetTimer(int timerID)
        {
            var timer = timers.FirstOrDefault(t => t.TimerID == timerID);
            return Task.FromResult(timer);
        }

        public Task<Interval> GetInterval(int intervalID)
        {
            var interval = timers.SelectMany(t => t.Intervals).FirstOrDefault(i => i.IntervalID == intervalID);
            return Task.FromResult(interval);
        }

        public async Task<bool> RemoveInterval(int timerID, int intervalID)
        {
            var timer = await GetTimer(timerID);
            var interval = await GetInterval(intervalID);
            timer.Intervals.Remove(interval);
            return true;
        }

        public async Task<Timer> UpdateTimer(Timer timer)
        {
            var timerToRemove = timers.FirstOrDefault(t => t.TimerID == timer.TimerID);
            int index = timers.IndexOf(timerToRemove);
            timers[index] = timer;
            return await Task.FromResult(timer);
        }

        public async Task<bool> RemoveTimer(int timerID)
        {
            var timer = await GetTimer(timerID);
            if (timer is null)
                throw new ArgumentException($"The provided timerID = {timerID} does not exists", nameof(timerID));

            timers.Remove(timer);
            return true;
        }
    }
}
