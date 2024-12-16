
using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace SlaveMachine.Resources.Converters;

public class BoolOnOffLine : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine(parameter);
        return (bool)value ? "ONLINE" : "OFFLINE";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
