using SkiaSharp;

namespace TablerIcons.Avalonia;

internal interface ISvgData
{
    bool IsStroke { get; }
    bool IsFill { get; }
}
internal class PathData : ISvgData
{
    public PathData(string data, bool isStroke, bool isFill)
    {
        Data = data;
        IsStroke = isStroke;
        IsFill = isFill;
    }

    public string Data { get; }
    public bool IsStroke { get; }
    public bool IsFill { get; }
}

internal class CircleData : ISvgData
{
    public CircleData(float centerX, float centerY, float r, bool isStroke, bool isFill)
    {
        Radius = r;
        IsStroke = isStroke;
        Center = new SKPoint(centerX, centerY);
        IsFill = isFill;
    }

    public SKPoint Center { get; }
    public float CenterX => Center.X;
    public float CenterY => Center.Y;
    public float Radius { get; }
    public bool IsStroke { get; }
    public bool IsFill { get; }
}

internal class RectData : ISvgData
{
    public RectData(float x, float y, float width, float height, float rx, bool isStroke, bool isFill)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Rx = rx;
        IsStroke = isStroke;
        IsFill = isFill;
    }

    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
    public float Rx { get; }
    public bool IsStroke { get; }
    public bool IsFill { get; }
}

internal class DefaultStrokeWidthData : ISvgData
{
    public DefaultStrokeWidthData(float defaultStrokeWidth = 2)
    {
        DefaultStrokeWidth = defaultStrokeWidth;
    }
    public bool IsStroke => false;
    public bool IsFill => false;

    public float DefaultStrokeWidth { get; }
}
