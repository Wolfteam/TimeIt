﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeIt.Interfaces;
using TimeIt.Models;

namespace TimeIt.Services
{
    public class TimeItDataService : ITimeItDataService
    {
        private readonly TimeItDbContext _timeItDbContext;

        public TimeItDataService(TimeItDbContext timeItDbContext)
        {
            TimeItDbContext.Init();
            _timeItDbContext = timeItDbContext;
        }

        public async Task<Interval> AddInterval(int timerID, Interval interval)
        {
            var timer = await GetTimer(timerID);
            if (timer is null)
                throw new ArgumentException($"The provided timerId = {timerID} does not exists", nameof(timerID));

            timer.Intervals.Add(interval);
            await _timeItDbContext.SaveChangesAsync();
            return interval;
        }

        public async Task<Timer> AddTimer(Timer timer)
        {
            await _timeItDbContext.Timers.AddAsync(timer);
            await _timeItDbContext.SaveChangesAsync();
            return timer;
        }

        public async Task<IEnumerable<Timer>> GetAllTimers()
        {
            return await _timeItDbContext
                .Timers
                .Include(t => t.Intervals)
                .ToListAsync();
        }

        public async Task<Interval> GetInterval(int intervalID)
        {
            return await _timeItDbContext
                .Intervals
                .FirstOrDefaultAsync(i => i.IntervalID == intervalID);

        }

        public async Task<IEnumerable<Interval>> GetIntervals(int timerID)
        {
            return await _timeItDbContext
                .Intervals
                .ToListAsync();
        }

        public async Task<Timer> GetTimer(int timerID)
        {
            return await _timeItDbContext
                 .Timers
                 .Include(t => t.Intervals)
                 .FirstOrDefaultAsync(t => t.TimerID == timerID);
        }

        public async Task<bool> RemoveInterval(int timerID, int intervalID)
        {
            var intervalToRemove = await GetInterval(intervalID);
            if (intervalToRemove is null)
                throw new ArgumentException($"The provided intervalID = {intervalID} does not exists", nameof(intervalID));

            _timeItDbContext.Intervals.Remove(intervalToRemove);
            await _timeItDbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> RemoveIntervals(int timerID)
        {
            var intervalsToRemove = _timeItDbContext.Intervals.Where(i => i.TimerID == timerID);
            _timeItDbContext.Intervals.RemoveRange(intervalsToRemove);
            await _timeItDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTimer(int timerID)
        {
            var timerToRemove = await GetTimer(timerID);
            if (timerToRemove is null)
                throw new ArgumentException($"The provided timerID = {timerID} does not exists", nameof(timerID));

            _timeItDbContext.Timers.Remove(timerToRemove);
            await _timeItDbContext.SaveChangesAsync();
            return true;

        }

        public async Task<Timer> UpdateTimer(Timer timer)
        {
            _timeItDbContext.Timers.Update(timer);
            await _timeItDbContext.SaveChangesAsync();
            return timer;
        }

        public async Task<Timer> UpdateTimer(Timer timer, IEnumerable<Interval> intervals)
        {
            _timeItDbContext.Intervals.RemoveRange(timer.Intervals);

            foreach (var interval in intervals)
            {
                interval.IntervalID = 0;
                timer.Intervals.Add(interval);
            }
            await _timeItDbContext.SaveChangesAsync();
            return timer;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _timeItDbContext.SaveChangesAsync();
        }
    }
}
