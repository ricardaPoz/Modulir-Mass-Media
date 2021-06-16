using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Modulir_Mass_Media.Helpers
{
    class FillImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
