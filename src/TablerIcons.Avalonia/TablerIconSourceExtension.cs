using Avalonia.Markup.Xaml;
using System;
using Avalonia.Media;

namespace TablerIcons
{
    internal sealed class TablerIconSourceExtension : MarkupExtension
    {
        public TablerIconSourceExtension() { }
        public TablerIconSourceExtension(Icons icon)
        {
            Icon = icon;
        }
        public float StrokeWidth { get; set; } = 2;
        public IBrush Brush { get; set; } = Brushes.Black;
        public Icons Icon { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TablerIconSource()
            {
                Icon = Icon,
                StrokeWidth = StrokeWidth,
                Brush = Brush,
            };
        }
    }
}