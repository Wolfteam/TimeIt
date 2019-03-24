using System.ComponentModel;
using TimeIt.Controls;
using TimeIt.UWP.Renderers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TimerActionButton), typeof(TimerActionButtonRenderer))]
namespace TimeIt.UWP.Renderers
{
    public class TimerActionButtonRenderer : ButtonRenderer
    {
        private readonly Windows.UI.Xaml.Thickness _disabledBorderThickness = new Windows.UI.Xaml.Thickness(0, 0, 0, 0);
        private readonly Windows.UI.Xaml.Thickness _enabledBorderThickness = new Windows.UI.Xaml.Thickness(2, 2, 2, 2);

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control is null)
                return;

            var button = Control as Windows.UI.Xaml.Controls.Button;
            if (button.IsEnabled)
            {
                button.BorderThickness = _enabledBorderThickness;
                button.Margin = _disabledBorderThickness;
                return;
            }
            button.BorderThickness = _disabledBorderThickness;
            button.Margin = _enabledBorderThickness;
        }

    }
}
