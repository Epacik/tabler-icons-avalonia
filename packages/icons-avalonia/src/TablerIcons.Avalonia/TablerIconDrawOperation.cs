using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;

namespace TablerIcons.Avalonia;

internal class TablerIconDrawOperation : ICustomDrawOperation
{
    private static readonly SKColor s_white = new SKColor(255, 255, 255, 255);
    private readonly Rect _rect;
    private readonly Icons _glyph;
    private readonly float _strokeWidth;
    private readonly SKShader _sKShader;
    private readonly float _size;
    private readonly ISvgData[] _svgData;
    //private readonly SKSurface _surface;
    //private readonly SKPoint _point;

    public TablerIconDrawOperation(Rect rect, Icons glyph, ISvgData[] data, float strokeWidth, SKShader sKShader)
    {
        _rect = rect;
        _glyph = glyph;
        _strokeWidth = strokeWidth;
        _sKShader = sKShader;
        _svgData = data;
        _size = (float)(Math.Min(_rect.Width, _rect.Height));

        //_point = new SKPoint(
        //    (float)(_rect.Width < _rect.Height ? (_rect.Height - _rect.Width) / 2 : 0),
        //    (float)(_rect.Width > _rect.Height ? (_rect.Width - _rect.Height) / 2 : 0));

        //var imageInfo = new SKImageInfo((int)_size, (int)_size);
        //_surface = SKSurface.Create(imageInfo);
    }

    public Rect Bounds => _rect;

    public void Dispose()
    {
        //_surface?.Dispose();
    }

    public bool Equals(ICustomDrawOperation other) => false;

    public bool HitTest(Point p) => false;

    public void Render(ImmediateDrawingContext context)
    {

        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();

        if (leaseFeature is null)
            return;

        using var lease = leaseFeature.Lease();

        var canvas = lease?.SkCanvas;//_surface.Canvas;
        if (canvas is null/* || lease?.SkCanvas is null*/)
            return;


        canvas.Save();

        var pathData = _svgData;

        var strokeMultiplier = pathData[0] is DefaultStrokeWidthData dswd ? (dswd.DefaultStrokeWidth / 2f) : 1f;

        var strokeWidth = _strokeWidth * strokeMultiplier * (_size / 24f);

        foreach (var p in pathData)
        {
            if (p is DefaultStrokeWidthData)
                continue;

            using var paint = new SKPaint();

            paint.IsAntialias = true;
            paint.Shader = _sKShader;

            paint.Color = s_white;
            paint.Style = (p.IsStroke, p.IsFill) switch
            {
                (true, true) => SKPaintStyle.StrokeAndFill,
                (true, false) => SKPaintStyle.Stroke,
                (false, true) => SKPaintStyle.Fill,
                _ => SKPaintStyle.Fill,
            };
            paint.StrokeWidth = strokeWidth;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeJoin = SKStrokeJoin.Round;


            switch (p)
            {
                case PathData pd:
                    DrawPath(pd, paint, canvas);
                    break;
                case CircleData cd:
                    DrawCircle(cd, paint, canvas);
                    break;
                case RectData rd:
                    DrawRect(rd, paint, canvas);
                    break;
            }
        }

        //lease.SkCanvas.Save();
        //lease.SkCanvas.DrawSurface(_surface, _point);
        //lease.SkCanvas.Restore();

        canvas.Restore();
    }

    private void DrawPath(PathData pd, SKPaint paint, SKCanvas canvas)
    {
        var path = SKPath.ParseSvgPathData(pd.Data);

        path.Transform(GetScale());

        canvas.DrawPath(path, paint);
    }

    private void DrawCircle(CircleData cd, SKPaint paint, SKCanvas canvas)
    {
        var scale = GetScalingFactor();
        canvas.DrawCircle(
            (cd.CenterX * scale) + OffsetX,
            (cd.CenterY * scale) + OffsetY,
            cd.Radius * scale, paint);
    }

    private void DrawRect(RectData rd, SKPaint paint, SKCanvas canvas)
    {

        var scale = GetScalingFactor();
        canvas.DrawRoundRect(
            (rd.X * scale) + OffsetX,
            (rd.Y * scale) + OffsetY,
            rd.Width * scale,
            rd.Height * scale,
            rd.Rx * scale,
            0,
            paint);
    }

    private SKMatrix GetScale()
    {
        var size = Math.Min(_rect.Width, _rect.Height);
        var scale = SKMatrix.CreateScale(
            (float)(size / 24),
            (float)(size / 24));

        var translate = SKMatrix.CreateTranslation(
            OffsetX, OffsetY);

        return scale.PostConcat(translate);

    }

    private float GetScalingFactor()
    {
        var currentWidth = Math.Min(_rect.Width, _rect.Height);

        return (float)(currentWidth / 24);
    }

    private float OffsetX => (_rect.Width > _rect.Height) switch
    {
        true => (float)((_rect.Width - _rect.Height) / 2),
        false => 0f,
    };

    private float OffsetY => (_rect.Width < _rect.Height) switch
    {
        true => (float)((_rect.Height - _rect.Width) / 2),
        false => 0f,
    };
}
