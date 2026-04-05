using System;
using System.Globalization;
using System.Windows.Data;

namespace StaffBY.App.Converters
{
    public class SalaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal salary)
                return salary.ToString("N2");
            return "0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && decimal.TryParse(str, out decimal result))
                return result;
            return 0m;
        }
    }
}