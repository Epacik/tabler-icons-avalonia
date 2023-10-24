using Avalonia.Markup.Xaml;
using System;
using Avalonia.Media;

namespace TablerIcons
{
    public sealed class TablerIconExtension : MarkupExtension
    {
        public TablerIconExtension() { }
        public TablerIconExtension(Icons icon)
        {
            Icon = icon;
        }
        public float StrokeWidth { get; set; } = 2;
        public IBrush Brush { get; set; } = Brushes.Black;
        public Icons Icon { get; set; }
        public double Width { get; set; } = 30;
        public double Height { get; set; } = 30;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TablerIcon()
            {
                Icon = Icon,
                StrokeWidth = StrokeWidth,
                Brush = Brush,
                Width = Width,
                Height = Height,
            };
        }
    }
}