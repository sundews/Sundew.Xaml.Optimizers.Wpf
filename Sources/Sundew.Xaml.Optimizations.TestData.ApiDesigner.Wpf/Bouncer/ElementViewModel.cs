using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

public class ElementViewModel : INotifyPropertyChanged
{
    private double x;
    private double y;
    private int xDirection = 1;
    private int yDirection = 1;
    public event PropertyChangedEventHandler PropertyChanged;

    public double Width { get; set; } = 20;
    public double Height { get; set; } = 20;

    public double X
    {
        get => x;
        set
        {
            x = value;
            OnPropertyChanged();
        }
    }

    public double Y
    {
        get => y;
        set
        {
            y = value;
            OnPropertyChanged();
        }
    }

    public void Animate(in double width, in double height)
    {
        var shouldContinue = false;
        var newX = this.X + xDirection;
        if (newX < 0)
        {
            xDirection = 1;
            shouldContinue = true;
        }

        if (newX + this.Width > width)
        {
            xDirection = -1;
            shouldContinue = true;
        }

        var newY = this.Y + yDirection;
        if (newY < 0)
        {
            yDirection = 1;
            shouldContinue = true;
        }

        if (newY + this.Height > height)
        {
            yDirection = -1;
            shouldContinue = true;
        }

        if (shouldContinue)
        {
            return;
        }

        this.X = newX;
        this.Y = newY;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}