using System;
using System.Windows.Input;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // A simple ICommand implementation for relaying actions from the View to the ViewModel
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        // Manually trigger a re-evaluation of CanExecute.
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // The Generic version
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        // 1. Revert to simple event declaration
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            // Handle potential null/default value
            T typedParameter = (parameter is T) ? (T)parameter : default;

            // Check if the parameter can be correctly cast before running CanExecute
            // This prevents issues where a parameter of a wrong type is passed.
            bool isCastable = parameter is T || parameter == null && !typeof(T).IsValueType;

            return isCastable && _canExecute(typedParameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                T typedParameter = (parameter is T) ? (T)parameter : default;
                _execute(typedParameter);
            }
        }

        // 2. Add the manual RaiseCanExecuteChanged method
        /// <summary>
        /// Manually triggers a re-evaluation of CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}