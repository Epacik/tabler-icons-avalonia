using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia
{
    internal interface ISvgData
    {
        bool IsStroke { get; }
    }
    internal class PathData : ISvgData
    {
        public PathData(string data, bool isStroke)
        {
            Data = data;
            IsStroke = isStroke;
        }

        public string Data { get; }
        public bool IsStroke { get; }
    }

    internal class CircleData : ISvgData
    {
        public CircleData(float cx, float cy, float r, bool isStroke)
        {
            Cx = cx;
            Cy = cy;
            R = r;
            IsStroke = isStroke;
        }

        public float Cx { get; }
        public float Cy { get; }
        public float R { get; }
        public bool IsStroke { get; }
    }

    internal class RectData : ISvgData
    {
        public RectData(float x, float y, float width, float height, float rx, bool isStroke)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Rx = rx;
            IsStroke = isStroke;
        }

        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public float Rx { get; }
        public bool IsStroke { get; }
    }
}
