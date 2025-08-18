using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sundew.Xaml.Optimizations.TestData.Bouncer
{
    [global::System.CodeDom.Compiler.GeneratedCode("Sundew.Xaml.Optimizations.Bindings", "2.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ReflectionBouncerControlBindingContainer : global::Sundew.Xaml.Optimizations.BindingConnector<global::Sundew.Xaml.Optimizations.TestData.Bouncer.ReflectionBouncerControl>
    {
        protected override void OnConnect()
        {
            var animationViewModelDataContext = this.GetDataContext(view => (global::Sundew.Xaml.Optimizations.TestData.Bouncer.AnimationViewModel)view.DataContext);
            animationViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Canvas)r.FindName("Canvas1"),
                "Width",
                s => s.Width,
                global::Sundew.Xaml.Optimizations.TestData.Bouncer.ActualSize.ActualWidthProperty,
                t => global::Sundew.Xaml.Optimizations.TestData.Bouncer.ActualSize.GetActualWidth(t));
            animationViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Canvas)r.FindName("Canvas1"),
                "Height",
                s => s.Height,
                global::Sundew.Xaml.Optimizations.TestData.Bouncer.ActualSize.ActualHeightProperty,
                t => global::Sundew.Xaml.Optimizations.TestData.Bouncer.ActualSize.GetActualHeight(t));
            var animationViewModelEllipseDataContext = animationViewModelDataContext.Bind("Ellipse", s => s.Ellipse);
            animationViewModelEllipseDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Ellipse)r.FindName("ReflectionEllipse"),
                "Width",
                s => s.Width,
                global::System.Windows.Shapes.Ellipse.WidthProperty,
                t => t.Width,
                (s, v) => s.Width = v);
            animationViewModelEllipseDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Ellipse)r.FindName("ReflectionEllipse"),
                "Height",
                s => s.Height,
                global::System.Windows.Shapes.Ellipse.HeightProperty,
                t => t.Height,
                (s, v) => s.Height = v);
            animationViewModelEllipseDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Ellipse)r.FindName("ReflectionEllipse"),
                "X",
                s => s.X,
                global::System.Windows.Controls.Canvas.LeftProperty,
                t => global::System.Windows.Controls.Canvas.GetLeft(t),
                (s, v) => s.X = v);
            animationViewModelEllipseDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Ellipse)r.FindName("ReflectionEllipse"),
                "Y",
                s => s.Y,
                global::System.Windows.Controls.Canvas.TopProperty,
                t => global::System.Windows.Controls.Canvas.GetTop(t),
                (s, v) => s.Y = v);
            var animationViewModelRectangleDataContext = animationViewModelDataContext.Bind("Rectangle", s => s.Rectangle);
            animationViewModelRectangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Rectangle)r.FindName("ReflectionRectangle"),
                "Width",
                s => s.Width,
                global::System.Windows.Shapes.Rectangle.WidthProperty,
                t => t.Width,
                (s, v) => s.Width = v);
            animationViewModelRectangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Rectangle)r.FindName("ReflectionRectangle"),
                "Height",
                s => s.Height,
                global::System.Windows.Shapes.Rectangle.HeightProperty,
                t => t.Height,
                (s, v) => s.Height = v);
            animationViewModelRectangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Rectangle)r.FindName("ReflectionRectangle"),
                "X",
                s => s.X,
                global::System.Windows.Controls.Canvas.LeftProperty,
                t => global::System.Windows.Controls.Canvas.GetLeft(t),
                (s, v) => s.X = v);
            animationViewModelRectangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Rectangle)r.FindName("ReflectionRectangle"),
                "Y",
                s => s.Y,
                global::System.Windows.Controls.Canvas.TopProperty,
                t => global::System.Windows.Controls.Canvas.GetTop(t),
                (s, v) => s.Y = v);
            var animationViewModelTriangleDataContext = animationViewModelDataContext.Bind("Triangle", s => s.Triangle);
            animationViewModelTriangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Polygon)r.FindName("ReflectionPolygon"),
                "Width",
                s => s.Width,
                global::System.Windows.Shapes.Polygon.WidthProperty,
                t => t.Width,
                (s, v) => s.Width = v);
            animationViewModelTriangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Polygon)r.FindName("ReflectionPolygon"),
                "Height",
                s => s.Height,
                global::System.Windows.Shapes.Polygon.HeightProperty,
                t => t.Height,
                (s, v) => s.Height = v);
            animationViewModelTriangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Polygon)r.FindName("ReflectionPolygon"),
                "X",
                s => s.X,
                global::System.Windows.Controls.Canvas.LeftProperty,
                t => global::System.Windows.Controls.Canvas.GetLeft(t),
                (s, v) => s.X = v);
            animationViewModelTriangleDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Shapes.Polygon)r.FindName("ReflectionPolygon"),
                "Y",
                s => s.Y,
                global::System.Windows.Controls.Canvas.TopProperty,
                t => global::System.Windows.Controls.Canvas.GetTop(t),
                (s, v) => s.Y = v);
            animationViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Documents.Run)r.FindName("Run1"),
                "Frame",
                s => s.Frame,
                global::System.Windows.Documents.Run.TextProperty,
                t => t.Text,
                (s, v) => s.Frame = v);
            animationViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button1"),
                "StartCommand",
                s => s.StartCommand,
                global::System.Windows.Controls.Button.CommandProperty,
                t => t.Command);
            animationViewModelDataContext.BindProperty(
                -1,
                r => (global::System.Windows.Controls.Button)r.FindName("Button2"),
                "StopCommand",
                s => s.StopCommand,
                global::System.Windows.Controls.Button.CommandProperty,
                t => t.Command);

        }
    }
}