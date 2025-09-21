using System;
using System.Windows.Input;

namespace iCos5.BACnet.MVVM
{
  public class CommandBase : ICommand
  {
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public CommandBase(Action<object> execute, Predicate<object> canExecute)
    {
      if (execute == null)
        throw new NullReferenceException("execute can not null");

      _execute = execute;
      _canExecute = canExecute;
    }

    public CommandBase(Action<object> execute) : this(execute, null)
    {

    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _execute.Invoke(parameter);
    }
  }
}
