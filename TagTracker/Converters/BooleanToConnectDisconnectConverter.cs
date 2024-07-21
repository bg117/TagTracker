using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TagTracker.Converters;

public class BooleanToConnectDisconnectConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is bool isConnected)
            return isConnected ? "Disconnect" : "Connect";

        throw new InvalidOperationException("Value must be a boolean");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}