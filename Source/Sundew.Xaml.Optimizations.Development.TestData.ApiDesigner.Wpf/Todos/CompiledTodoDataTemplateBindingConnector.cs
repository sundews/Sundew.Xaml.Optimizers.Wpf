using System.Windows.Data;

namespace Sundew.Xaml.Optimizations.TestData.Todos;

public class CompiledTodoDataTemplateBindingConnector : BindingConnector<System.Windows.Controls.Grid>
{
    protected override void OnConnect()
    {
        var dataContext = this.GetDataContext(view => (ITodo)view.DataContext);
        dataContext.BindInvariant(
            -1,
            (System.Windows.Controls.TextBlock)this.Root.FindName("TextBlock2"),
            dataContext.CreateSourceProperty(nameof(ITodo.Description)),
            s => s.Description,
            System.Windows.Controls.TextBlock.TextProperty,
            t => t.Text,
            (s, v) => s.Description = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        dataContext.BindInvariant(
            -1,
            (System.Windows.Controls.TextBox)this.Root.FindName("TextBox3"),
            dataContext.CreateSourceProperty(nameof(ITodo.Description)),
            s => s.Description,
            System.Windows.Controls.TextBox.TextProperty,
            t => t.Text,
            (s, v) => s.Description = v,
            UpdateSourceTrigger.PropertyChanged,
            BindingMode.TwoWay);
        dataContext.Bind(
            -1,
            (System.Windows.Controls.CheckBox)this.Root.FindName("CheckBox4"),
            dataContext.CreateSourceProperty(nameof(ITodo.IsDone)),
            s => s.IsDone,
            System.Windows.Controls.CheckBox.IsCheckedProperty,
            t => t.IsChecked,
            (s, v) => s.IsDone = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        dataContext.BindInvariant(
            -1,
            (System.Windows.Controls.TextBlock)this.Root.FindName("TextBlock5"),
            dataContext.CreateSourceProperty(nameof(ITodo.Notes)),
            s => s.Notes,
            System.Windows.Controls.TextBlock.TextProperty,
            t => t.Text,
            (s, v) => s.Notes = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        dataContext.BindInvariant(
            -1,
            (System.Windows.Controls.TextBox)this.Root.FindName("TextBox6"),
            dataContext.CreateSourceProperty(nameof(ITodo.Notes)),
            s => s.Notes,
            System.Windows.Controls.TextBox.TextProperty,
            t => t.Text,
            (s, v) => s.Notes = v,
            UpdateSourceTrigger.LostFocus,
            BindingMode.TwoWay);
        dataContext.Bind(
            1,
            (System.Windows.Controls.TextBlock)this.Root.FindName("TextBlock7"),
            dataContext.CreateSourceProperty(nameof(ITodo.Created)),
            s => s.Created,
            System.Windows.Controls.TextBlock.TextProperty,
            t => t.Text,
            (s, v) => s.Created = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        dataContext.BindOneWay(
            1,
            (System.Windows.Controls.TextBlock)this.Root.FindName("TextBlock8"),
            dataContext.CreateSourceProperty(nameof(ITodo.Completed)),
            s => s.Completed,
            System.Windows.Controls.TextBlock.TextProperty,
            t => t.Text,
            BindingMode.OneWay);
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
        return new CompiledTodoDataTemplateBindingConnector();
    }
}