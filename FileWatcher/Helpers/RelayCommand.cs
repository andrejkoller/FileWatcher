using System.Windows.Input;

namespace FileWatcher.Helpers
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter is T typedParameter || parameter == null && default(T) == null)
            {
                var safeTypedParameter = (T?)parameter;
                return _canExecute?.Invoke(safeTypedParameter!) ?? true;
            }
            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T typedParameter || parameter == null && default(T) == null)
            {
                _execute((T?)parameter!);
            }
            else
            {
                throw new ArgumentException($"Invalid parameter type. Expected type {typeof(T)}.", nameof(parameter));
            }
        }
    }
}
