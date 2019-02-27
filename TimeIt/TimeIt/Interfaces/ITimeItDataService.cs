using System.Collections.Generic;
using System.Threading.Tasks;
using TimeIt.Models;

namespace TimeIt.Interfaces
{
    public interface ITimeItDataService
    {
        Task<Timer> AddTimer(Timer timer);
        Task<IEnumerable<Timer>> GetAllTimers();
        Task<Timer> GetTimer(int timerID);
        Task<Timer> UpdateTimer(Timer timer);
        Task<bool> RemoveTimer(int timerID);

        Task<Interval> AddInterval(int timerID, Interval interval);
        Task<IEnumerable<Interval>> GetIntervals(int timerID);
        Task<Interval> GetInterval(int intervalID);
        Task<bool> RemoveInterval(int timerID, int intervalID);
    }
}
