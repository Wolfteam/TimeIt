
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.UserControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnackBar : TemplatedView
    {

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SnackBar), default(Color));
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty ButtonTextColorProperty = BindableProperty.Create(nameof(ButtonTextColor), typeof(Color), typeof(SnackBar), default(Color));
        public Color ButtonTextColor
        {
            get { return (Color)GetValue(ButtonTextColorProperty); }
            set { SetValue(ButtonTextColorProperty, value); }
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.Create(nameof(Message), typeof(string), typeof(SnackBar), default(string));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty CloseButtonTextProperty = BindableProperty.Create(nameof(CloseButtonText), typeof(string), typeof(SnackBar), "Close");
        public string CloseButtonText
        {
            get { return (string)GetValue(CloseButtonTextProperty); }
            set { SetValue(CloseButtonTextProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(float), typeof(SnackBar), default(float));
        public float FontSize
        {
            get { return (float)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(SnackBar), false, propertyChanged: IsOpenChanged);
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly BindableProperty SecondsToHideProperty = BindableProperty.Create(nameof(SecondsToHide), typeof(double), typeof(SnackBar), default(double));
        public double SecondsToHide
        {
            get { return (double)GetValue(SecondsToHideProperty); }
            set { SetValue(SecondsToHideProperty, value); }
        }


        public SnackBar()
        {
            InitializeComponent();
        }


        public async void Close()
        {
            await this.TranslateTo(0, 50, 500);
            Message = string.Empty;
            IsOpen = IsVisible = false;
        }

        public async void Open(string message)
        {
            IsVisible = true;
            Message = message;
            await this.TranslateTo(0, 0, 500);
        }

        private static void IsOpenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool isOpen;

            if (bindable != null && newValue != null)
            {
                var control = (SnackBar)bindable;
                isOpen = (bool)newValue;

                if (!control.IsOpen)
                {
                    control.Close();
                }
                else
                {
                    control.Open(control.Message);
                    Device.StartTimer(TimeSpan.FromSeconds(control.SecondsToHide == 0 ? 10 : control.SecondsToHide), () =>
                    {
                        control.Close();
                        return false;
                    });
                }
            }
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}