using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

public class TodosViewModel : ITodosViewModel
{
    public TodosViewModel()
    {
        this.AddCommand = new Command<string>(description => this.Todos.Add(new Todo { Description = description, Created = DateTime.Now }), description => !string.IsNullOrEmpty(description));
        this.RemoveCommand = new Command<object>(parameter =>
            {
                switch (parameter)
                {
                    case null:
                        return;
                    case IEnumerable enumerable:
                    {
                        foreach (var todo in enumerable.OfType<ITodo>().ToList())
                        {
                            this.Todos.Remove(todo);
                        }

                        break;
                    }
                    case ITodo todoItem:
                        this.Todos.Remove(todoItem);
                        break;
                }
            },
            parameter =>
            {
                return parameter switch
                {
                    null => false,
                    IEnumerable enumerable => enumerable.OfType<ITodo>().Any(),
                    _ => true
                };
            });
        this.AddManyCommand = new Command<object>(o =>
            {
                for (int i = 0; i < 20; i++)
                {
                    this.Todos.Add(new Todo
                    {
                        Created = DateTime.Now,
                        Description = i.ToString(),
                        Notes = i.ToString()
                    });
                }
            },
            o => this.Todos != null);
        this.ClearCommand = new Command<object>(o => this.Todos.Clear(), o => this.Todos != null);
    }

    public ObservableCollection<ITodo> Todos { get; } = new ObservableCollection<ITodo>();

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand AddManyCommand { get; }

    public ICommand ClearCommand { get; }
}