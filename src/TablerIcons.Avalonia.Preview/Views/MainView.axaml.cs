using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace TablerIcons.Avalonia.Preview.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        var rand = new Random();

        Func<byte> nextByte = () => (byte)rand.Next(0, 255);

        var timer = new DispatcherTimer(
            TimeSpan.FromSeconds(2),
            DispatcherPriority.Normal,
            (s, e) =>
            {
                _icon.Brush = new SolidColorBrush(
                    new Color(
                        255,
                        nextByte(),
                        nextByte(),
                        nextByte()));

                _icon.StrokeWidth = (rand.NextSingle() * 4f) + 0.5f;
            });
    }
}
