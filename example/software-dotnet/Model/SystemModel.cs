namespace TestDevice.Model
{
    /// <summary>
    /// System model class.
    /// Properties are parameter model objects that implement setting logic.
    /// Methods implement some other logic apart from parameter setting.
    /// </summary>
    internal class SystemModel
    {
        private readonly PL.BlackBox.ModbusClient _client;
        private System.IO.Ports.SerialPort? _port;

        public SystemModel(PL.BlackBox.ModbusClient client)
        {
            _client = client;

            Parameter1 = new(0, value =>
            {
                _client.WriteSingleHoldingRegister(1000, value);
                return _client.ReadHoldingRegisters(1000, 1)[0];
            });
            Parameter2 = new(0, value =>
            {
                _client.WriteSingleHoldingRegister(1001, value);
                return _client.ReadHoldingRegisters(1001, 1)[0];
            });
            Parameter3 = new(0, value =>
            {
                _client.WriteSingleHoldingRegister(1002, value);
                return _client.ReadHoldingRegisters(1002, 1)[0];
            });

            List<Parameter> parameters = new() { Connection.Interface,
                Connection.SerialPortName, Connection.SerialBaudRate, Connection.SerialDataBits, Connection.SerialParity, Connection.SerialStopBits, Connection.SerialFlowControl,
                Connection.SerialProtocol, Connection.SerialStationAddress,
                Connection.NetworkAddress, Connection.NetworkPort, Connection.NetworkStationAddress };
            foreach (var parameter in parameters)
                parameter.ValueChanged += (s, e) => ChangeStream();

            ChangeStream();
        }

        public Connection Connection { get; } = new();
        public Parameter<Exception?> Exception { get; } = new(null);

        public Parameter<string> HardwareNameAndVersion { get; } = new("");
        public Parameter<string> HardwareUid { get; } = new("");
        public Parameter<string> FirmwareNameAndVersion { get; } = new("");
        public Parameter<string> DeviceName { get; } = new("");
        
        public Parameter<ushort> Parameter1 { get; }
        public Parameter<ushort> Parameter2 { get; }
        public Parameter<ushort> Parameter3 { get; }

        public Parameter<ushort> Input1 { get; } = new(0);
        public Parameter<ushort> Input2 { get; } = new(0);
        public Parameter<ushort> Input3 { get; } = new(0);

        public Parameter<List<double>> GraphData { get; } = new(new());

        public void SaveAllConfigurations() => _client.SaveAllConfigurations();

        public void ReadGraph()
        {
            List<double> graphData = new();
            var registers = _client.ReadInputRegisters(2000, 100);
            for (int i = 0; i < registers.Count / 2; i++)
                graphData.Add(BitConverter.UInt32BitsToSingle(registers[i * 2] + ((uint)registers[i * 2 + 1] << 16)));
            GraphData.Value = graphData;
        }

        private void ChangeStream()
        {
            lock(_client)
            {
                if (Connection.Interface.Value == PL.Modbus.Interface.Serial)
                {
                    _port?.Close();
                    _port = new System.IO.Ports.SerialPort();
                    if (Connection.SerialPortName.Value is not null && Connection.SerialPortName.Value != "")
                        _port.PortName = Connection.SerialPortName.Value;
                    _port.BaudRate = Connection.SerialBaudRate.Value;
                    _port.DataBits = Connection.SerialDataBits.Value;
                    _port.Parity = Connection.SerialParity.Value;
                    _port.StopBits = Connection.SerialStopBits.Value;
                    _port.Handshake = Connection.SerialFlowControl.Value;

                    _client.Stream = new PL.Modbus.SerialStream(_port);
                    _client.Protocol = Connection.SerialProtocol.Value;
                    _client.StationAddress = Connection.SerialStationAddress.Value;
                }
                else
                {
                    _client.Stream.Close();
                    _client.Stream = new PL.Modbus.NetworkStream(Connection.NetworkAddress.Value, Connection.NetworkPort.Value);
                    _client.Protocol = PL.Modbus.Protocol.Tcp;
                    _client.StationAddress = Connection.NetworkStationAddress.Value;
                }
            }
        }
    }
}