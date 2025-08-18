using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Sundew.Xaml.Optimizations.Bindings;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

/// <summary>
/// Interaction logic for ReflectionTodosControl.xaml
/// </summary>
public partial class ReflectionBouncerControl
{
    private const string TicksFormat = "FFFFFFF";

    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register("IsRunning", typeof(bool), typeof(ReflectionBouncerControl), new PropertyMetadata(OnIsRunningChanged));

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly List<DependencyPropertyListener> dependencyPropertyListeners = new List<DependencyPropertyListener>();
    private int updateCount = 0;

    public ReflectionBouncerControl()
    {
        InitializeComponent();
        this.Loaded += this.OnLoaded;
        this.Unloaded += this.OnUnloaded;
    }

    public bool IsRunning
    {
        get => (bool)this.GetValue(IsRunningProperty);
        set => this.SetValue(IsRunningProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (HwndSource.FromDependencyObject(this) is HwndSource hwndSource)
        {
            hwndSource.Disposed += this.OnUnloaded;
        }

        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionEllipse, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionEllipse, Canvas.LeftProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionRectangle, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionRectangle, Canvas.LeftProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionPolygon, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.ReflectionPolygon, Canvas.LeftProperty, this.OnPropertyChanged));
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        if (HwndSource.FromDependencyObject(this) is HwndSource hwndSource)
        {
            hwndSource.Disposed -= this.OnUnloaded;
        }

        foreach (var dependencyPropertyListener in this.dependencyPropertyListeners)
        {
            dependencyPropertyListener.Dispose();
        }
    }

    private void OnPropertyChanged(object sender, EventArgs eventArgs)
    {
        var ms = this.stopwatch.Elapsed;
        Run2.Text = ms.ToString(TicksFormat);
        updateCount++;
        Run3.Text = updateCount.ToString();
        stopwatch.Restart();
    }

    private static void OnIsRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ReflectionBouncerControl reflectionBouncerControl)
        {
            if (reflectionBouncerControl.IsRunning)
            {
                reflectionBouncerControl.updateCount = 0;
                reflectionBouncerControl.stopwatch.Start();
            }
            else
            {
                reflectionBouncerControl.stopwatch.Stop();
            }
        }
    }
}