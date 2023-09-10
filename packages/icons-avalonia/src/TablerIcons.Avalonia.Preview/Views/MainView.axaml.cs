using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace TablerIcons.Avalonia.Preview.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        //var random = new Random();
        //string values = "0123456789ABCDEF";
        //Func<char> r = () => values[random.Next(0, values.Length)];
        //foreach (var value in Enum.GetValues<Icons>())
        //{
        //    var color = $"#{r()}{r()}{r()}{r()}{r()}{r()}";
        //    _icons.Children.Add(new StackPanel()
        //    {
        //        Margin = new Thickness(5),
        //        Children =
        //        {
        //            new Image()
        //            {
        //                Width = 40,
        //                Height = 40,
        //                Source = new TablerIconSource
        //                {
        //                    Icon = value,
        //                    Color = color,
        //                }
        //            },
        //            new TextBlock()
        //            {
        //                Text = value.ToString(),
        //                TextAlignment = TextAlignment.Center
        //            }
        //        },
        //    });
        //}
    }

}
