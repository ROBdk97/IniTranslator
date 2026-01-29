using System.Globalization;
using System.Windows.Data;

namespace IniTranslator.Helpers
{
    public sealed class PasswordPairConverter : IMultiValueConverter
    {
        public PasswordPairConverter() { }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var google = values.Length > 0 ? values[0] as string : null;
            var deepL = values.Length > 1 ? values[1] as string : null;

            return new PasswordPair
            {
                GoogleApiKey = google,
                DeepLApiKey = deepL
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
