using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sundew.Xaml.Optimizations.TestData.Todos
{
    [global::System.CodeDom.Compiler.GeneratedCode("Sundew.Xaml.Optimizations.Bindings", "3.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ReflectionTodosControlBindingContainer : global::Sundew.Xaml.Optimizations.BindingConnector<global::Sundew.Xaml.Optimizations.TestData.Todos.ReflectionTodosControl>
    {
        protected override void OnConnect()
        {
            var iTodosViewModelDataContext = this.GetDataContext(view => (global::Sundew.Xaml.Optimizations.TestData.Todos.ITodosViewModel)view.DataContext);
            var iTodosViewModelTodosDataContext = iTodosViewModelDataContext.BindDataContext((global::System.Windows.Controls.Grid)this.Root.FindName("Grid1"), dataContext => dataContext.Todos, "Todos");
            iTodosViewModelTodosDataContext.Bind(
                r => (global::System.Windows.Controls.ListBox)r.FindName("TodosListBox"),
                (s, t) => t.ItemsSource = s);
            iTodosViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button2"),
                "AddCommand",
                s => s.AddCommand,
                global::System.Windows.Controls.Button.CommandProperty,
                t => t.Command);
            iTodosViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button3"),
                "RemoveCommand",
                s => s.RemoveCommand,
                global::System.Windows.Controls.Button.CommandProperty,
                t => t.Command);
            var textBoxTodoDescriptionTextBoxSource = this.GetElementContext(r => (global::System.Windows.Controls.TextBox)r.FindName("TodoDescriptionTextBox"));
            textBoxTodoDescriptionTextBoxSource.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button2"),
                "Text",
                s => s.Text,
                global::System.Windows.Controls.Button.CommandParameterProperty,
                t => t.CommandParameter,
                (s, v) => s.Text = v);
            var listBoxTodosListBoxSource = this.GetElementContext(r => (global::System.Windows.Controls.ListBox)r.FindName("TodosListBox"));
            listBoxTodosListBoxSource.BindOneWayByEvent(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button3"),
                (s, u) =>
                    {
                        var h = new global::System.Windows.Controls.SelectionChangedEventHandler((s, e) => u());
                        s.SelectionChanged += h;
                        return h;
                    },
                (s, h) => s.SelectionChanged -= h,
                (s, t) => t.CommandParameter = s.SelectedItems);

        }
    }
}