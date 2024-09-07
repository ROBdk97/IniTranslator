using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IniTranslator.Converter
{
    public class ColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualWidth && parameter is string percentageString &&
                double.TryParse(percentageString, out double percentage))
            {
                // Subtract 10 for some padding/margin space (adjust as needed)
                return (actualWidth - 10) * percentage;
            }
            return 100; // Default width in case of binding failure
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
