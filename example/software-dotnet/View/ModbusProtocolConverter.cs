using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts Modbus protocol to string.
    /// </summary>
    internal class ModbusProtocolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PL.Modbus.Protocol.Rtu => "RTU",
                PL.BlackBox.ModbusProtocol.Rtu => "RTU",
                PL.Modbus.Protocol.Ascii => "ASCII",
                PL.BlackBox.ModbusProtocol.Ascii => "ASCII",
                PL.Modbus.Protocol.Tcp => "TCP",
                PL.BlackBox.ModbusProtocol.Tcp => "TCP",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
