using CommunityToolkit.Mvvm.ComponentModel;

namespace TestDevice.ViewModel
{
    /// <summary>
    /// View model parameter class.
    /// </summary>
    /// <typeparam name="T">Viewmodel value type.</typeparam> 
    internal partial class Parameter<T> : ObservableObject
    {
        private bool _valueChangedFromModel = false;
        [ObservableProperty]
        private T _value;
        private readonly Func<T> _viewModelValueGenerator;
        private readonly Action<T> _modelValueSetter;

        /// <summary>
        /// Initializes a new instance of the Parameter class with binding to multiple model parameters.
        /// </summary>
        /// <param name="modelParameters">Model parameters.</param>
        /// /// <param name="viewModelValueGenerator">View model value generator.</param>
        /// /// <param name="modelValueSetter">Model value setter.</param>
        public Parameter(List<Model.Parameter> modelParameters, Func<T> viewModelValueGenerator, Action<T> modelValueSetter)
        {
            _viewModelValueGenerator = viewModelValueGenerator;
            _modelValueSetter = modelValueSetter;
            _value = viewModelValueGenerator();

            modelParameters.ForEach(p => p.ValueChanged += OnModelValueChanged);
        }

        /// <summary>
        /// Initializes a new instance of the Parameter class with binding to multiple model parameters.
        /// </summary>
        /// <param name="modelParameters">Model parameters.</param>
        /// /// <param name="viewModelValueGenerator">View model value generator.</param>
        public Parameter(List<Model.Parameter> modelParameters, Func<T> viewModelValueGenerator) :
            this(modelParameters, viewModelValueGenerator, value => throw new NotImplementedException()) { }

        /// <summary>
        /// Initializes a new instance of the Parameter class with binding to a single model parameter of the same value type.
        /// </summary>
        /// <param name="modelParameter">Model parameter.</param>
        public Parameter(Model.Parameter<T> modelParameter) :
            this(new List<Model.Parameter>() { modelParameter },
                () => modelParameter.Value,
                value => modelParameter.SetValue(value)) { }

        /// <summary>
        /// Initializes a new instance of the Parameter class without binding to a model parameter.
        /// </summary>
        /// <param name="initialValue">Initial value.</param>
        public Parameter(T initialValue) : this(new Model.Parameter<T>(initialValue)) { }

        /// <summary>
        /// Parameter value changed event.
        /// </summary>
        public event EventHandler? ValueChanged;

        protected virtual void OnValueChanged(EventArgs e) => ValueChanged?.Invoke(this, e);

        partial void OnValueChanged(T? oldValue, T newValue)
        {
            if (_valueChangedFromModel)
                return;

            try
            {
                _modelValueSetter(newValue);
            }
            catch (Exception e)
            {
                App.ShowError(e);
            }
            finally
            {
                OnModelValueChanged(this, EventArgs.Empty);
            }
        }

        private void OnModelValueChanged(object? sender, EventArgs e)
        {
            _valueChangedFromModel = true;

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    Value = _viewModelValueGenerator();
                    OnValueChanged(EventArgs.Empty);
                });
            }
            catch { }

            _valueChangedFromModel = false;
        }
    }

    /// <summary>
    /// View model parameter class with binding to a single model parameter.
    /// </summary>
    /// <typeparam name="T">Viewmodel value type.</typeparam> 
    internal partial class Parameter<ViewModelType, ModelType> : Parameter<ViewModelType>
    {
        /// <summary>
        /// Initializes a new instance of the Parameter class with binding to a single model parameter.
        /// </summary>
        /// <param name="modelParameter">Model parameter.</param>
        /// <param name="modelToViewModelConverter">Model to view model converter.</param>
        /// <param name="viewModelToModelConverter">View model to model converter.</param>
        public Parameter(Model.Parameter<ModelType> modelParameter,
            Converter<ModelType, ViewModelType> modelToViewModelConverter,
            Converter<ViewModelType, ModelType> viewModelToModelConverter) :
            base(new() { modelParameter },
                () => modelToViewModelConverter(modelParameter.Value),
                value => modelParameter.SetValue(viewModelToModelConverter(value))) { }

        /// <summary>
        /// Initializes a new instance of the Parameter class with binding to a single model parameter.
        /// </summary>
        /// <param name="modelParameter">Model parameter.</param>
        /// <param name="modelToViewModelConverter">Model to view model converter.</param>
        public Parameter(Model.Parameter<ModelType> modelParameter,
            Converter<ModelType, ViewModelType> modelToViewModelConverter) :
            this (modelParameter, modelToViewModelConverter, value => throw new NotImplementedException()) { }
    }
}