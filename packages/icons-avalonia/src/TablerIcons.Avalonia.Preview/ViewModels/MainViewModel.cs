using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace TablerIcons.Avalonia.Preview.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        var random = new Random();
        string values = "0123456789ABCDEF";
        Func<char> r = () => values[random.Next(0, values.Length)];
        Func<float> s = () => (float)((random.NextDouble() + 0.1) * 2);
        foreach (var value in Enum.GetValues<Icons>())
        {
            var color = $"#{r()}{r()}{r()}{r()}{r()}{r()}";
            Icons.Add(new(value, value.ToString(), color, s()));
        }

        new DispatcherTimer(
            TimeSpan.FromSeconds(2),
            DispatcherPriority.Default,
            (_, e) =>
        {
            foreach(var icon in Icons)
            {
                icon.Color = $"#{r()}{r()}{r()}{r()}{r()}{r()}";
                icon.Stroke = s();
            }
        }).Start();
    }
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    private ObservableCollection<IconModel> _icons = new();
}

public partial class IconModel : ObservableObject
{

    [ObservableProperty]
    Icons _icon;
    [ObservableProperty]
    string _name;
    [ObservableProperty]
    string _color;
    [ObservableProperty]
    float _stroke;
    
    public IconModel(Icons icon, string name, string color, float stroke)
    {
        Icon = icon;
        Name = name;
        Color = color;
        Stroke = stroke;
    }
}