using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Sundew.Xaml.Optimizations.TestData.Bouncer;
using Sundew.Xaml.Optimizations.TestData.Gallery;
using Sundew.Xaml.Optimizations.TestData.Todos;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace Sundew.Xaml.Optimizations.TestData;

public class DemoViewModel
{
    public TodosViewModel TodoDemo { get; } = new TodosViewModel();

    public TrackerViewModel TrackerDemo { get; } = new TrackerViewModel();

    public GalleryViewModel GalleryDemo { get; } = new GalleryViewModel();

    public ICommand ShowOneWayBoundDependencyPropertiesCommand { get; } = new Command<object>(_ =>
    {
        var textWindow = new TextWindow();
        textWindow.Show();

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(@"new Dictionary<string, IReadOnlyCollection<string>>
{");
        foreach (var assembly in new[]  {typeof(FrameworkElement).Assembly, typeof(ContentElement).Assembly, typeof(DependencyObject).Assembly})
        {
            foreach (var dependencyObjectType in assembly.ExportedTypes.Where(x => typeof(DependencyObject).IsAssignableFrom(x) && x.IsPublic))
            {
                var isTypeWritten = false;
                foreach (var property in dependencyObjectType.GetFields(BindingFlags.Static | BindingFlags.Public)
                             .Where(x => x.FieldType == typeof(DependencyProperty)))
                {
                    var dependencyProperty = (DependencyProperty) property.GetValue(null);
                    try
                    {
                        if (dependencyProperty.GetMetadata(dependencyObjectType) is
                            FrameworkPropertyMetadata
                            frameworkPropertyMetadata)
                        {
                            if (!frameworkPropertyMetadata.BindsTwoWayByDefault)
                            {
                                Write(stringBuilder, dependencyObjectType, ref isTypeWritten, dependencyProperty);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Write(stringBuilder, dependencyObjectType, ref isTypeWritten, dependencyProperty);
                    }

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                        new Action(delegate { }));
                }

                if (isTypeWritten)
                {
                    stringBuilder.AppendLine(@"        }
    },");
                }
            }
        }

        stringBuilder.AppendLine("}");
        textWindow.DataContext = stringBuilder.ToString();
    });

    public ICommand ShowReadOnlyDependencyPropertiesCommand { get; } = new Command<object>(_ =>
    {
        var textWindow = new TextWindow();
        textWindow.Show();

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(@"new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>
{");
        foreach (var assembly in new[] { typeof(FrameworkElement).Assembly, typeof(ContentElement).Assembly, typeof(DependencyObject).Assembly })
        {
            foreach (var dependencyObjectType in assembly.ExportedTypes.Where(x => typeof(DependencyObject).IsAssignableFrom(x) && x.IsPublic))
            {
                var isTypeWritten = false;
                foreach (var fieldInfo in dependencyObjectType.GetFields(BindingFlags.Static | BindingFlags.Public)
                             .Where(x => x.FieldType == typeof(DependencyProperty)))
                {
                    var dependencyProperty = (DependencyProperty)fieldInfo.GetValue(null);
                    if (dependencyProperty.ReadOnly)
                    {
                        if (!isTypeWritten)
                        {
                            isTypeWritten = true;
                            stringBuilder.AppendLine(@$"    {{
        ""{dependencyObjectType.Assembly.GetName().Name}|{dependencyObjectType.FullName}"",
        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
        {{");
                        }

                        stringBuilder.AppendLine(@$"            {{ ""{dependencyProperty.Name}"", null }},");
                    }

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                        new Action(delegate { }));
                }

                if (isTypeWritten)
                {
                    stringBuilder.AppendLine(@"        }
    },");
                }
            }
        }

        stringBuilder.AppendLine("}");
        textWindow.DataContext = stringBuilder.ToString();
    });

    private static void Write(StringBuilder stringBuilder, Type dependencyObjectType, ref bool isTypeWritten, DependencyProperty dependencyProperty)
    {
        if (!isTypeWritten)
        {
            isTypeWritten = true;
            stringBuilder.AppendLine(@$"    {{
        ""{dependencyObjectType.Assembly.GetName().Name}|{dependencyObjectType.FullName}"",
        new HashSet<string>
        {{");
        }

        stringBuilder.AppendLine(@$"            ""{dependencyProperty.Name}"",");
    }
}