using ColorMixer.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ColorMixer.Converters
{
    public class OperationTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var operation = (OperationType)value;

            switch (operation)
            {
                case OperationType.Addition:
                    return "\xE710";

                case OperationType.Subtraction:
                    return "\xE738";

                default:
                    throw new InvalidOperationException($"Unknown operation type '{operation}'.");
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}