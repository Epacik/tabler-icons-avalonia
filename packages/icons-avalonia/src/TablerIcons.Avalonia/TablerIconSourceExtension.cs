using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using Svg;
using System;
using System.Drawing;

using IImage = Avalonia.Media.IImage;
using SvgImage = Avalonia.Svg.Skia.SvgImage;
using Image = Avalonia.Controls.Image;
using Svg.DataTypes;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TablerIcons.Avalonia
{
    public sealed class TablerIconSourceExtension : MarkupExtension
    {
        public TablerIconSourceExtension() { }
        public TablerIconSourceExtension(Icons icon)
        {
            Icon = icon;
        }
        public float? Size { get; set; }
        public float StrokeWidth { get; set; } = 2;
        public string Color { get; set; } = "Black";
        public Icons Icon { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            var (document, source) = Utils.GetSvgSource(Icon, Size, StrokeWidth, Color);

            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (target.TargetProperty is AvaloniaProperty property)
            {
                if (property.PropertyType == typeof(IImage))
                {
                    return new SvgImage { Source = source };
                }
                var img = new Image { Source = new SvgImage { Source = source } };
                if (Size is float s)
                {
                    img.Height = s;
                    img.Width = s;
                }
                return img;
            }
            return new SvgImage { Source = source };
        }
    }
}