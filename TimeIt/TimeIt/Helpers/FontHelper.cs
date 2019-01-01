using System;
using Xamarin.Forms;

namespace TimeIt.Helpers
{
    public class FontHelper
    {
        public static double GetNamedSize(int size)
        {
            var size2 = (NamedSize)Enum.Parse(typeof(NamedSize), $"{size}");
            return Device.GetNamedSize(size2, typeof(Label));
        }
    }
}
