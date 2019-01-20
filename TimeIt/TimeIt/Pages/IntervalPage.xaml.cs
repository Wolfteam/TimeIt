using Amporis.Xamarin.Forms.ColorPicker;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntervalPage : ContentPage
    {
        public IntervalPage()
        {
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    colorPickerAndroid.RootContainer = editorGrid;
                    colorPickerAndroid.DialogSettings = new ColorDialogSettings
                    {
                        DialogColor = Color.Gray
                    };
                    break;
                default:
                    colorPickerUWP.RootContainer = editorGrid;
                    colorPickerUWP.DialogSettings = new ColorDialogSettings
                    {
                        DialogColor = Color.Gray
                    };
                    break;
            }

        }
    }
}