using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Linq;
using TimeIt.Helpers;
using TimeIt.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.UserControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerControl : ContentView
    {
        private volatile bool _rendering;

        private TimerItemViewModel ViewModel
            => (TimerItemViewModel)BindingContext;

        public TimerControl()
        {
                InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            System.Diagnostics.Debug.WriteLine($"Binding context changed");
            if (ViewModel is null)
            {
                System.Diagnostics.Debug.WriteLine($"View model is null..");
                return;
            }
            ViewModel.InvalidateSurfaceEvent = CanvasView.InvalidateSurface;
        }

        private void ContentView_SizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Size changed. Width = {Width} - Height = {Height}");
            if (ViewModel.CustomTimer?.IsRunning == true ||
                ViewModel.CustomTimer?.IsPaused == true)
                ViewModel.RequestReDraw = true;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_rendering)
            {
                System.Diagnostics.Debug.WriteLine("Canvas is being already rendered");
                return;
            }
            System.Diagnostics.Debug.WriteLine("Invalidate was called");
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            float radius = Math.Min(info.Width / 2.5f, info.Height / 2.5f);
            float yPoint = info.Height / 2f;

            var center = new SKPoint(info.Width / 2f, yPoint);
            var rect = new SKRect(center.X - radius, center.Y - radius,
                                     center.X + radius, center.Y + radius);

            float totalTime = ViewModel.GetTimerCycleTotalTime();
            float startAngle = 0;
            float sweepAngle;

            try
            {
                if (ViewModel.CustomTimer?.IsRunning == true &&
                    !ViewModel.RequestReDraw)
                {
                    System.Diagnostics.Debug.WriteLine($"--------------Timer = {ViewModel.Name} is running and a redraw was not requested");
                    float totalElapsedTime = ViewModel.GetTimerCycleTotalElapsedTime();
                    startAngle = ViewModel.CalculateAngle(totalElapsedTime, totalTime) - 90f;
                    //sometimes i need a *-1 in the sweepAngle o.o
                    sweepAngle = ViewModel.CalculateAngle(1, totalTime) * -1;
                    UpdateCurrentInterval(rect, canvas, startAngle, sweepAngle);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"--------------Rendering the timer started. Timer = {ViewModel.Name}");
                _rendering = true;
                canvas.Clear();

                foreach (var interval in ViewModel.Intervals.OrderBy(t => t.Position))
                {
                    //System.Diagnostics.Debug.WriteLine($"----Rendering Interval = {interval.Name} started");
                    sweepAngle = ViewModel.CalculateAngle(interval.Duration, totalTime);
                    //System.Diagnostics.Debug.WriteLine($"Duration = {interval.Duration}, Start angle = {startAngle}, final angle = {sweepAngle}");
                    using (var path = new SKPath())
                    using (var intervalTextPaint = new SKPaint
                    {
                        Color = GetTextColor()
                    })
                    using (var strokePaint = new SKPaint
                    {
                        Color = SKColor.Parse(interval.Color),
                        Style = SKPaintStyle.Stroke,
                        StrokeCap = SKStrokeCap.Butt
                    })
                    using (var contourPaint = new SKPaint
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

                        var pathMeasure = new SKPathMeasure(path);

                        //draw the interval name text
                        intervalTextPaint.TextScaleX = 1.2f;
                        //TODO: IF THE INTERVAL IS SMALL, DURATION AND INTERVAL NAME TEXT GETS CUT
                        string intervalName;
                        if (intervalTextPaint.MeasureText(interval.Name) >= pathMeasure.Length)
                        {
                            float charWidth = intervalTextPaint.MeasureText(interval.Name) / interval.Name.Length;
                            float diff = intervalTextPaint.MeasureText(interval.Name) - pathMeasure.Length * 0.8f;
                            int charsToRemove = (int)Math.Floor(diff / charWidth);
                            intervalName = $"{interval.Name.Substring(0, interval.Name.Length - charsToRemove)}...";
                        }
                        else
                            intervalName = interval.Name;

                        float textWidth = intervalTextPaint.MeasureText(intervalName);
                        float nameHOffset = pathMeasure.Length / 2f - textWidth / 2f;

                        canvas.DrawTextOnPath(intervalName, path, nameHOffset, -strokePaint.StrokeWidth, intervalTextPaint);
                    }
                    DrawDurationIntervalText(rect, canvas, interval);
                    startAngle += sweepAngle;
                    //System.Diagnostics.Debug.WriteLine($"----Rendering Interval = {interval.Name} completed");
                }

                if (ViewModel.CustomTimer?.IsRunning == true && ViewModel.RequestReDraw ||
                    ViewModel.CustomTimer?.IsPaused == true && ViewModel.RequestReDraw)
                {
                    System.Diagnostics.Debug.WriteLine($"--------------Timer = {ViewModel.Name} is running/paused and a redraw was requested");
                    float totalElapsedTime = ViewModel.GetTimerCycleTotalElapsedTime();
                    startAngle = -90f;
                    sweepAngle = ViewModel.CalculateAngle(totalElapsedTime, totalTime);
                    UpdateCurrentInterval(rect, canvas, startAngle, sweepAngle);
                    ViewModel.RequestReDraw = false;
                }
                _rendering = false;
                System.Diagnostics.Debug.WriteLine($"--------------Rendering the timer completed. Timer = {ViewModel.Name}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        private void UpdateCurrentInterval(SKRect rect, SKCanvas canvas, float startAngle, float sweepAngle)
        {
            var currentInterval = ViewModel.Intervals.FirstOrDefault(t => t.IsRunning);
            if (currentInterval is null)
                throw new NullReferenceException($"Timer = {ViewModel.Name} doesnt have a interval that is running");

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
            string ciText = TimeSpan.FromSeconds(currentInterval.TimeLeft).ToString(Constans.DefaultTimeSpanFormat);
            string replacementText = TimeSpan.FromSeconds(currentInterval.TimeLeft + 1).ToString(Constans.DefaultTimeSpanFormat);
            float startAngle = 0;
            float sweepAngle = 0;

            foreach (var interval in ViewModel.Intervals.OrderBy(i => i.Position))
            {
                sweepAngle = ViewModel.CalculateAngle(interval.Duration, ViewModel.GetTimerCycleTotalTime());
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
                var pathMeasure = new SKPathMeasure(path);

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
                //TODO: there is a small glitch with the colors here
                //first we draw the "transparent" one
                canvas.DrawTextOnPath(replacementText, path, durationHOffset, durationVOffset, intervalTimeTransparentPaint);
                //and then the real one, with the corresponding color
                canvas.DrawTextOnPath(ciText, path, durationHOffset, durationVOffset, intervalTimePaint);
            }
        }

        private SKColor GetTextColor()
        {
            return ViewModel.IsDarkTheme() ?
                SKColors.White :
                SKColor.Parse("#2c2929");
        }

        private SKColor GetTransparentColor()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    return ViewModel.IsDarkTheme() ?
                        SKColors.Black :
                        SKColors.White;
            }
            return ViewModel.IsDarkTheme() ?
                SKColor.Parse("#2c2929") :
                SKColors.White;

        }
    }
}