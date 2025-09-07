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
    [global::System.CodeDom.Compiler.GeneratedCode("Sundew.Xaml.Optimizations.Bindings", "2.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ITodoBindingContainer : global::Sundew.Xaml.Optimizations.BindingConnector<global::System.Windows.Controls.Grid>
    {
        protected override void OnConnect()
        {
            var iTodoDataContext = this.GetDataContext(view => (global::Sundew.Xaml.Optimizations.TestData.Todos.ITodo)view.DataContext);
            iTodoDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.TextBlock)r.FindName("TextBlock1"),
                "Description",
                s => s.Description,
                global::System.Windows.Controls.TextBlock.TextProperty,
                t => t.Text,
                (s, v) => s.Description = v);
            iTodoDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.TextBox)r.FindName("TextBox2"),
                "Description",
                s => s.Description,
                global::System.Windows.Controls.TextBox.TextProperty,
                t => t.Text,
                (s, v) => s.Description = v,
                global::System.Windows.Data.UpdateSourceTrigger.PropertyChanged);
            iTodoDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.CheckBox)r.FindName("CheckBox3"),
                "IsDone",
                s => s.IsDone,
                global::System.Windows.Controls.CheckBox.IsCheckedProperty,
                t => t.IsChecked,
                (s, v) => s.IsDone = v);
            iTodoDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.TextBlock)r.FindName("TextBlock4"),
                "Notes",
                s => s.Notes,
                global::System.Windows.Controls.TextBlock.TextProperty,
                t => t.Text,
                (s, v) => s.Notes = v);
            iTodoDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.TextBox)r.FindName("TextBox5"),
                "Notes",
                s => s.Notes,
                global::System.Windows.Controls.TextBox.TextProperty,
                t => t.Text,
                (s, v) => s.Notes = v,
                global::System.Windows.Data.UpdateSourceTrigger.LostFocus);
            iTodoDataContext.BindProperty(
                1,
                r => (global::System.Windows.Controls.TextBlock)r.FindName("TextBlock6"),
                "Created",
                s => s.Created,
                global::System.Windows.Controls.TextBlock.TextProperty,
                t => t.Text,
                (s, v) => s.Created = v);
            iTodoDataContext.BindProperty(
                1,
                r => (global::System.Windows.Controls.TextBlock)r.FindName("TextBlock7"),
                "Completed",
                s => s.Completed,
                global::System.Windows.Controls.TextBlock.TextProperty,
                t => t.Text);

        }
    }
}