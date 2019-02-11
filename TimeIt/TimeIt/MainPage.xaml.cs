using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeIt.UserControls;
using TimeIt.ViewModels;
using Xamarin.Forms;

namespace TimeIt
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel
            => (MainPageViewModel) BindingContext;

        public MainPage()
        {
            InitializeComponent();
            ViewModel.Init().GetAwaiter().GetResult();
        }

        #region Old

        //void StartTimer()
        //{
        //    System.Diagnostics.Debug.WriteLine("--------------Starting the timer...");
        //    intervals.ForEach(t =>
        //    {
        //        if (t.TimeLeft == 0)
        //            t.TimeLeft = t.Duration;
        //        if (t.Position == 1)
        //            t.IsRunning = true;
        //    });

        //    int totalMinutes = intervals.Sum(t => t.Duration);
        //    totalTime.Text = TimeSpan.FromSeconds(totalMinutes).ToString("c");
        //    _customTimer = new CustomTimer(TimeSpan.FromSeconds(1), () =>
        //    {
        //        UpdateCurrentInterval();
        //        canvasView.InvalidateSurface();
        //    });
        //    _customTimer.Start();
        //    startButton.IsEnabled = false;
        //    pauseButton.IsEnabled = true;
        //    stopButton.IsEnabled = true;
        //}

        //void PauseTimer()
        //{
        //    if (_customTimer.IsRunning)
        //    {
        //        _customTimer.Stop();
        //        _customTimer.IsPaused = true;
        //    }
        //    else
        //    {
        //        _customTimer.Start();
        //        _customTimer.IsPaused = false;
        //    }
        //}

        //void StopTimer()
        //{
        //    startButton.IsEnabled = true;
        //    pauseButton.IsEnabled = false;
        //    stopButton.IsEnabled = false;

        //    System.Diagnostics.Debug.WriteLine("--------------Stopping the timer");
        //    _customTimer.Stop();
        //    _customTimer.IsPaused = false;
        //    intervals.ForEach(t =>
        //    {
        //        t.IsRunning = false;
        //        t.TimeLeft = t.Duration;
        //    });
        //    canvasView.InvalidateSurface();
        //}

        //void UpdateCurrentInterval()
        //{
        //    var currentInterval = intervals.FirstOrDefault(t => t.IsRunning);
        //    if (currentInterval is null)
        //        throw new NullReferenceException("There arent 0 running intervals");
        //    System.Diagnostics.Debug.WriteLine("--------------Updating current interval started");
        //    System.Diagnostics.Debug.WriteLine($"Interval = {currentInterval.Name}, time left = {currentInterval.TimeLeft}");

        //    if (currentInterval.TimeLeft == 0)
        //    {
        //        currentInterval.IsRunning = false;
        //        var nextInterval = intervals.FirstOrDefault(t => t.Position == currentInterval.Position + 1);
        //        if (nextInterval is null)
        //        {
        //            StopTimer();
        //        }
        //        else
        //        {
        //            nextInterval.IsRunning = true;
        //            nextInterval.TimeLeft--;
        //        }

        //    }
        //    else
        //    {
        //        currentInterval.TimeLeft--;
        //    }
        //    System.Diagnostics.Debug.WriteLine("--------------Updating current interval completed");
        //}

        #endregion

        #region Old 2

        //private float CalculateFinalAngle(int intervalDuration, float totalTime)
        //{
        //    float intervalAngle = intervalDuration * 360 / totalTime;
        //    System.Diagnostics.Debug.WriteLine($"Calculated final angle = {intervalAngle}");
        //    return intervalAngle;
        //}

        //private float CalculateCurrentAngle(float elapsedTime, float totalTime)
        //{
        //    float intervalAngle = elapsedTime * 360 / totalTime;
        //    return intervalAngle;
        //}

        #endregion

        #region Not being used

        //private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        //{
        //    SKSurface surface = e.Surface;
        //    SKCanvas canvas = surface.Canvas;

        //    canvas.DrawPaint(backgroundFillPaint);

        //    int width = e.Info.Width;
        //    int height = e.Info.Height;

        //    // Set transforms
        //    canvas.Translate(width / 2, height / 2);
        //    canvas.Scale(Math.Min(width / 210f, height / 520f));

        //    // Get DateTime
        //    DateTime dateTime = DateTime.Now;

        //    // Head
        //    canvas.DrawCircle(0, -160, 75, blackFillPaint);

        //    // Draw ears and eyes
        //    for (int i = 0; i < 2; i++)
        //    {
        //        canvas.Save();
        //        canvas.Scale(2 * i - 1, 1);

        //        canvas.Save();
        //        canvas.Translate(-65, -255);
        //        canvas.DrawPath(catEarPath, blackFillPaint);
        //        canvas.Restore();

        //        canvas.Save();
        //        canvas.Translate(10, -170);
        //        canvas.DrawPath(catEyePath, greenFillPaint);
        //        canvas.DrawPath(catPupilPath, blackFillPaint);
        //        canvas.Restore();

        //        // Draw whiskers
        //        canvas.DrawLine(10, -120, 100, -100, whiteStrokePaint);
        //        canvas.DrawLine(10, -125, 100, -120, whiteStrokePaint);
        //        canvas.DrawLine(10, -130, 100, -140, whiteStrokePaint);
        //        canvas.DrawLine(10, -135, 100, -160, whiteStrokePaint);

        //        canvas.Restore();
        //    }

        //    // Move Tail
        //    float t = (float)Math.Sin((dateTime.Second % 2 + dateTime.Millisecond / 1000.0) * Math.PI);
        //    catTailPath.Reset();
        //    catTailPath.MoveTo(0, 100);
        //    SKPoint point1 = new SKPoint(-50 * t, 200);
        //    SKPoint point2 = new SKPoint(0, 250 - Math.Abs(50 * t));
        //    SKPoint point3 = new SKPoint(50 * t, 250 - Math.Abs(75 * t));
        //    catTailPath.CubicTo(point1, point2, point3);

        //    canvas.DrawPath(catTailPath, blackStrokePaint);

        //    // Clock background
        //    canvas.DrawCircle(0, 0, 100, blackFillPaint);

        //    // Hour and minute marks
        //    for (int angle = 0; angle < 360; angle += 6)
        //    {
        //        canvas.DrawCircle(0, -90, angle % 30 == 0 ? 4 : 2, whiteFillPaint);
        //        canvas.RotateDegrees(6);
        //    }

        //    // Hour hand
        //    canvas.Save();
        //    canvas.RotateDegrees(30 * dateTime.Hour + dateTime.Minute / 2f);
        //    canvas.DrawPath(hourHandPath, grayFillPaint);
        //    canvas.DrawPath(hourHandPath, whiteStrokePaint);
        //    canvas.Restore();

        //    // Minute hand
        //    canvas.Save();
        //    canvas.RotateDegrees(6 * dateTime.Minute + dateTime.Second / 10f);
        //    canvas.DrawPath(minuteHandPath, grayFillPaint);
        //    canvas.DrawPath(minuteHandPath, whiteStrokePaint);
        //    canvas.Restore();

        //    // Second hand
        //    canvas.Save();
        //    float seconds = dateTime.Second + dateTime.Millisecond / 1000f;
        //    canvas.RotateDegrees(6 * seconds);
        //    whiteStrokePaint.StrokeWidth = 2;
        //    canvas.DrawLine(0, 10, 0, -80, whiteStrokePaint);
        //    canvas.Restore();
        //}

        //private void StartAngleSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        //{
        //    canvasView.InvalidateSurface();
        //} 

        #endregion
    }
}