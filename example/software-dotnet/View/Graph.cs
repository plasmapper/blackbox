using ScottPlot;
using System.Runtime.Versioning;

namespace TestDevice.View
{
    public class Graph : ScottPlot.WPF.WpfPlot
    {
        private const double _defaultMin = -10;
        private const double _defaultMax = 10;

        public Graph() : base()
        {
            Plot.HideAxesAndGrid();
        }

        [SupportedOSPlatform("windows")]
        internal void AutoScaleX()
        {
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (var plot in Plot.PlottableList)
            {
                if (plot is ScottPlot.Plottables.Signal signalPlot && signalPlot.IsVisible)
                {
                    min = Math.Min(min, signalPlot.Data.XOffset);
                    max = Math.Max(max, signalPlot.Data.XOffset + (signalPlot.Data.GetYs().Count - 1) * signalPlot.Data.Period);
                }
            }

            Plot.Axes.SetLimitsX(min == double.MaxValue ? _defaultMin : min, max == double.MinValue ? _defaultMax : max);
        }

        [SupportedOSPlatform("windows")]
        public void AutoScaleY()
        {
            double minX = Plot.Axes.GetLimits().Left;
            double maxX = Plot.Axes.GetLimits().Right;

            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (var plot in Plot.PlottableList)
            {
                if (plot is ScottPlot.Plottables.Signal signalPlot && signalPlot.IsVisible)
                {
                    var xOffset = signalPlot.Data.XOffset;
                    var xInterval = signalPlot.Data.Period;
                    var yValues = signalPlot.Data.GetYs();

                    if (signalPlot.Data.XOffset <= maxX && xOffset + (yValues.Count - 1) * xInterval >= minX)
                    {
                        int startIndex = Math.Max(0, Math.Min(yValues.Count - 1, (int)Math.Ceiling((minX - xOffset) / xInterval)));
                        int endIndex = Math.Max(0, Math.Min(yValues.Count - 1, (int)Math.Floor((maxX - xOffset) / xInterval)));
                        var visibleYValues = yValues.Skip(startIndex).Take(endIndex - startIndex + 1);
                        minY = Math.Min(minY, visibleYValues.Min());
                        maxY = Math.Max(maxY, visibleYValues.Max());
                    }
                }
            }

            if (minY != double.MaxValue && maxY != double.MinValue)
                Plot.Axes.SetLimitsY(minY - 0.1 * (maxY - minY), maxY + 0.1 * (maxY - minY));
            else
                Plot.Axes.SetLimitsY(_defaultMin, _defaultMax);
        }

        [SupportedOSPlatform("windows")]
        public void AutoScale()
        {
            AutoScaleX();
            AutoScaleY();
        }
    }
}
