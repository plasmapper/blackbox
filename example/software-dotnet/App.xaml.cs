using System.Windows;
using System.Windows.Threading;

namespace TestDevice
{
    public partial class App : Application
    {
        private readonly PL.BlackBox.ModbusClient _client;
        private readonly Model.SystemModel _systemModel;
        private readonly Thread _monitorThread;
        private bool _stopMonitorThread = false;

        public App()
        {
            _client = new(new System.IO.Ports.SerialPort());
            _client.ReadTimeout = 1000;
            _systemModel = new(_client);
            _monitorThread = new Thread(HardwareMonitor);
        }

        internal static string WindowHeader { get; } = "Test Device";

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                _monitorThread.Start();

                new View.MainWindow(new ViewModel.MainWindowViewModel(this, _systemModel)).Show();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Current.Shutdown();
            }
        }

        internal void HardwareMonitor()
        {
            while (!_stopMonitorThread)
            {
                try
                {
                    // Get serial port names
                    var portNames = System.IO.Ports.SerialPort.GetPortNames().ToList();
                    if (!portNames.SequenceEqual(_systemModel.Connection.SerialPortNames.Value))
                        _systemModel.Connection.SerialPortNames.Value = portNames;

                    // Read device configuration
                    var deviceConfiguration = _client.ReadDeviceConfiguration();
                    _systemModel.DeviceName.Value = deviceConfiguration.Name;

                    // Read device state
                    var deviceState = _client.ReadDeviceState();
                    _systemModel.HardwareNameAndVersion.Value = $"{deviceState.HardwareInfo.Name} v{deviceState.HardwareInfo.Version.Major}." +
                        $"{deviceState.HardwareInfo.Version.Minor}.{deviceState.HardwareInfo.Version.Patch}";
                    _systemModel.HardwareUid.Value = deviceState.HardwareInfo.Uid;
                    _systemModel.FirmwareNameAndVersion.Value = $"{deviceState.FirmwareInfo.Name} v{deviceState.FirmwareInfo.Version.Major}." +
                        $"{deviceState.FirmwareInfo.Version.Minor}.{deviceState.FirmwareInfo.Version.Patch}";

                    // Read parameters
                    var parameters = _client.ReadHoldingRegisters(1000, 3);
                    _systemModel.Parameter1.Value = parameters[0];
                    _systemModel.Parameter2.Value = parameters[1];
                    _systemModel.Parameter3.Value = parameters[2];

                    // Read inputs
                    var inputs = _client.ReadInputRegisters(1000, 3);
                    _systemModel.Input1.Value = inputs[0];
                    _systemModel.Input2.Value = inputs[1];
                    _systemModel.Input3.Value = inputs[2];

                    _systemModel.Exception.Value = null;
                }
                catch (Exception e)
                {
                    try
                    {
                        _systemModel.Exception.Value = e;   
                    }
                    catch { }
                }
                Thread.Sleep(100);
            }

            try
            {
                _client.Dispose();
            }
            catch { }
        }

        internal static void UserCommand(Action command)
        {
            try
            {
                command();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        internal static MessageBoxResult ShowError(string text) =>
            MessageBox.Show(text, App.WindowHeader, MessageBoxButton.OK, MessageBoxImage.Error);

        internal static MessageBoxResult ShowError(Exception exception) =>
            MessageBox.Show(ExceptionToString(exception), App.WindowHeader, MessageBoxButton.OK, MessageBoxImage.Error);

        protected override void OnExit(ExitEventArgs e)
        {
            _stopMonitorThread = true;
            base.OnExit(e);
        }

        private static string ExceptionToString(Exception? exception)
        {
            string exceptionString = "";
            if (exception != null)
            {
                if (exception is AggregateException ae)
                    exceptionString = string.Join(" ", ae.Flatten().InnerExceptions.Select((ie) => ie.Message));
                else
                    exceptionString = exception.Message;
            }
            return exceptionString;
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowError(e.Exception);
            e.Handled = true;
        }
    }
}
