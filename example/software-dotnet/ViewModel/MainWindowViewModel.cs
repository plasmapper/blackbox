using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace TestDevice.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(App app, Model.SystemModel systemModel)
        {
            WindowHeader = App.WindowHeader;

            Connection = new(systemModel.Connection);
            Exception = new Parameter<string, Exception?>(systemModel.Exception, value => value is null ? "" : value.Message);

            HardwareNameAndVersion = new(systemModel.HardwareNameAndVersion);
            HardwareUid = new(systemModel.HardwareUid);
            FirmwareNameAndVersion = new(systemModel.FirmwareNameAndVersion);
            DeviceName = new(systemModel.DeviceName);

            Parameter1 = new(systemModel.Parameter1);
            Parameter2 = new(systemModel.Parameter2);
            Parameter3 = new(systemModel.Parameter3);
            Input1 = new(systemModel.Input1);
            Input2 = new(systemModel.Input2);
            Input3 = new(systemModel.Input3);

            GraphData = new(systemModel.GraphData);

            SaveAllConfigurations = new Command(systemModel.SaveAllConfigurations);
            ReadGraph = new Command(systemModel.ReadGraph);
        }

        public string WindowHeader { get; }

        public Connection Connection { get; }
        public Parameter<string> Exception { get; }

        public Parameter<string> HardwareNameAndVersion { get; }
        public Parameter<string> HardwareUid { get; }
        public Parameter<string> FirmwareNameAndVersion { get; }
        public Parameter<string> DeviceName { get; }

        public Parameter<ushort> Parameter1 { get; }
        public Parameter<ushort> Parameter2 { get; }
        public Parameter<ushort> Parameter3 { get; }

        public Parameter<ushort> Input1 { get; }
        public Parameter<ushort> Input2 { get; }
        public Parameter<ushort> Input3 { get; }

        public Parameter<List<double>> GraphData { get; }

        public ICommand SaveAllConfigurations { get; }
        public ICommand ReadGraph { get; }
    }
}
