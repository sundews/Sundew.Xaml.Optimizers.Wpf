using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

public interface ITodosViewModel
{
    ObservableCollection<ITodo> Todos { get; }

    ICommand AddCommand { get; }
        
    ICommand RemoveCommand { get; }
}