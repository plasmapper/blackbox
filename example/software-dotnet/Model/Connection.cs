using System.Net;
using System.Text.RegularExpressions;

namespace TestDevice.Model
{
    internal class Connection
    {
        public Connection() { }

        public Parameter<PL.Modbus.Interface> Interface { get; } = new(PL.Modbus.Interface.Serial);

        public Parameter<List<string>> SerialPortNames { get; } = new(new());
        public Parameter<string> SerialPortName { get; } = new("");
        public Parameter<int> SerialBaudRate { get; } = new(115200);
        public Parameter<int> SerialDataBits { get; } = new(8);
        public Parameter<System.IO.Ports.Parity> SerialParity { get; } = new(System.IO.Ports.Parity.Even);
        public Parameter<System.IO.Ports.StopBits> SerialStopBits { get; } = new(System.IO.Ports.StopBits.One);
        public Parameter<System.IO.Ports.Handshake> SerialFlowControl { get; } = new(System.IO.Ports.Handshake.None);
        public Parameter<PL.Modbus.Protocol> SerialProtocol { get; } = new(PL.Modbus.Protocol.Rtu);
        public Parameter<byte> SerialStationAddress { get; } = new(1);

        public Parameter<string> NetworkAddress { get; } = new("192.168.1.1");
        public Parameter<ushort> NetworkPort { get; } = new(502);
        public Parameter<byte> NetworkStationAddress { get; } = new(255);
        public Parameter<string> ServiceName { get; } = new("TestDevice");

        public void FindService()
        {
            string? hostName = null;
            IPAddress? ipAddress = null;
            ushort? port = null;

            var mdns = new Makaretu.Dns.MulticastService();
            var serviceDiscovery = new Makaretu.Dns.ServiceDiscovery(mdns);

            // Find all services
            mdns.NetworkInterfaceDiscovered += (s, e) => serviceDiscovery.QueryAllServices();

            // Find all service instances
            serviceDiscovery.ServiceDiscovered += (s, e) => mdns.SendQuery(e, type: Makaretu.Dns.DnsType.PTR);

            serviceDiscovery.ServiceInstanceDiscovered += (s, e) =>
            {
                string? instanceName = e.ServiceInstanceName.Labels.Count == 0 ? null : e.ServiceInstanceName.Labels[0];
                if (instanceName is not null && instanceName.Contains(ServiceName.Value))
                    mdns.SendQuery(e.ServiceInstanceName, type: Makaretu.Dns.DnsType.SRV);
            };

            mdns.AnswerReceived += (s, e) =>
            {
                foreach (var answer in e.Message.Answers)
                {
                    if (answer is Makaretu.Dns.SRVRecord srvRecord)
                    {
                        string? instanceName = srvRecord.Name.Labels.Count == 0 ? null : srvRecord.Name.Labels[0];

                        if (instanceName is not null && instanceName.Contains(ServiceName.Value))
                        {
                            hostName = srvRecord.Target.ToString();
                            port = srvRecord.Port;
                            mdns.SendQuery(srvRecord.Target, type: Makaretu.Dns.DnsType.A);
                        }
                    }

                    if (answer is Makaretu.Dns.AddressRecord addressRecord
                        && addressRecord.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && addressRecord.Name.ToString() == hostName)
                    {
                        ipAddress = addressRecord.Address;
                    }
                }
            };

            mdns.Start();
            Thread.Sleep(5000);
            mdns.Stop();
            mdns.Dispose();

            if (ipAddress is not null && port is not null)
            {
                NetworkAddress.Value = ipAddress.ToString();
                NetworkPort.Value = port ?? 0;
            }
        }
    }
}