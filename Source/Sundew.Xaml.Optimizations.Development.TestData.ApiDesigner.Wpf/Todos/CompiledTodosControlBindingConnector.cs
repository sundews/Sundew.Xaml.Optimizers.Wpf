using System.Windows.Data;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

internal class CompiledTodosControlBindingConnector : BindingConnector<CompiledTodosControl>
{
    protected override void OnConnect()
    {
        var rootDataContext = this.GetDataContext(view => (ITodosViewModel)view.DataContext);
        var todosDataContext = rootDataContext.BindTargetDataContextOneWay(-1, this.Root.Grid1, dataContext => dataContext.Todos, "Todos");
        rootDataContext.BindInvariantOneWay(
            -1,
            this.Root.Button5,
            rootDataContext.CreateSourceProperty("AddCommand"),
            s => s.AddCommand,
            System.Windows.Controls.Button.CommandProperty,
            t => t.Command,
            BindingMode.OneWay);
        rootDataContext.BindInvariantOneWay(
            -1,
            this.Root.Button6,
            rootDataContext.CreateSourceProperty("RemoveCommand"),
            s => s.RemoveCommand,
            System.Windows.Controls.Button.CommandProperty,
            t => t.Command,
            BindingMode.OneWay);
        todosDataContext.BindSourceDataContextOneWay(
            -1,
            this.Root.TodosListBox,
            System.Windows.Controls.ListBox.ItemsSourceProperty,
            t => t.ItemsSource);

        var todosListBoxSource = this.GetElementContext(r => (System.Windows.Controls.ListBox)r.FindName("TodosListBox"));
        todosListBoxSource.BindInvariantOneWay(
            -1,
            this.Root.Button6,
            todosListBoxSource.CreateSourceProperty(
                (s, u) =>
                {
                    var h = new System.Windows.Controls.SelectionChangedEventHandler((s, e) => u());
                    s.SelectionChanged += h;
                    return h;
                },
                (s, h) => s.SelectionChanged -= h),
            s => s.SelectedItems,
            System.Windows.Controls.Button.CommandParameterProperty,
            t => t.CommandParameter,
            BindingMode.OneWay);

        var todoDescriptionTextBoxSource = this.GetElementContext(r => (System.Windows.Controls.TextBox)r.FindName("TodoDescriptionTextBox"));
        todoDescriptionTextBoxSource.BindInvariantOneWay(
            -1,
            this.Root.Button5,
            todoDescriptionTextBoxSource.CreateSourceProperty(System.Windows.Controls.TextBox.TextProperty),
            s => s.Text,
            System.Windows.Controls.Button.CommandParameterProperty,
            t => t.CommandParameter,
            BindingMode.OneWay);
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
        return new CompiledTodosControlBindingConnector();
    }
}