using Android.Content;
using Android.Views;
using TimeIt.Controls;
using TimeIt.Droid.Remderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(IntervalListView), typeof(IntervalListViewRenderer))]
namespace TimeIt.Droid.Remderers
{
    /// <summary>
    /// For some reason, when the phone is in landscape mode,
    /// scroll does not work inside the listview..
    /// https://forums.xamarin.com/discussion/2857/listview-inside-scrollview
    /// </summary>
    public class IntervalListViewRenderer : ListViewRenderer
    {
        public IntervalListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.Touch -= Control_Touch;
            }

            if (e.NewElement != null)
            {
                Control.Touch += Control_Touch;
            }
        }

        private void Control_Touch(object sender, TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    // Disallow ScrollView to intercept touch events.
                    Parent.RequestDisallowInterceptTouchEvent(true);
                    break;

                case MotionEventActions.Up:
                    // Allow ScrollView to intercept touch events.
                    Parent.RequestDisallowInterceptTouchEvent(false);
                    break;
            }

            Control.OnTouchEvent(e.Event);
        }
    }
}