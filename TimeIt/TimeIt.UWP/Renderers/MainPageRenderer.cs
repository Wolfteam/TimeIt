using System;
using TimeIt;
using TimeIt.UWP.Renderers;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

//[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace TimeIt.UWP.Renderers
{
    class MainPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
                {
                    AcrylicBrush myBrush = new AcrylicBrush
                    {
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                        TintColor = Windows.UI.Color.FromArgb(255, 200, 200, 200),
                        TintOpacity = 0.2
                    };

                    Background = myBrush;
                }
                else
                {
                    SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 240, 240, 240));

                    Background = myBrush;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

    }
}
