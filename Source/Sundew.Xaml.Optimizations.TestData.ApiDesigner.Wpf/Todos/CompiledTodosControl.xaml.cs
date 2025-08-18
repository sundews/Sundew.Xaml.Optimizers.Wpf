namespace Sundew.Xaml.Optimizations.TestData.Todos;

/// <summary>
/// Interaction logic for DemoWindow.xaml
/// </summary>
public partial class CompiledTodosControl
{
    public CompiledTodosControl()
    {
        InitializeComponent();
    }

    private void Grid_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        var container0 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(0);
        var container1 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(1);
        var container2 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(2);
        var container3 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(3);
        var container4 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(4);
        var container5 = this.TodosListBox.ItemContainerGenerator.ContainerFromIndex(5);
        var containerItem = this.TodosListBox.ItemContainerGenerator.ContainerFromItem(e.NewValue);
        var indexContainer = this.TodosListBox.ItemContainerGenerator.IndexFromContainer(containerItem);

    }
}