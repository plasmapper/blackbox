namespace TestDevice.Model
{
    /// <summary>
    /// Abstract model parameter class.
    /// </summary>
    internal abstract class Parameter
    {
        /// <summary>
        /// Parameter value changed event.
        /// </summary>
        public event EventHandler? ValueChanged;

        protected virtual void OnValueChanged(EventArgs e) => ValueChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Model parameter class.
    /// </summary>
    /// <typeparam name="T">Parameter value type.</typeparam>
    internal class Parameter<T> : Parameter
    {
        private T _value;
        private readonly Func<T, T> _valueSetter;

        /// <summary>
        /// Initializes a new instance of the Parameter class.
        /// </summary>
        /// <param name="initialValue">Initial value.</param>
        /// /// <param name="valueSetter">Parameter value setter that returns the actual set value.</param>
        public Parameter(T initialValue, Func<T, T> valueSetter)
        {
            _value = initialValue;
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Initializes a new instance of the Parameter class with default (assignment) value setter.
        /// </summary>
        /// <param name="initialValue">Initial value.</param>
        public Parameter(T initialValue) : this(initialValue, value => value) { }

        /// <summary>
        /// Gets and sets (assigns) the parameter value.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                T oldValue = _value;
                _value = value;

                if (!Equals(oldValue, _value))
                    OnValueChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the parameter value using the value setter.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetValue(T value) => Value = _valueSetter(value);
    }
}
