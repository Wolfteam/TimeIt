using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerPage : ContentPage
    {
        private double _width;
        private double _height;

        public TimerPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                bool isInLandscape = width > height;

                string state = isInLandscape ? "Landscape" : "Portrait";

                if (isInLandscape)
                {
                    ListViewStackLayout.Children[0].IsVisible = false;
                }
                else
                {
                    ListViewStackLayout.Children[0].IsVisible = true;
                }

                VisualStateManager.GoToState(MainGrid, state);
                VisualStateManager.GoToState(NameAndRepetitionGrid, state);
                VisualStateManager.GoToState(ListViewStackLayout, state);
                VisualStateManager.GoToState(IntervalTotalTimeStackLayout, state);
            }
        }
    }
}