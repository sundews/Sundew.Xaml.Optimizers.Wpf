using System;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

public interface ITodo
{
    string Description { get; set; }

    string Notes { get; set; }

    bool IsDone { get; set; }
        
    DateTime Created { get; set; }

    DateTime? Completed { get; }
}