using System.Windows;

namespace Sundew.Xaml.Optimizations.TestData;

/// <summary>
/// Interaction logic for TextWindow.xaml
/// </summary>
public partial class TextWindow : Window
{
    public TextWindow()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}