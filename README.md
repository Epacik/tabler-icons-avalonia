### Tabler Icons Avalonia Components

Tabler Icons Component library for Avalonia UI.

<p align="center">
  <a href="https://tabler-icons.io/"><strong>Browse at tabler-icons.io →</strong></a>
</p>

## Installation

You can install the package from nuget.

```
dotnet add package TablerIcons.Avalonia
```

## Usage

You can either use a TablerIcon control, or a TablerIcon Markup Extension.
TablerIcon control supports binding to it's properties.
Names of the icons are the same as React names displayed on the official  [Tabler Icons](https://tabler.io/icons) website.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             ...
             xmlns:ti="using:TablerIcons">
  <StackPanel>
    <ti:TablerIcon Icon="{Binding IconHeart}" Width="{Binding IconSize}" Height="{Binding IconSize}" StrokeWidth="{Binding StrokeWidth}"/>
    <ti:TablerIcon Icon="IconHeart" Width="24" Height="24" StrokeWidth="1"/>
    <ti:TablerIcon Icon="IconHeart" Brush="Crimson" Width="24" Height="24" StrokeWidth="1"/>
    <ContentControl Content="{ti:TablerIcon IconHeart, Width=24, Height=24, StrokeWidth=1}"/>
    <ContentControl Content="{ti:TablerIcon IconHeart, Brush=Crimson, Width=24, Height=24, StrokeWidth=1}"/>
  </StackPanel>
</UserControl>
```

Properties were renamed to align more to what naming convention found in Avalonia UI controls.

- size - Width and Height

- color - Brush (supports SolidColorBrush and default Gradient brushes from Avalonia)

- stroke - StrokeWidth

## License

Tabler Icons is licensed under the [MIT License](https://github.com/tabler/tabler-icons/blob/master/LICENSE).

Tabler Icons Avalonia Components is licensed under the [MIT License](https://github.com/Epacik/tabler-icons-avalonia/blob/master/LICENSE) as well
