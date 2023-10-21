using Avalonia;
using Avalonia.Media;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Svg.Skia;
using HarfBuzzSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TablerIcons.Avalonia
{
    internal class TablerIconDrawOperation : ICustomDrawOperation
    {
        private readonly Rect _rect;
        private readonly IEnumerable<ISvgData> _pathData;
        private readonly float _strokeWidth;
        private readonly SKShader _sKShader;
        private readonly double _opacity;

        public TablerIconDrawOperation(Rect rect, IEnumerable<ISvgData> glyph, float strokeWidth, SKShader sKShader, double opacity)
        {
            _rect = rect;
            _pathData = glyph;
            _strokeWidth = strokeWidth;
            _sKShader = sKShader;
            _opacity = opacity;
        }

        public Rect Bounds => _rect;

        public void Dispose()
        {
        }

        public bool Equals(ICustomDrawOperation other) => false;

        public bool HitTest(Point p) => false;

        public void Render(ImmediateDrawingContext context)
        {

            if (_pathData is null)
                return;

            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();

            if (leaseFeature is null)
                return;

            using var lease = leaseFeature.Lease();

            var canvas = lease?.SkCanvas;
            if (canvas is null)
                return;


            canvas.Save();


            var defaultStrokeWidth = (_pathData
                .FirstOrDefault(x => x is DefaultStrokeWidthData)
                as DefaultStrokeWidthData);

            var strokeMultiplier = (defaultStrokeWidth?.DefaultStrokeWidth ?? 2) / 2f;

            var data = _pathData
                .Where(x => x is not DefaultStrokeWidthData);
            foreach (var p in data)
            {
                using var paint = new SKPaint();

                paint.IsAntialias = true;
                paint.Shader = _sKShader;

                paint.Color = new SKColor(255, 255, 255);
                paint.IsStroke = p.IsStroke;

                paint.StrokeWidth = _strokeWidth * strokeMultiplier;
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
            canvas.DrawCircle(cd.CenterX * scale, cd.CenterY * scale, cd.Radius * scale, paint);
        }

        private void DrawRect(RectData rd, SKPaint paint, SKCanvas canvas)
        {

            var scale = GetScalingFactor();
            canvas.DrawRoundRect(
                rd.X * scale,
                rd.Y * scale,
                rd.Width * scale,
                rd.Height * scale,
                rd.Rx * scale,
                0,
                paint);
            throw new NotImplementedException();
        }

        private SKMatrix GetScale() => SKMatrix.CreateScale(
                (float)(_rect.Width / 24),
                (float)(_rect.Height / 24));

        private float GetScalingFactor()
        {
            var currentWidth = _rect.Width > _rect.Height ? _rect.Height : _rect.Width;

            return (float)(currentWidth / 24);
        }
    }
}
