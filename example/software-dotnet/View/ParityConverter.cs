using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts parity to string.
    /// </summary>
    internal class ParityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                System.IO.Ports.Parity.None => "None",
                PL.BlackBox.UartParity.None => "None",
                System.IO.Ports.Parity.Even => "Even",
                PL.BlackBox.UartParity.Even => "Even",
                System.IO.Ports.Parity.Odd => "Odd",
                PL.BlackBox.UartParity.Odd => "Odd",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
