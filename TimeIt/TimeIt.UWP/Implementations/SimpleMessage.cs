using System;
using TimeIt.Interfaces;
using TimeIt.UWP.Helpers;
using TimeIt.UWP.Implementations;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

[assembly: Xamarin.Forms.Dependency(typeof(SimpleMessage))]
namespace TimeIt.UWP.Implementations
{
    public class SimpleMessage : ISimpleMessage
    {
        private const double LONG_DELAY = 8.0;
        private const double SHORT_DELAY = 4.0;

        public void ShowMessage(string message, bool longDelay = false)
        {
            ShowMessage(message, longDelay ? LONG_DELAY : SHORT_DELAY);
        }

        private void ShowMessage(string message, double duration)
        {
            var window = Window.Current.Content as FrameworkElement;
            double width = window.ActualWidth > 500 ?
                window.ActualWidth / 2 :
                window.ActualWidth;
            var label = new TextBlock
            {
                Text = message,
                Width = 0.90 * width,
                Margin = new Thickness(10, 5, 5, 5),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalTextAlignment = TextAlignment.Start,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            var tooltip = new ToolTip
            {
                Content = message
            };
            ToolTipService.SetToolTip(label, tooltip);

            var style = new Style
            {
                TargetType = typeof(FlyoutPresenter)
            };
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(ColorUtils.GetColor("#323232"))));
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, width));
            style.Setters.Add(new Setter(FrameworkElement.MaxWidthProperty, width));

            var flyout = new Flyout
            {
                Content = label,
                Placement = FlyoutPlacementMode.Bottom,
                FlyoutPresenterStyle = style,
            };
            flyout.ShowAt(window);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(duration)
            };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                flyout.Hide();
            };
            timer.Start();
        }
    }
}
