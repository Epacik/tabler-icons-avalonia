using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Svg.Skia;
using HarfBuzzSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TablerIcons.Avalonia
{
    internal class TablerIconDrawOperation : ICustomDrawOperation
    {
        private readonly Rect _rect;
        private readonly IEnumerable<PathData> _pathData;
        private readonly float _strokeWidth;
        private readonly SKShader _sKShader;
        private readonly double _opacity;


        public TablerIconDrawOperation(Rect rect, IEnumerable<PathData> glyph, float strokeWidth, SKShader sKShader, double opacity)
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
            //_surface.Dispose();
            //_font.Dispose();
        }

        public bool Equals(ICustomDrawOperation other) => false;

        public bool HitTest(Point p) => false;

        public void Render(ImmediateDrawingContext context)
        {

            if (_pathData is null)
            {
                return;
            }

            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null)
            {
                return;
            }
            using (var lease = leaseFeature.Lease())
            {
                var canvas = lease?.SkCanvas;
                if (canvas is null)
                {
                    return;
                }
                
                //var tempCanvas = _surface.Canvas;

                // throw away previously rendered rect
                //tempCanvas.Clear();
                //tempCanvas.Save();



                // draw a rectangle with selected brush
                //using (var paint = new SKPaint())
                //{
                //    paint.Shader = _sKShader;
                //    paint.IsAntialias = true;
                //    paint.Color = new SKColor(255, 255, 255, (byte)(255 * _opacity));
                //    tempCanvas.DrawRect(_rect.ToSKRect(), paint);
                //}

                canvas.Save();

                // "cut out" the icon out of created rectangle

                foreach (var p in _pathData)
                {
                    if (p.Fill == "none" && p.Stroke == "none")
                        continue;

                    using (var paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        paint.Shader = _sKShader;

                        paint.Color = new SKColor(255, 255, 255);
                        paint.IsStroke = p.Stroke != "none";

                        paint.StrokeWidth = _strokeWidth;
                        paint.StrokeCap = SKStrokeCap.Round;
                        paint.StrokeJoin = SKStrokeJoin.Round;

                        var path = SKPath.ParseSvgPathData(p.Data);

                        path.Transform(SKMatrix.CreateScale(
                            (float)(_rect.Width / 24),
                            (float)(_rect.Height / 24)));

                        canvas.DrawPath(path, paint);
                        //tempCanvas.DrawPicture(_source.Picture, paint);
                        //canvas.DrawText(
                        //    _glyph,
                        //    (float)(_rect.Width / 2),
                        //    (float)(_rect.Height * 0.9f),
                        //    paint);
                    }
                }
                

                //using (var paint = new SKPaint())
                //{
                //    paint.Shader = _sKShader;
                //    paint.BlendMode = SKBlendMode.Xor;
                //    paint.IsAntialias = true;
                //    tempCanvas.DrawRect(_rect.ToSKRect(), paint);
                //}

                // draw an icon to the screen
                //canvas.Save();
                //canvas.DrawSurface(_surface, new SKPoint(0, 0));
                canvas.Restore();
                //tempCanvas.Restore();
            }
        }
    }
}
