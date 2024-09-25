using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace TestDevice.View
{
    /// <summary>
    /// View utilities.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Finds all logical children of a parent element.
        /// </summary>
        /// <typeparam name="T">Child element type.</typeparam>
        /// <param name="parent">Parent element.</param>
        /// <returns>Logical children.</returns>
        internal static List<T> FindLogicalChildren<T>(DependencyObject parent)
        {
            List<T> controls = new();
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is T t)
                    controls.Add(t);
                if (child is DependencyObject dependencyObject)
                    controls.AddRange(FindLogicalChildren<T>(dependencyObject));
            }

            return controls;
        }

        /// <summary>
        /// Adds the default event handlers to text boxes.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        internal static void AddDefautTextBoxEventHandlers(UIElement parent)
        {
            // Find dummy control with width = height = 0
            var dummyControls = FindLogicalChildren<Control>(parent).Where(e => e.Width == 0 && e.Height == 0);
            if (!dummyControls.Any())
                return;

            var dummyControl = dummyControls.First();
            
            // Add default text box event handlers
            FindLogicalChildren<TextBox>(parent).Where(textBox => !textBox.IsReadOnly).ToList().ForEach(textBox =>
            {
                // Update target on lost focus (undo edit on error)
                textBox.LostFocus += (s, e) => ((TextBox)s).GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();

                textBox.KeyDown += (s, e) =>
                {
                    // Lose focus on Enter
                    if (e.Key == Key.Enter)
                        dummyControl.Focus();

                    // Undo edit on Esc
                    if (e.Key == Key.Escape)
                    {
                        ((TextBox)s).GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
                        dummyControl.Focus();
                    }
                };
            });

            // Lose focus on mouse down
            parent.MouseDown += (s, e) => dummyControl.Focus();
        }
    }
}
