using System;
using System.Globalization;
using Xamarin.Forms;

namespace TimeIt.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hexColor = (string)value;
            return Color.FromHex(hexColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
