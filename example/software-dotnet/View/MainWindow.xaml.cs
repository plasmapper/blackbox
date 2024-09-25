using TestDevice.ViewModel;
using System.Windows;

namespace TestDevice.View
{
    public partial class MainWindow : Window
    {
        private readonly List<double> _graphData = new();
        private readonly ScottPlot.Plottables.Signal _plot;

        internal MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Utilities.AddDefautTextBoxEventHandlers(this);

            Graph.Interaction.Disable();
            _plot = Graph.Plot.Add.Signal(_graphData);
            _plot.MarkerSize = 0;
            viewModel.GraphData.ValueChanged += (s, e) =>
            {
                _graphData.Clear();
                _graphData.AddRange(viewModel.GraphData.Value);
                Graph.AutoScale();
                Graph.Refresh();
            };
        }
    }
}