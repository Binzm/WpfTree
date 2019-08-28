using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TreeLibrary.Converter
{
    public class ImageNameToPhotoPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/images/{value}.png"))
                return null;

            if (value != null)
                return new BitmapImage(new Uri($@"pack://siteoforigin:,,,/images/{value}.png",
                    UriKind.RelativeOrAbsolute));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}