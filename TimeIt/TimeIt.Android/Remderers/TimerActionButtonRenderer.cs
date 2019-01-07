using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using TimeIt.Controls;
using TimeIt.Droid.Remderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AndroidColor = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(TimerActionButton), typeof(TimerActionButtonRenderer))]
namespace TimeIt.Droid.Remderers
{
    public class TimerActionButtonRenderer : ButtonRenderer
    {
        private AndroidColor _pressedColor = AndroidColor.LightGray;
        private AndroidColor _backgroundColor = AndroidColor.Red;

        public TimerActionButtonRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            var button = Control as Android.Widget.Button;
            button.SetBackgroundColor(_backgroundColor);

            button.Touch += (sender, q) => SetBackgroundColor(q.Event.Action);
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
                (Control as Android.Widget.Button).Touch -= (sender, q) => SetBackgroundColor(q.Event.Action);

            base.Dispose(disposing);
        }

        private void SetBackgroundColor(MotionEventActions eventAction)
        {
            var button = Control as Android.Widget.Button;
            var currentColor = button.Background as ColorDrawable;

            if (eventAction == MotionEventActions.Down)
            {
                button.SetBackgroundColor(_pressedColor);
            }
            else if (eventAction == MotionEventActions.Up)
            {
                ((IButtonController)Element)?.SendClicked();
                button.SetBackgroundColor(_backgroundColor);
            }
            else if (eventAction == MotionEventActions.Cancel)
            {
                button.SetBackgroundColor(_backgroundColor);
            }
        }
    }
}