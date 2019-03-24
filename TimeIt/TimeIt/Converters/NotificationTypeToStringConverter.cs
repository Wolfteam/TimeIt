using System;
using System.Globalization;
using TimeIt.Enums;
using Xamarin.Forms;

namespace TimeIt.Converters
{
    public class NotificationTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notificationType = (NotificationType)value;
            switch (notificationType)
            {
                case NotificationType.VOICE:
                    return "Voice";
                case NotificationType.TOAST:
                    return "Toast";
                case NotificationType.BOTH:
                    return "Both";
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, $"The provided notificaion type is not valid");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
