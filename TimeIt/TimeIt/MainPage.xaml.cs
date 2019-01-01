using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Linq;
using TimeIt.ViewModels;
using Xamarin.Forms;

namespace TimeIt
{
    public partial class MainPage : ContentPage
    {
        private volatile bool _rendering = false;
        private string _timeSpanFormat = "hh\\:mm\\:ss";

        public MainPageViewModel ViewModel
        {
            get => (MainPageViewModel)BindingContext;
        }

        public MainPage()
        {
            InitializeComponent();
            ViewModel.InvalidateSurfaceEvent = canvasView.InvalidateSurface;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            System.Diagnostics.Debug.WriteLine("size allocated");
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_rendering)
            {
                System.Diagnostics.Debug.WriteLine("Canvas is being already rendered");
                return;
            }

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            float explodeOffset = 50;
            float radius = Math.Min(info.Width / 2.5f, info.Height / 2.5f);

            //0 = i
            float yPoint = (info.Height / 2);
            SKPoint center = new SKPoint(info.Width / 2, yPoint + (radius * 2 * 0) + (explodeOffset * 0));
            SKRect rect = new SKRect(center.X - radius, center.Y - radius,
                                     center.X + radius, center.Y + radius);

            try
            {
                if (ViewModel.customTimer?.IsRunning == true && 
                    !ViewModel.requestReDraw)
                {
                    float totalTime = ViewModel.GetTimerTotalTime();
                    float totalElapsedTime = ViewModel.GetTimerTotalElapsedTime();
                    float startAngle = ViewModel.CalculateAngle(totalElapsedTime, totalTime) - 90f;
                    //sometimes i need a *-1 in the sweepAngle o.o
                    float sweepAngle = ViewModel.CalculateAngle(1, totalTime) *-1;
                    UpdateCurrentInterval(rect, canvas, totalElapsedTime, startAngle, sweepAngle);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"{args.Surface.Canvas.LocalClipBounds.IsEmpty}");
                System.Diagnostics.Debug.WriteLine("--------------Rendering the timer started");
                _rendering = true;
                canvas.Clear();
                for (int i = 0; i < 1; i++)
                {
                    float startAngle = 0;
                    float sweepAngle = 0;
                    float totalTime = ViewModel.GetTimerTotalTime();

                    foreach (var interval in ViewModel.Timer.Intervals.OrderBy(t => t.Position))
                    {
                        System.Diagnostics.Debug.WriteLine($"----Rendering Interval = {interval.Name} started");
                        sweepAngle = ViewModel.CalculateAngle(interval.Duration, totalTime);
                        System.Diagnostics.Debug.WriteLine($"Duration = {interval.Duration}, Start angle = {startAngle}, final angle = {sweepAngle}");
                        using (var path = new SKPath())
                        using (var intervalTextPaint = new SKPaint()
                        {
                            Color = GetTextColor()
                        })
                        using (var strokePaint = new SKPaint()
                        {
                            Color = SKColor.Parse(interval.Color),
                            Style = SKPaintStyle.Stroke,
                            StrokeCap = SKStrokeCap.Butt
                        })
                        using (var contourPaint = new SKPaint()
                        {
                            Color = SKColors.Black,
                            Style = SKPaintStyle.Stroke,
                            StrokeCap = SKStrokeCap.Butt
                        })
                        {
                            intervalTextPaint.TextSize = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
                            strokePaint.StrokeWidth = intervalTextPaint.TextSize;
                            contourPaint.StrokeWidth = (float)(1.25 * strokePaint.StrokeWidth);

                            switch (Device.RuntimePlatform)
                            {
                                case Device.Android:
                                    strokePaint.StrokeWidth =
                                        intervalTextPaint.TextSize *= 2.5f;
                                    break;
                            }
                            //draw the arc
                            path.AddArc(rect, startAngle - 90f, sweepAngle);
                            canvas.DrawPath(path, contourPaint);
                            canvas.DrawPath(path, strokePaint);

                            var pathMeasure = new SKPathMeasure(path, false, 1);

                            //draw the interval name text
                            intervalTextPaint.TextScaleX = 1.2f;

                            string intervalName = string.Empty;
                            if (intervalTextPaint.MeasureText(interval.Name) >= pathMeasure.Length)
                            {
                                intervalName = $"{interval.Name}...";
                                float charWidth = intervalTextPaint.MeasureText(intervalName) / intervalName.Length;
                                float diff = intervalTextPaint.MeasureText(intervalName) - pathMeasure.Length * 0.8f;
                                int charsToRemove = (int)Math.Floor(diff / charWidth);
                                intervalName = $"{intervalName.Substring(0, interval.Name.Length - charsToRemove)}...";
                            }
                            else
                                intervalName = interval.Name;

                            float textWidth = intervalTextPaint.MeasureText(intervalName);
                            float nameHOffset = pathMeasure.Length / 2f - textWidth / 2f;

                            canvas.DrawTextOnPath(intervalName, path, nameHOffset, -strokePaint.StrokeWidth, intervalTextPaint);
                        }
                        DrawDurationIntervalText(rect, canvas, interval);
                        startAngle += sweepAngle;
                        System.Diagnostics.Debug.WriteLine($"----Rendering Interval = {interval.Name} completed");
                    }
                }

                if (ViewModel.customTimer?.IsRunning == true && ViewModel.requestReDraw ||
                    ViewModel.customTimer?.IsPaused == true && ViewModel.requestReDraw)
                {
                    float totalTime = ViewModel.GetTimerTotalTime();

                    float totalElapsedTime = ViewModel.GetTimerTotalElapsedTime();
                    float startAngle = -90f;
                    float sweepAngle = ViewModel.CalculateAngle(totalElapsedTime, totalTime);
                    UpdateCurrentInterval(rect, canvas, totalElapsedTime, startAngle, sweepAngle);
                    ViewModel.requestReDraw = false;
                }
                _rendering = false;
                System.Diagnostics.Debug.WriteLine("--------------Rendering the timer completed");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw e;
            }
        }

