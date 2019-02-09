using System;
using System.Globalization;
using System.Windows.Data;

namespace CashManager.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.TryParse((string)value, out DateTime date) ? date : DateTime.MinValue;
        }

        #endregion
    }
}