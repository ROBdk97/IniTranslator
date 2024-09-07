using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IniTranslator.Converter
{
    public class ValueDifferenceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return false;  // Ensure there are at least 2 values to compare

            var value = values[0] as string;
            var newValue = values[1] as string;

            // Check for null values and compare strings
            return !string.Equals(value, newValue, StringComparison.Ordinal);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
