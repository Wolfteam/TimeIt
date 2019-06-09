using TimeIt.UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Frame), typeof(FixFrameRenderer))]
namespace TimeIt.UWP.Renderers
{
    /// <summary>
    /// https://github.com/AndreiMisiukevich/CardView/issues/126
    /// </summary>
    public class FixFrameRenderer : FrameRenderer
    {
        protected override void UpdateBackgroundColor()
        {
            if (Control is null)
                return;
            var backgroundColor = Element.BackgroundColor;
            if (!backgroundColor.IsDefault)
            {
                Control.Background = backgroundColor.ToBrush();
            }
            else
            {
                //////Control.ClearValue(BackgroundProperty); ISSUE HERE
                Control.Background = new Color(0, 0, 0, 0).ToBrush();
            }
        }
    }
}
