using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ColorMixer.Converters
{
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var thickness = (Thickness)value;

            if (thickness.Left == thickness.Top &&
                thickness.Top == thickness.Right &&
                thickness.Right == thickness.Bottom)
            {
                return thickness.Left;
            }
            else
            {
                throw new InvalidOperationException("Thickness must have " +
                                                    "uniform length on all sides.");
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return new Thickness((double)value);
        }
    }
}