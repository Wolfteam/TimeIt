using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimeIt.Helpers
{
    public class CustomTimer
    {
        private readonly TimeSpan _timeSpan;
        private volatile Action _callback;
        private volatile Stopwatch _stopwatch = new Stopwatch();

        private volatile CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; set; }

        public CustomTimer(TimeSpan timeSpan, Action callback)
        {
            _timeSpan = timeSpan;
            _callback = callback;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            IsRunning = true;
            _stopwatch.Start();
            Device.BeginInvokeOnMainThread(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _callback.Invoke();
                    //kinda hack...
                    int ms = (int)(_timeSpan.TotalMilliseconds - _stopwatch.Elapsed.TotalMilliseconds);
                    if (ms > 0)
                        await Task.Delay(ms);
                    _stopwatch.Restart();
                }
                _cancellationTokenSource = new CancellationTokenSource();
                _stopwatch.Stop();
            });
            //Task.Run(async () =>
            //{
            //    while (!_cancellationTokenSource.IsCancellationRequested)
            //    {
            //        _callback.Invoke();
            //        //kinda hack...
            //        int ms = (int)(_timeSpan.TotalMilliseconds - _stopwatch.Elapsed.TotalMilliseconds);
            //        await Task.Delay(ms);
            //        _stopwatch.Restart();
            //    }
            //    _cancellationTokenSource = new CancellationTokenSource();
            //    _stopwatch.Stop();
            //});
        }

        public void Stop()
        {
            IsRunning = false;
            _cancellationTokenSource.Cancel();
        }
    }
}
