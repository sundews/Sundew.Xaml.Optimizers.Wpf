using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer;

public class CompiledBouncerControlBindingConnector : BindingConnector<CompiledBouncerControl>
{
    protected override void OnConnect()
    {
        var rootDataContext = this.GetDataContext(view => (AnimationViewModel)view.DataContext);
        rootDataContext.BindInvariantOneWay(
            -1,
            this.Root,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.IsRunning)),
            s => s.IsRunning,
            CompiledBouncerControl.IsRunningProperty,
            t => t.IsRunning,
            BindingMode.OneWay);
        rootDataContext.BindInvariant(
            -1,
            this.Root.Canvas1,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Width)),
            s => s.Width,
            ActualSize.ActualWidthProperty,
            ActualSize.GetActualWidth,
            (s, v) => s.Width = v,
            UpdateSourceTrigger.Default,
            BindingMode.OneWayToSource);
        rootDataContext.BindInvariant(
            -1,
            this.Root.Canvas1,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Height)),
            s => s.Height,
            ActualSize.ActualHeightProperty,
            ActualSize.GetActualHeight,
            (s, v) => s.Height = v,
            UpdateSourceTrigger.Default,
            BindingMode.OneWayToSource);
        var ellipse = rootDataContext.BindPart(
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Ellipse)),
            s => s.Ellipse,
            BindingMode.TwoWay);
        ellipse.BindInvariant(
            -1,
            this.Root.Ellipse1,
            ellipse.CreateSourceProperty(nameof(ElementViewModel.X)),
            s => s.X,
            Canvas.LeftProperty,
            Canvas.GetLeft,
            (s, v) => s.X = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        ellipse.BindInvariant(
            -1,
            this.Root.Ellipse1,
            ellipse.CreateSourceProperty(nameof(ElementViewModel.Y)),
            s => s.Y,
            Canvas.TopProperty,
            Canvas.GetTop,
            (s, v) => s.Y = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        ellipse.BindInvariant(
            -1,
            this.Root.Ellipse1,
            ellipse.CreateSourceProperty(nameof(ElementViewModel.Width)),
            s => s.Width,
            Ellipse.WidthProperty,
            t => t.Width,
            (s, v) => s.Width = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        ellipse.BindInvariant(
            -1,
            this.Root.Ellipse1,
            ellipse.CreateSourceProperty(nameof(ElementViewModel.Height)),
            s => s.Height,
            Ellipse.HeightProperty,
            t => t.Height,
            (s, v) => s.Height = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);

        var rectangle = rootDataContext.BindPart(
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Rectangle)),
            s => s.Rectangle,
            BindingMode.TwoWay);
        rectangle.BindInvariant(
            -1,
            this.Root.Rectangle1,
            rectangle.CreateSourceProperty(nameof(ElementViewModel.X)),
            s => s.X,
            Canvas.LeftProperty,
            Canvas.GetLeft,
            (s, v) => s.X = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        rectangle.BindInvariant(
            -1,
            this.Root.Rectangle1,
            rectangle.CreateSourceProperty(nameof(ElementViewModel.Y)),
            s => s.Y,
            Canvas.TopProperty,
            Canvas.GetTop,
            (s, v) => s.Y = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        rectangle.BindInvariant(
            -1,
            this.Root.Rectangle1,
            rectangle.CreateSourceProperty(nameof(ElementViewModel.Width)),
            s => s.Width,
            Rectangle.WidthProperty,
            t => t.Width,
            (s, v) => s.Width = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        rectangle.BindInvariant(
            -1,
            this.Root.Rectangle1,
            rectangle.CreateSourceProperty(nameof(ElementViewModel.Height)),
            s => s.Height,
            Rectangle.HeightProperty,
            t => t.Height,
            (s, v) => s.Height = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);

        var triangle = rootDataContext.BindPart(
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Triangle)),
            s => s.Triangle,
            BindingMode.TwoWay);
        triangle.BindInvariant(
            -1,
            this.Root.Triangle1,
            triangle.CreateSourceProperty(nameof(ElementViewModel.X)),
            s => s.X,
            Canvas.LeftProperty,
            Canvas.GetLeft,
            (s, v) => s.X = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        triangle.BindInvariant(
            -1,
            this.Root.Triangle1,
            triangle.CreateSourceProperty(nameof(ElementViewModel.Y)),
            s => s.Y,
            Canvas.TopProperty,
            Canvas.GetTop,
            (s, v) => s.Y = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        triangle.BindInvariant(
            -1,
            this.Root.Triangle1,
            triangle.CreateSourceProperty(nameof(ElementViewModel.Width)),
            s => s.Width,
            Polygon.WidthProperty,
            t=> t.Width,
            (s, v) => s.Width = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);
        triangle.BindInvariant(
            -1,
            this.Root.Triangle1,
            triangle.CreateSourceProperty(nameof(ElementViewModel.Height)),
            s => s.Height,
            Polygon.HeightProperty,
            t=> t.Height,
            (s, v) => s.Height = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);

        rootDataContext.Bind(
            -1,
            this.Root.Run1,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.Frame)),
            s => s.Frame,
            Run.TextProperty,
            t => t.Text,
            (s, v) => s.Frame = v,
            UpdateSourceTrigger.Default,
            BindingMode.TwoWay);

        rootDataContext.BindInvariantOneWay(
            -1,
            this.Root.Button1,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.StartCommand)),
            s => s.StartCommand,
            Button.CommandProperty,
            t => t.Command,
            BindingMode.OneWay);

        rootDataContext.BindInvariantOneWay(
            -1,
            this.Root.Button2,
            rootDataContext.CreateSourceProperty(nameof(AnimationViewModel.StopCommand)),
            s => s.StopCommand,
            Button.CommandProperty,
            t => t.Command,
            BindingMode.OneWay);
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
        return new CompiledBouncerControlBindingConnector();
    }
}