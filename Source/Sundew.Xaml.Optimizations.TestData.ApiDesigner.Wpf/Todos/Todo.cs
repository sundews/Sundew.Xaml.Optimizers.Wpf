using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

public class Todo : ITodo, INotifyPropertyChanged
{
    private string description;
    private string notes;
    private bool isDone;
    private DateTime created;
    private DateTime? completed;

    public string Description
    {
        get => this.description;
        set
        {
            this.description = value;
            OnPropertyChanged();
        }
    }

    public string Notes
    {
        get => this.notes;
        set
        {
            this.notes = value;
            OnPropertyChanged();
        }
    }

    public bool IsDone
    {
        get => this.isDone;
        set
        {
            this.isDone = value;
            this.completed = this.isDone ? (DateTime?)DateTime.Now : null;
            OnPropertyChanged();
            OnPropertyChanged(nameof(this.Completed));
        }
    }

    public DateTime Created
    {
        get => this.created;
        set
        {
            this.created = value;
            OnPropertyChanged();
        }
    }

    public DateTime? Completed => this.completed;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}