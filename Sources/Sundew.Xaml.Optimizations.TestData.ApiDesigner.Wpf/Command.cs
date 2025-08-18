using System;
using System.Windows.Input;

namespace Sundew.Xaml.Optimizations.TestData;

public class Command<TParameter> : ICommand
{
    private readonly Action<TParameter> action;
    private readonly Func<TParameter, bool> canExecute;

    public Command(Action<TParameter> action, Func<TParameter, bool> canExecute = null)
    {
        this.action = action;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute?.Invoke((TParameter)parameter) ?? true;
    }

    public void Execute(object parameter)
    {
        this.action((TParameter)parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}