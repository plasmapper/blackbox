using System.Windows.Input;

namespace TestDevice.ViewModel
{
    internal class Connection
    {
        public Connection(Model.Connection connection)
        {
            Interface = new(connection.Interface);
            InterfaceIsSerial = new Parameter<bool, PL.Modbus.Interface>(connection.Interface, value => value == PL.Modbus.Interface.Serial);
            InterfaceIsNetwork = new Parameter<bool, PL.Modbus.Interface>(connection.Interface, value => value == PL.Modbus.Interface.Network);

            SerialPortNames = new(connection.SerialPortNames);
            SerialPortName = new(connection.SerialPortName);
            SerialBaudRate = new(connection.SerialBaudRate);
            SerialDataBits = new(connection.SerialDataBits);
            SerialParity = new(connection.SerialParity);
            SerialStopBits = new(connection.SerialStopBits);
            SerialFlowControl = new(connection.SerialFlowControl);
            SerialProtocol = new(connection.SerialProtocol);
            SerialStationAddress = new(connection.SerialStationAddress);

            NetworkAddress = new(connection.NetworkAddress);
            NetworkPort = new(connection.NetworkPort);
            NetworkStationAddress = new(connection.NetworkStationAddress);
            ServiceName = new(connection.ServiceName);
            FindService = new Command(connection.FindService);
        }

        public List<PL.Modbus.Interface> InterfaceValues { get; } = new() { PL.Modbus.Interface.Serial, PL.Modbus.Interface.Network };
        public Parameter<PL.Modbus.Interface> Interface { get; }
        public Parameter<bool> InterfaceIsSerial { get; }
        public Parameter<bool> InterfaceIsNetwork { get; }

        public Parameter<List<string>> SerialPortNames { get; }
        public Parameter<string> SerialPortName { get; }
        public Parameter<int> SerialBaudRate { get; }
        public Parameter<int> SerialDataBits { get; }
        public List<System.IO.Ports.Parity> SerialParityValues { get; } = new() { System.IO.Ports.Parity.None, System.IO.Ports.Parity.Even, System.IO.Ports.Parity.Odd };
        public Parameter<System.IO.Ports.Parity> SerialParity { get; }
        public List<System.IO.Ports.StopBits> SerialStopBitsValues { get; } = new() { System.IO.Ports.StopBits.One, System.IO.Ports.StopBits.OnePointFive, System.IO.Ports.StopBits.Two };
        public Parameter<System.IO.Ports.StopBits> SerialStopBits { get; }
        public List<System.IO.Ports.Handshake> SerialFlowControlValues { get; } = new() { System.IO.Ports.Handshake.None, System.IO.Ports.Handshake.RequestToSend };
        public Parameter<System.IO.Ports.Handshake> SerialFlowControl { get; }
        public List<PL.Modbus.Protocol> SerialProtocolValues { get; } = new() { PL.Modbus.Protocol.Rtu, PL.Modbus.Protocol.Ascii };
        public Parameter<PL.Modbus.Protocol> SerialProtocol { get; }
        public Parameter<byte> SerialStationAddress { get; }

        public Parameter<string> NetworkAddress { get; }
        public Parameter<ushort> NetworkPort { get; }
        public Parameter<byte> NetworkStationAddress { get; }
        public Parameter<string> ServiceName { get; }
        public ICommand FindService { get; }
    }
}