        void UpdateCurrentInterval(SKRect rect, SKCanvas canvas, float totalElapsedTime, float startAngle, float sweepAngle)
        {
            var currentInterval = ViewModel.Timer.Intervals.FirstOrDefault(t => t.IsRunning);
            if (currentInterval is null)
                throw new NullReferenceException("There arent 0 running intervals");

            float totalTime = ViewModel.GetTimerTotalTime();

            using (var path = new SKPath())
            using (var textPaint = new SKPaint())
            using (var transparentStrokePaint = new SKPaint
            {
                Color = GetTransparentColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Entry)) + 1f,
                StrokeCap = SKStrokeCap.Butt
            })
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        transparentStrokePaint.StrokeWidth *= 2.5f;
                        break;
                }

                path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, transparentStrokePaint);
            }
            DrawDurationIntervalText(rect, canvas, currentInterval);
            ViewModel.ElapsedTimeText = TimeSpan.FromSeconds(totalElapsedTime).ToString(_timeSpanFormat);
        }

        private void DrawDurationIntervalText(SKRect rect, SKCanvas canvas, IntervalItemViewModel currentInterval)
        {
            float textSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Entry)) * 1.5f;
            float durationVOffset = (float)Device.GetNamedSize(NamedSize.Small, typeof(Entry)) * 2f;

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    durationVOffset *= 2.5f;
                    textSize *= 2.5f;
                    break;
            }

            //Here i create two text, one that will be drawn with a "transparent" color
            //and the other one that will be drawn over the "transparent" one
            string ciText = TimeSpan.FromSeconds(currentInterval.TimeLeft).ToString(_timeSpanFormat);
            string replacementText = TimeSpan.FromSeconds(currentInterval.TimeLeft + 1).ToString(_timeSpanFormat);
            float startAngle = 0;
            float sweepAngle = 0;

            foreach (var interval in ViewModel.Timer.Intervals.OrderBy(i => i.Position))
            {
                sweepAngle = ViewModel.CalculateAngle(interval.Duration, ViewModel.GetTimerTotalTime());
                if (interval.Position == currentInterval.Position)
                {
                    break;
                }
                startAngle += sweepAngle;
            }


            using (var path = new SKPath())
            using (var intervalTimePaint = new SKPaint())
            using (var intervalTimeTransparentPaint = new SKPaint())
            {
                path.AddArc(rect, startAngle - 90f, sweepAngle);
                var pathMeasure = new SKPathMeasure(path, false, 1);

                intervalTimePaint.Color = SKColor.Parse(currentInterval.Color);
                intervalTimeTransparentPaint.Color = GetTransparentColor();
                intervalTimePaint.TextSize =
                    intervalTimeTransparentPaint.TextSize = textSize;
                intervalTimePaint.Style =
                    intervalTimeTransparentPaint.Style = SKPaintStyle.StrokeAndFill;
                intervalTimePaint.StrokeWidth =
                    intervalTimeTransparentPaint.StrokeWidth = 2;
                //intervalTimePaint.TextScaleX =
                //    intervalTimeTransparentPaint.TextScaleX = pathMeasure.Length / pathMeasure.Length * 0.6f;

                float durationWidth = intervalTimePaint.MeasureText(ciText);
                float durationHOffset = pathMeasure.Length / 2f - durationWidth / 2f;
                //float durationHOffset = (float)(Math.PI * sweepAngle * radius / 360) - durationWidth / 2f;

                //first we draw the "transparent" one
                canvas.DrawTextOnPath(replacementText, path, durationHOffset, durationVOffset, intervalTimeTransparentPaint);
                //and then the real one, with the corresponding color
                canvas.DrawTextOnPath(ciText, path, durationHOffset, durationVOffset, intervalTimePaint);
            }
        }

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Size changed. Width = {Width} - Height = {Height}");
            if (ViewModel.customTimer?.IsRunning == true ||
                ViewModel.customTimer?.IsPaused == true)
                ViewModel.requestReDraw = true;
        }

        private SKColor GetTextColor()
        {
            return ViewModel.IsDarkTheme() ?
                SKColors.White :
                SKColor.Parse("#271c1c");
        }

        private SKColor GetTransparentColor()
        {
            return ViewModel.IsDarkTheme() ?
                SKColor.Parse("#271c1c") :
                SKColors.White;
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
