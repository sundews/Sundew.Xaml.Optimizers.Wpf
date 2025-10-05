using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

public class AnimationViewModel : INotifyPropertyChanged
{
    private Task task;
    private CancellationTokenSource cancellationTokenSource;
    private int frame;
    private int benchmarkRuns = 0;
    private TimeSpan? benchmarkTime;

    public AnimationViewModel()
    {
        this.Ellipse = new ElementViewModel();
        this.Rectangle = new ElementViewModel();
        this.Triangle = new ElementViewModel();
        this.StartCommand = new Command<object>(_ =>
        {
            this.benchmarkRuns = 0;
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = new CancellationTokenSource();

            this.Start(this.cancellationTokenSource);
        }, _ => this.cancellationTokenSource == null);
        this.StopCommand = new Command<object>(_ =>
        {
            this.cancellationTokenSource.Cancel();
            this.Wait();
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;
        }, _ => this.cancellationTokenSource != null);

        this.BenchmarkCommand = new Command<object>(_ =>
        {
            this.benchmarkRuns = 1_000_000;
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = new CancellationTokenSource();
            var stopwatch = Stopwatch.StartNew();
            this.Start(this.cancellationTokenSource).ContinueWith(_ => this.BenchmarkTime = stopwatch.Elapsed);
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand StartCommand { get; }

    public ICommand StopCommand { get; }

    public ICommand BenchmarkCommand { get; }

    public ElementViewModel Triangle { get; }

    public ElementViewModel Rectangle { get; }

    public ElementViewModel Ellipse { get; }

    public double Width { get; set; }

    public double Height { get; set; }

    public int Frame
    {
        get => this.frame;
        set
        {
            this.frame = value;
            this.OnPropertyChanged();
        }
    }

    public int Delay { get; set; }

    public bool IsRunning => this.task != null;

    public TimeSpan? BenchmarkTime
    {
        get => this.benchmarkTime;
        private set
        {
            this.benchmarkTime = value;
            this.OnPropertyChanged();
        }
    }

    public Task Await()
    {
        return this.task ?? Task.CompletedTask;
    }

    private Task Start(CancellationTokenSource cancellationTokenSource)
    {
        this.cancellationTokenSource = cancellationTokenSource;
        this.task = Task.Run(Animate);
        this.OnPropertyChanged(nameof(IsRunning));
        return this.task;
    }

    private void Wait()
    {
        this.task?.Wait();
        this.task = null;
        this.OnPropertyChanged(nameof(IsRunning));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Animate()
    {
        this.Frame = 0;
        while (!this.cancellationTokenSource.IsCancellationRequested)
        {
            this.Ellipse.Animate(this.Width, this.Height);
            this.Rectangle.Animate(this.Width, this.Height);
            this.Triangle.Animate(this.Width, this.Height);

            unchecked
            {
                this.Frame++;
            }

            if (this.benchmarkRuns <= 0)
            {
                Thread.Sleep(this.Delay);
            }

            if (this.Frame == this.benchmarkRuns)
            {
                break;
            }
        }
    }
}