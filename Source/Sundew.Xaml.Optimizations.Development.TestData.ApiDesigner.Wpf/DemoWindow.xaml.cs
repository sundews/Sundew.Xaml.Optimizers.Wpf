namespace Sundew.Xaml.Optimizations.TestData;

/// <summary>
/// Interaction logic for DemoWindow.xaml
/// </summary>
public partial class DemoWindow
{
    public DemoWindow()
    {
        InitializeComponent();
        this.DataContext = new DemoViewModel();
    }
}