using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts view model state to brush.
    /// </summary>
    internal class StateBrushConverter : IValueConverter
    {
        /// <summary>
        /// Off state brush.
        /// </summary>
        public Brush OffBrush { get; set; } = default!;
        /// <summary>
        /// On state brush.
        /// </summary>
        public Brush OnBrush { get; set; } = default!;
        /// <summary>
        /// Warning state brush.
        /// </summary>
        public Brush WarningBrush { get; set; } = default!;
        /// <summary>
        /// Fault state brush.
        /// </summary>
        public Brush FaultBrush { get; set; } = default!;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewModel.State state)
            {
                return state switch
                {
                    ViewModel.State.On => OnBrush,
                    ViewModel.State.Warning => WarningBrush,
                    ViewModel.State.Fault => FaultBrush,
                    _ => OffBrush
                };
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
