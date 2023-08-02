using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowsEnvironment.View;

internal class PanelSizeConverter : IValueConverter
{
    public static readonly PanelSizeConverter Instance = new();

    private PanelSizeConverter() { }

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var size = (double?)value;
        return size.HasValue ? new GridLength(size.Value, GridUnitType.Pixel) : new GridLength(1, GridUnitType.Star);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var length = (GridLength)value;
        return !length.IsStar ? length.Value : null;
    }
}
