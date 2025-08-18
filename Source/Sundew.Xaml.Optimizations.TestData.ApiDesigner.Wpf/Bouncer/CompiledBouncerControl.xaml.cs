using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Sundew.Xaml.Optimizations.Bindings;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

/// <summary>
/// Interaction logic for DemoWindow.xaml
/// </summary>
public partial class CompiledBouncerControl
{
    private const string TicksFormat = "FFFFFFF";

    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register("IsRunning", typeof(bool), typeof(CompiledBouncerControl), new PropertyMetadata(OnIsRunningChanged));

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly List<DependencyPropertyListener> dependencyPropertyListeners = new List<DependencyPropertyListener>();
    private int updateCount = 0;

    public CompiledBouncerControl()
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

        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Ellipse1, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Ellipse1, Canvas.LeftProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Rectangle1, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Rectangle1, Canvas.LeftProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Triangle1, Canvas.TopProperty, this.OnPropertyChanged));
        this.dependencyPropertyListeners.Add(DependencyPropertyListener.Subscribe(this.Triangle1, Canvas.LeftProperty, this.OnPropertyChanged));
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
        if (d is CompiledBouncerControl compiledBouncerControl)
        {
            if (compiledBouncerControl.IsRunning)
            {
                compiledBouncerControl.updateCount = 0;
                compiledBouncerControl.stopwatch.Start();
            }
            else
            {
                compiledBouncerControl.stopwatch.Stop();
            }
        }
    }
}