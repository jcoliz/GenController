using System;
using System.Windows.Input;

namespace Common
{
    /// <summary>
    /// Quick and reusable ICommand
    /// </summary>
    /// <remarks>
    /// Move me to manialabs portable base
    /// </remarks>
    /// <seealso>
    /// http://www.wpftutorial.net/delegatecommand.html
    /// </seealso>
    public class DelegateCommand : ICommand
    {
        Func<object, bool> canExecute;
        Action<object> executeAction;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));

            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public void Execute(object parameter) => executeAction?.Invoke(parameter);
    }
}
