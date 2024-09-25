using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts flow control to string.
    /// </summary>
    internal class FlowControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                System.IO.Ports.Handshake.None => "None",
                PL.BlackBox.UartFlowControl.None => "None",
                System.IO.Ports.Handshake.RequestToSend => "RTS",
                PL.BlackBox.UartFlowControl.Rts => "RTS",
                PL.BlackBox.UartFlowControl.Cts => "CTS",
                PL.BlackBox.UartFlowControl.RtsCts => "RTS/CTS",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
