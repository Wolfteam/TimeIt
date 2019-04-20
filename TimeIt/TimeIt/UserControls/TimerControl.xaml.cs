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
            if (ViewModel?.CustomTimer?.IsRunning == true ||
                ViewModel?.CustomTimer?.IsPaused == true)
                ViewModel.RequestReDraw = true;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_rendering)
            {
                System.Diagnostics.Debug.WriteLine("Canvas is being already rendered");
                return;
            }

            if (ViewModel is null)
            {
                System.Diagnostics.Debug.WriteLine("View model is null from OnCanvasViewPaintSurface");
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
            try
            {
                System.Diagnostics.Debug.WriteLine($"--------------Rendering the timer started. Timer = {ViewModel.Name}");
                DrawIntervals(canvas, rect);
                System.Diagnostics.Debug.WriteLine($"--------------Rendering the timer completed. Timer = {ViewModel.Name}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        private void DrawIntervals(SKCanvas canvas, SKRect rect)
        {
            float totalTime = ViewModel.GetTimerCycleTotalTime();
            float startAngle = 0;
            float sweepAngle;
            //this avoids redrawing all the intervals but with big intervals
            //it shows white dots...
            //if (ViewModel.CustomTimer?.IsRunning == true &&
            //    !ViewModel.RequestReDraw)
            //{
            //    System.Diagnostics.Debug.WriteLine($"--------------Timer = {ViewModel.Name} is running and a redraw was not requested");
            //    float totalElapsedTime = ViewModel.GetTimerCycleTotalElapsedTime();
            //    startAngle = ViewModel.CalculateAngle(totalElapsedTime, totalTime) - 90f;
            //    //sometimes i need a *-1 in the sweepAngle o.o
            //    sweepAngle = ViewModel.CalculateAngle(1, totalTime) * -1;
            //    UpdateCurrentInterval(rect, canvas, startAngle, sweepAngle);
            //    return;
            //}
            _rendering = true;
            var orderedIntervals = ViewModel.Intervals.OrderBy(t => t.Position);
            canvas.Clear();
            //We draw all the intervals...
            foreach (var interval in orderedIntervals)
            {
                sweepAngle = CalculateAngle(interval.Duration, totalTime);
                using (var path = new SKPath())
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
                    strokePaint.StrokeWidth = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
                    contourPaint.StrokeWidth = (float)(1.25 * strokePaint.StrokeWidth);
                    strokePaint.StrokeWidth *= Device.RuntimePlatform == Device.Android
                        ? 2.5f
                        : 1f;

                    //draw the arc
                    path.AddArc(rect, startAngle - 90f, sweepAngle);
                    canvas.DrawPath(path, contourPaint);
                    canvas.DrawPath(path, strokePaint);

                    //Draw the interval duration text
                    DrawIntervalDuration(rect, canvas, interval);

                    //Draw the interval name
                    DrawIntervalName(path, canvas, interval.Name, -strokePaint.StrokeWidth);
                }

                startAngle += sweepAngle;
            }

            //then we draw a 'transparent' arc based on the elapsed time
            if (ViewModel.CustomTimer?.IsRunning == true ||
                ViewModel.CustomTimer?.IsPaused == true)
            {
                System.Diagnostics.Debug.WriteLine($"--------------Timer = {ViewModel.Name} is running/paused and a redraw was requested");
                float totalElapsedTime = ViewModel.GetTimerCycleTotalElapsedTime();
                startAngle = -90f;
                sweepAngle = CalculateAngle(totalElapsedTime, totalTime);
                UpdateCurrentInterval(rect, canvas, startAngle, sweepAngle);
                ViewModel.RequestReDraw = false;
            }
            _rendering = false;
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
                transparentStrokePaint.StrokeWidth *= Device.RuntimePlatform == Device.Android
                    ? 2.5f
                    : 1f;
                path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, transparentStrokePaint);
            }
            DrawIntervalDuration(rect, canvas, currentInterval);
        }

        private void DrawIntervalName(SKPath intervalPath, SKCanvas canvas, string currentIntName, float vOffset)
        {
            using (var intervalTextPaint = new SKPaint
            {
                Color = GetTextColor(),
                TextScaleX = 1.2f
            })
            {
                var pathMeasure = new SKPathMeasure(intervalPath);

                intervalTextPaint.TextSize = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
                intervalTextPaint.TextSize *= Device.RuntimePlatform == Device.Android
                    ? 2.5f
                    : 1f;
                float initialTextSize = intervalTextPaint.TextSize;
                SetIntervalTextSize(intervalTextPaint, pathMeasure.Length);

                //if the new text size is too small, just return
                if (IsIntervalTextSizeTooSmall(initialTextSize, intervalTextPaint.TextSize))
                    return;

                string intervalName = currentIntName;
                int aditionalCharsToRemove = 0;
                while (intervalTextPaint.MeasureText(intervalName) >= pathMeasure.Length * 0.8f)
                {
                    float charWidth = intervalTextPaint.MeasureText(currentIntName) / currentIntName.Length;
                    float diff = intervalTextPaint.MeasureText(currentIntName) - pathMeasure.Length * 0.8f;
                    int charsToRemove = (int)Math.Floor(diff / charWidth) + aditionalCharsToRemove;
                    //just in case...
                    if (currentIntName.Length - charsToRemove < 1)
                        return;
                    intervalName = $"{currentIntName.Substring(0, currentIntName.Length - charsToRemove)}...";
                    aditionalCharsToRemove++;
                }

                float textWidth = intervalTextPaint.MeasureText(intervalName);
                float nameHOffset = pathMeasure.Length / 2f - textWidth / 2f;

                canvas.DrawTextOnPath(intervalName, intervalPath, nameHOffset, vOffset, intervalTextPaint);
            }
        }

        private void DrawIntervalDuration(SKRect rect, SKCanvas canvas, IntervalItemViewModel currentInterval)
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
            string ciText = TimeSpan.FromSeconds(currentInterval.TimeLeft).ToString(AppConstants.DefaultTimeSpanFormat);
            string replacementText = TimeSpan.FromSeconds(currentInterval.TimeLeft + 1).ToString(AppConstants.DefaultTimeSpanFormat);
            float startAngle = 0;
            float sweepAngle = 0;

            foreach (var interval in ViewModel.Intervals.OrderBy(i => i.Position))
            {
                sweepAngle = CalculateAngle(interval.Duration, ViewModel.GetTimerCycleTotalTime());
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
                intervalTimeTransparentPaint.Color = GetAppBgColor();
                intervalTimePaint.TextSize =
                    intervalTimeTransparentPaint.TextSize = textSize;
                intervalTimePaint.Style =
                    intervalTimeTransparentPaint.Style = SKPaintStyle.StrokeAndFill;
                intervalTimePaint.StrokeWidth =
                    intervalTimeTransparentPaint.StrokeWidth = 2;

                float initialTextSize = intervalTimePaint.TextSize;
                SetIntervalTextSize(intervalTimePaint, pathMeasure.Length);
                SetIntervalTextSize(intervalTimeTransparentPaint, pathMeasure.Length);

                if (IsIntervalTextSizeTooSmall(initialTextSize, intervalTimePaint.TextSize))
                    return;
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

        private void SetIntervalTextSize(SKPaint sKPaint, float max)
        {
            float minimum = 0;
            bool enoughSpace = false;
            string minimumChars = "00:00:00";
            while (!enoughSpace)
            {
                minimum = sKPaint.MeasureText(minimumChars);
                //we want a 20% extra space
                enoughSpace = minimum * 1.2f < max;
                if (enoughSpace)
                    break;
                //if there is not enough space, decrement it by 10%
                sKPaint.TextSize -= 0.1f * sKPaint.TextSize;
                sKPaint.TextScaleX -= 0.1f * sKPaint.TextScaleX;
                sKPaint.StrokeWidth -= 0.1f * sKPaint.StrokeWidth;
            }
        }

        private bool IsIntervalTextSizeTooSmall(float originalSize, float newSize)
        {
            return newSize <= originalSize * 0.7f;
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
                GetAppBgColor() :
                SKColors.White;
        }

        private SKColor GetAppBgColor()
        {
            var appBgColor = (Color)Application.Current.Resources[AppConstants.AppBackgroundColorKey];
            return appBgColor.ToSKColor();
        }

        private float CalculateAngle(float time, float totalTime)
        {
            float intervalAngle = time * 360f / totalTime;
            return intervalAngle;
        }
    }
}