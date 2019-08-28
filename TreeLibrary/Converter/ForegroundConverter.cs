using System;
using System.Windows.Data;
using System.Windows.Media;

namespace TreeLibrary.Converter
{
    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new SolidColorBrush(Colors.Black);
            return new SolidColorBrush((Color) ColorConverter.ConvertFromString(value.ToString()));
        }


        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}