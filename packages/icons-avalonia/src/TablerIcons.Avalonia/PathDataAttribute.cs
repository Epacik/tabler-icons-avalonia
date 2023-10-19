using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class PathDataAttribute : Attribute
    {
        public PathDataAttribute(string data, string stroke = null, string fill = null)
        {
            Data = data;
            Stroke = stroke;
            Fill = fill;
        }

        public string Data { get; }
        public string Stroke { get; }
        public string Fill { get; }
    }
}
