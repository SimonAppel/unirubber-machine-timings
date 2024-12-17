using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SlaveMachine.Resources.Converters;

public class IntToStr : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return $"{value}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
