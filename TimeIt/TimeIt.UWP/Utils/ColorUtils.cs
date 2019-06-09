using System;
using Windows.UI;

namespace TimeIt.UWP.Helpers
{
    public class ColorUtils
    {
        public static Color GetColor(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            if (hex.Length > 6)
            {
                byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                var r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
                var g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
                var b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
        }
    }
}
