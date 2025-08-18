using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sundew.Xaml.Optimizations.TestData.Gallery;

public class PhotoViewModelBindingConnector : BindingConnector<Border>
{
    protected override void OnConnect()
    {
        var dataContext = this.GetDataContext(view => (PhotoViewModel)view.DataContext);
        dataContext.BindOneWay(
            -1,
            (Image)this.Root.FindName("Image1"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.Source)),
            s => s.Source,
            Image.SourceProperty,
            t => t.Source,
            System.Windows.Data.BindingMode.OneWay);
        dataContext.BindInvariantOneWay(
            -1,
            (TextBlock)this.Root.FindName("TextBlock2"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.Title)),
            s => s.Title,
            TextBlock.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
        dataContext.BindInvariantOneWay(
            -1,
            (TextBlock)this.Root.FindName("TextBlock3"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.OriginalResolution)),
            s => s.OriginalResolution,
            TextBlock.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
        dataContext.BindInvariantOneWay(
            -1,
            (TextBlock)this.Root.FindName("TextBlock4"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.LargeUrl)),
            s => s.LargeUrl,
            TextBlock.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
        dataContext.BindOneWay(
            -1,
            (Run)this.Root.FindName("Run5"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.DateTaken)),
            s => s.DateTaken,
            Run.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
        dataContext.BindInvariantOneWay(
            -1,
            (Run)this.Root.FindName("Run6"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.License)),
            s => s.License,
            Run.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
        dataContext.BindInvariantOneWay(
            -1,
            (TextBlock)this.Root.FindName("TextBlock7"),
            dataContext.CreateSourceProperty(nameof(PhotoViewModel.Description)),
            s => s.Description,
            TextBlock.TextProperty,
            t => t.Text,
            System.Windows.Data.BindingMode.OneTime);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new PhotoViewModelBindingConnector();
    }
}