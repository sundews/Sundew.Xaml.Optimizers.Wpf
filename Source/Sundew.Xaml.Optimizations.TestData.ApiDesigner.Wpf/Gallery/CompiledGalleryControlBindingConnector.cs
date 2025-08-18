using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sundew.Xaml.Optimizations.TestData.Gallery;

public class CompiledGalleryControlBindingConnector : BindingConnector<UserControl>
{
    protected override void OnConnect()
    {
        var dataContext = this.GetDataContext(view => (GalleryViewModel)view.DataContext);
        dataContext.BindInvariantOneWay(
            -1,
            (System.Windows.Controls.ItemsControl)this.Root.FindName("ItemsControl1"),
            dataContext.CreateSourceProperty(nameof(GalleryViewModel.Photos)),
            s => s.Photos,
            System.Windows.Controls.ItemsControl.ItemsSourceProperty,
            t => t.ItemsSource,
            BindingMode.TwoWay);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new CompiledGalleryControlBindingConnector();
    }
}