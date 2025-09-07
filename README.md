# Sundew.Xaml.Optimizers
## Usage
Refer to the sample for how to use optimizations: https://github.com/hugener/Sundew.Xaml.Optimizer.Sample
## Supported optimizers:
### ResourceDictionaryOptimizer
The ResourceDictionaryCachingOptimizer enables caching for merged ResourceDictionaries and has the following advantages:
1. Merged ResourcesDictionaries are only loaded once.
2. Tooling and designers will not break, as they see the original WPF ResourceDictionary.
3. Less overhead maintaining DesignTimerResources.

Supported settings:
- DefaultReplacementType - Only used with Replace action, formatted as: "prefix|XamlNamespace|TypeName"
- ReplaceUncategorized - If true, all uncategorized ResourceDictionaries will be replaced with the built-in caching ResourceDictionary.
- OptimizationMappings - A list of optimizations to apply, default is empty where all ResourceDictionaries are replaced with the built-in caching ResourceDictionary.
    - Category - The category e.g. an emoji charater like 🎨 (likely to be used with Remove) to indicate a "theme" dictionary or ♻️ to shared (recycled) resources.
    - Action - Remove or Replace (With ReplacementType). 
    - ReplacementType - Only used with Replace action, formatted as: "prefix|XamlNamespace|TypeName"


### FreezeResourceOptimizer
The optimizer changes all WPF Freezable classes such as brushes to frozen unless po:Freeze="False" is set or the key include the UnfreezeMarker.  
This improves performance by avoiding that WPF has to clone brushes during rendering.  
Note that brushes that get modified (e.g. animated) at runtime must set po:Freeze="False" or use the UnfreezeMarker, otherwise exceptions will be thrown at runtime.  
For more information about presentation options see: https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/presentationoptions-freeze-attribute

Supported settings:
- IncludeFrameworkTypes (true/false), default true => Includes all WPF freezables.
- UnfreezeMarker - A string e.g. an emoji character like 💫 that can be used to mark resources as unfrozen. Any resource containing this marker in the key will not be frozen.
- IncludedTypes (List of xaml type names), default null => Additional freezables to be included.
- ExcludedTypes (List of xaml type names), default null => Freezables to be excluded. 
Types in IncludedTypes and ExcludedTypes that does not include a XML namespace will use the WPF presentation namespace.

### StaticToDynamicResourceOptimizer
This optimizer replaces StaticResource with DynamicResource, which is useful for themes, since the designer/editor can check that resource names actually exist.  
At build time these get rewritten to DynamicResource to support refreshing at runtime.

Supported settings:
- DynamicMarker - A string e.g. an emoji character like 🔄 that can be used to mark resources as dynamic. Any StaticResource containing this marker in the key will be replaced with DynamicResource, unless the resource is declared in the same file. 

### ThemeOptimizer

This optimizer enables theme support by merging ResourceDictionaries based on a theme and theme mode.
It supports ResourceDictionary reuse for theme modes for resource dictionaries places in the Modes folder which are not ThemeModes. These will be copied for each theme mode during optimization.

Themes can be exchanged at runtime by the ThemeManager found in [Sundew.Xaml.Theming.Wpf](https://www.nuget.org/packages/Sundew.Xaml.Theming.Wpf).

Supported settings:
- ThemesPath - The path relative to the project file where theme ResourceDictionaries are located. Default is "Themes".
- ThemeModesPath - The path relative to the ThemesPath where theme mode ResourceDictionaries are located. Default is "Modes".