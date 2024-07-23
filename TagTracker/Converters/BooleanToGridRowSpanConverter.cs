using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TagTracker.Converters;

public class BooleanToGridRowSpanConverter : IValueConverter
{
    public object Convert(object?     value,
                          Type        targetType,
                          object?     parameter,
                          CultureInfo culture)
    {
        if (value is bool isVisible)
            return isVisible ? 1 : 3;

        throw new InvalidOperationException("Value must be a boolean");
    }

    public object ConvertBack(object?     value,
                              Type        targetType,
                              object?     parameter,
                              CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
