using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MESBlastBlockGenerator.Converters
{
    public class IntValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && int.TryParse(str, out int result))
                return result;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
                return i.ToString();
            return string.Empty;
        }
    }
}