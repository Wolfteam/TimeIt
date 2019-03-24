using System;
using TimeIt.Delegates;
using TimeIt.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.Pages.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SliderDialog : ContentView, IConfirmationDialog<double>
    {
        private readonly double _steps;
        public OnConfirmDialogButtonClick<double> OnOptionSelected { get; set; }

        public SliderDialog(string title, double min, double max, double steps, double currentValue)
        {
            InitializeComponent();
            titleLabel.Text = title;
            currentValueLabel.Text = $"{currentValue}";
            _steps = steps;
            slider.Maximum = max;
            slider.Minimum = min;
            slider.Value = currentValue;
        }

        private void OkButton_Clicked(object sender, EventArgs e)
        {
            OnOptionSelected?.Invoke(slider.Value, false);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            OnOptionSelected?.Invoke(slider.Value, true);
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / _steps);

            slider.Value = newStep * _steps;
            currentValueLabel.Text = $"{slider.Value}";
        }
    }
}