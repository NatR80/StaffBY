using System;
using System.Globalization;
using System.Windows.Data;

namespace StaffBY.App.Converters
{
    /// <summary>
    /// Конвертер для инвертирования булевого значения
    /// </summary>
    public class BoolToOppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return true;
        }
    }
}