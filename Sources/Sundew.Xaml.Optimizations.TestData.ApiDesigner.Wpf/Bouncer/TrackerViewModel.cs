using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

public class TrackerViewModel : INotifyPropertyChanged
{
    private int delay = 1;

    public TrackerViewModel()
    {
        this.CompiledAnimationViewModel = new AnimationViewModel();
        this.ReflectionAnimationViewModel = new AnimationViewModel();
        this.StartAnimationCommand = new Command<object>(_ =>
        {
            this.ReflectionAnimationViewModel.StartCommand.Execute(null);
            this.CompiledAnimationViewModel.StartCommand.Execute(null);
        }, _ => !this.ReflectionAnimationViewModel.IsRunning && !this.CompiledAnimationViewModel.IsRunning);
        this.StopAnimationCommand = new Command<object>(_ =>
        {
            this.CompiledAnimationViewModel.StopCommand.Execute(null);
            this.ReflectionAnimationViewModel.StopCommand.Execute(null);
        }, _ => this.ReflectionAnimationViewModel.IsRunning && this.CompiledAnimationViewModel.IsRunning);
        this.ResetCommand = new Command<object>(_ =>
        {
            this.CompiledAnimationViewModel.Ellipse.X = 0;
            this.CompiledAnimationViewModel.Ellipse.Y = 0;
            this.CompiledAnimationViewModel.Rectangle.X = 0;
            this.CompiledAnimationViewModel.Rectangle.Y = 80;
            this.CompiledAnimationViewModel.Triangle.X = 80;
            this.CompiledAnimationViewModel.Triangle.Y = 0;
            this.ReflectionAnimationViewModel.Ellipse.X = 0;
            this.ReflectionAnimationViewModel.Ellipse.Y = 0;
            this.ReflectionAnimationViewModel.Rectangle.X = 0;
            this.ReflectionAnimationViewModel.Rectangle.Y = 80;
            this.ReflectionAnimationViewModel.Triangle.X = 80;
            this.ReflectionAnimationViewModel.Triangle.Y = 0;
        });
        this.ResetCommand.Execute(null);
        this.BenchmarkCommand = new Command<object>(async _ =>
        {
            this.CompiledAnimationViewModel.BenchmarkCommand.Execute(null);
            await this.CompiledAnimationViewModel.Await().ConfigureAwait(false);
            this.CompiledAnimationViewModel.StopCommand.Execute(null);

            await Task.Delay(1000).ConfigureAwait(false);

            this.ReflectionAnimationViewModel.BenchmarkCommand.Execute(null);
            await this.ReflectionAnimationViewModel.Await().ConfigureAwait(false);
            this.ReflectionAnimationViewModel.StopCommand.Execute(null);

            CommandManager.InvalidateRequerySuggested();
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public AnimationViewModel CompiledAnimationViewModel { get; }

    public AnimationViewModel ReflectionAnimationViewModel { get; }

    public ICommand StartAnimationCommand { get; }

    public ICommand StopAnimationCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand BenchmarkCommand { get; }

    public int Delay
    {
        get => this.delay;
        set
        {
            this.delay = value;
            this.CompiledAnimationViewModel.Delay = value;
            this.ReflectionAnimationViewModel.Delay = value;
            OnPropertyChanged();
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}