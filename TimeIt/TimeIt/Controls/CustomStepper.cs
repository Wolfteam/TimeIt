using System;
using TimeIt.Models;
using Xamarin.Forms;

namespace TimeIt.Controls
{
    public class CustomStepper : FlexLayout
    {
        private Button PlusBtn;
        private Button MinusBtn;
        private Label Entry;

        public static readonly BindableProperty StepperPositionProperty = BindableProperty.Create(
            propertyName: nameof(StepperPosition),
            returnType: typeof(int),
            declaringType: typeof(CustomStepper),
            defaultValue: 1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                var stepper = bindable as CustomStepper;

                int value = (int)newValue;
                if (value == stepper.Maximum)
                    stepper.PlusBtn.IsEnabled = false;
                if (value == stepper.Minimum)
                    stepper.MinusBtn.IsEnabled = false;
                if (value != stepper.Maximum && !stepper.PlusBtn.IsEnabled)
                    stepper.PlusBtn.IsEnabled = true;
                if (value != stepper.Minimum && !stepper.MinusBtn.IsEnabled)
                    stepper.MinusBtn.IsEnabled = true;
            });
        public int StepperPosition
        {
            get { return (int)GetValue(StepperPositionProperty); }
            set
            {
                SetValue(StepperPositionProperty, value);
            }
        }

        public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
            propertyName: nameof(Minimum),
            returnType: typeof(int),
            declaringType: typeof(CustomStepper),
            defaultValue: 0,
            defaultBindingMode: BindingMode.OneWay);
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
            propertyName: nameof(Maximum),
            returnType: typeof(int),
            declaringType: typeof(CustomStepper),
            defaultValue: 100,
            defaultBindingMode: BindingMode.OneWay);
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly BindableProperty IsButtonsEnabledProperty = BindableProperty.Create(
            propertyName: nameof(IsButtonsEnabled),
            returnType: typeof(bool),
            declaringType: typeof(CustomStepper),
            defaultValue: true,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                var stepper = (bindable as CustomStepper);
                stepper.MinusBtn.IsEnabled =
                    stepper.PlusBtn.IsEnabled = (bool)newValue;
            });
        public bool IsButtonsEnabled
        {
            get { return (bool)GetValue(IsButtonsEnabledProperty); }
            set
            {
                SetValue(IsButtonsEnabledProperty, value);
            }
        }

        public CustomStepper()
        {
            double fontSize = Device.GetNamedSize(
                    Device.RuntimePlatform == Device.Android
                    ? NamedSize.Large
                    : NamedSize.Medium,
                typeof(Button));

            var iconSize = (OnPlatform<double>)Application.Current.Resources["FontAwesomeIconSize"];
            var fontResource = (OnPlatform<string>)Application.Current.Resources["FontAwesomeSolid"];
            PlusBtn = new Button
            {
                BackgroundColor = Color.Transparent,
                FontSize = fontSize,
                FontAttributes = FontAttributes.Bold,
                ImageSource = new FontImageSource()
                {
                    FontFamily = fontResource,
                    Glyph = FontAwesome5SolidIcons.PlusCircle,
                    Color = Color.Red,
                    Size = Device.RuntimePlatform == Device.Android ? iconSize : iconSize / 2
                }
            };

            MinusBtn = new Button
            {
                BackgroundColor = Color.Transparent,
                FontSize = fontSize,
                FontAttributes = FontAttributes.Bold,
                ImageSource = new FontImageSource()
                {
                    FontFamily = fontResource,
                    Glyph = FontAwesome5SolidIcons.MinusCircle,
                    Color = Color.Red,
                    Size = Device.RuntimePlatform == Device.Android ? iconSize : iconSize / 2
                }
            };

            PlusBtn.Clicked += PlusBtnClicked;
            MinusBtn.Clicked += MinusBtnClicked;
            Entry = new Label
            {
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = fontSize,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsEnabled = false
            };

            Entry.SetBinding(Label.TextProperty, new Binding(nameof(StepperPosition), BindingMode.OneWay, source: this));
            //SetBasis(Entry, new FlexBasis(0.4f,true));
            Children.Add(MinusBtn);
            Children.Add(Entry);
            Children.Add(PlusBtn);
        }

        private void MinusBtnClicked(object sender, EventArgs e)
        {
            int newValue = StepperPosition - 1;
            if (newValue >= Minimum)
                StepperPosition--;
        }

        private void PlusBtnClicked(object sender, EventArgs e)
        {
            int newValue = StepperPosition + 1;
            if (newValue <= Maximum)
                StepperPosition++;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width < 0)
                return;

            foreach (var view in Children)
            {
                view.WidthRequest = width / 3;
            }
        }
    }
}
