using Avalonia.Platform;
using Avalonia.Svg.Skia;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TablerIcons.Avalonia
{
    internal class Utils
    {
        private static readonly MethodInfo _createPaintServerMethod;
        static Utils()
        {
            var attributes = TypeDescriptor.GetAttributes(typeof(SvgPaintServer));
            var typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];

            var types = typeof(SvgPaintServer).Assembly.DefinedTypes;
            var type = types.FirstOrDefault(x => x.AssemblyQualifiedName == typeConverterAttribute.ConverterTypeName);

            _createPaintServerMethod = type.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
        }
        internal static SvgPaintServer CreatePaintServer(string color, SvgDocument document)
            => (SvgPaintServer)_createPaintServerMethod.Invoke(null, new object[] { color, document });
        internal static void SetColor(SvgDocument document, string color)
        {
            var paint = CreatePaintServer(color, document);

            foreach (var child in document.Children)
            {
                if (child.Fill != SvgPaintServer.None)
                {
                    child.Fill = paint;
                }
            }

            if (document.Stroke != SvgPaintServer.None)
            {
                document.Stroke = paint;
            }
        }

        internal static (SvgDocument document, SvgSource source) GetSvgSource(Icons icon, float? size, float stroke, string color)
        {
            var name = icon.GetAttributeOfType<ValueAttribute>().Value;
            var uri = $"avares://TablerIcons.Avalonia/Assets/TablerIcons/{name}.svg";

            using (var stream = AssetLoader.Open(new Uri(uri)))
            {
                var document = SvgDocument.Open<SvgDocument>(stream);

                document.StrokeWidth = stroke;
                SetColor(document, color);
                if (size is float s)
                {
                    document.Height = new SvgUnit(SvgUnitType.Pixel, s);
                    document.Width = new SvgUnit(SvgUnitType.Pixel, s);
                }


                var source = new SvgSource();
                source.FromSvgDocument(document);

                return (document, source);
            }
        }
    }
}
