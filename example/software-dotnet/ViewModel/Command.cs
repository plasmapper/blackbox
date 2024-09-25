using System.Windows.Input;

namespace TestDevice.ViewModel
{
    /// <summary>
    /// View model command.
    /// </summary>
    internal partial class Command : ICommand
    {
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        /// <param name="action">Command action.</param>
        public Command(Action action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => App.UserCommand(_action);
    }
}