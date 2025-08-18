using System.Windows;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

public static class ActualSize
{
    public static readonly DependencyProperty TrackSizeProperty = DependencyProperty.RegisterAttached(
        "TrackSize", typeof(bool), typeof(ActualSize), new PropertyMetadata(false, OnTrackSizeChanged));

    public static readonly DependencyProperty ActualWidthProperty = DependencyProperty.RegisterAttached(
        "ActualWidth", typeof(double), typeof(ActualSize), new PropertyMetadata(default(double)));

    public static readonly DependencyProperty ActualHeightProperty = DependencyProperty.RegisterAttached(
        "ActualHeight", typeof(double), typeof(ActualSize), new PropertyMetadata(default(double)));

    public static bool GetTrackSize(DependencyObject dependencyObject)
    {
        return (bool) dependencyObject.GetValue(TrackSizeProperty);
    }

    public static void SetTrackSize(DependencyObject dependencyObject, bool trackSize)
    {
        dependencyObject.SetValue(TrackSizeProperty, trackSize);
    }

    public static double GetActualWidth(DependencyObject dependencyObject)
    {
        return (double) dependencyObject.GetValue(ActualWidthProperty);
    }

    public static void SetActualWidth(DependencyObject dependencyObject, double width)
    {
        dependencyObject.SetValue(ActualWidthProperty, width);
    }

    public static double GetActualHeight(DependencyObject dependencyObject)
    {
        return (double) dependencyObject.GetValue(ActualHeightProperty);
    }
    public static void SetActualHeight(DependencyObject dependencyObject, double height)
    {
        dependencyObject.SetValue(ActualHeightProperty, height);
    }

    private static void OnTrackSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement frameworkElement)
        {
            if (GetTrackSize(frameworkElement))
            {
                frameworkElement.SizeChanged -= FrameworkElement_SizeChanged;
                frameworkElement.SizeChanged += FrameworkElement_SizeChanged;
            }
            else
            {
                frameworkElement.SizeChanged -= FrameworkElement_SizeChanged;
            }
        }
    }

    private static void FrameworkElement_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is FrameworkElement frameworkElement)
        {
            frameworkElement.SetValue(ActualWidthProperty, e.NewSize.Width);
            frameworkElement.SetValue(ActualHeightProperty, e.NewSize.Height);
        }
    }
}