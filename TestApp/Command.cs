using System;
using System.Windows.Input;

namespace Buschmann.Windows.TestApp
{
    public class Command : ICommand
    {
        private bool _currentCanExecute = true;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public Command(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? delegate { };
            _canExecute = canExecute ?? (_ => true);
        }

        public void Execute(object parameter)
        {
            if (_canExecute(parameter))
                _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            bool canExecute = _canExecute(parameter);

            if (canExecute != _currentCanExecute)
            {
                _currentCanExecute = canExecute;                
                CanExecuteChanged(this, EventArgs.Empty);
            }

            return canExecute;
        }

        public event EventHandler CanExecuteChanged = delegate { };
    }
}