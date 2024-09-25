using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestDevice.View
{
    /// <summary>
    /// View converter that converts boolean to Visibility.Visible (if true) or Visibility.Collapsed (if false).
    /// </summary>
    internal class BoolVisibleCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool?)value == true ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }            

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
