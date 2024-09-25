using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts stop bits to string.
    /// </summary>
    internal class StopBitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                System.IO.Ports.StopBits.One => "1",
                PL.BlackBox.UartStopBits.One => "1",
                System.IO.Ports.StopBits.OnePointFive => "1.5",
                PL.BlackBox.UartStopBits.OnePointFive => "1.5",
                System.IO.Ports.StopBits.Two => "2",
                PL.BlackBox.UartStopBits.Two => "2",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
