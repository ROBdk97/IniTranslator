using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IniTranslator.Converter
{
    public class ListViewItemCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is System.Collections.ICollection collection ? collection.Count : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
