using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Svg.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia
{
    public class TablerIconDrawOperation : ICustomDrawOperation
    {
        private readonly Rect _rect;
        private readonly SvgSource _source;
        private readonly SKSurface _surface;
        private readonly SKShader _sKShader;
        private readonly double _opacity;
        private bool rendered = false;

        public TablerIconDrawOperation(Rect rect, SvgSource source, SKShader sKShader, double opacity)
        {
            _rect = rect;
            _source = source;
            _sKShader = sKShader;
            _opacity = opacity;
            SKImageInfo imageInfo = new SKImageInfo((int)rect.Width, (int)rect.Height);
            _surface = SKSurface.Create(imageInfo);
        }

        public Rect Bounds => _rect;

        public void Dispose()
        {
            _surface.Dispose();
        }

        public bool Equals(ICustomDrawOperation other) => false;

        public bool HitTest(Point p) => false;

        public void Render(ImmediateDrawingContext context)
        {

            if (_source?.Picture is null)
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
                
                var tempCanvas = _surface.Canvas;

                // throw away previously rendered rect
                tempCanvas.Clear();
                tempCanvas.Save();

                // draw a rectangle with selected brush
                using (var paint = new SKPaint())
                {
                    paint.Shader = _sKShader;
                    paint.IsAntialias = true;
                    paint.Color = new SKColor(255, 255, 255, (byte)(255 * _opacity));
                    tempCanvas.DrawRect(_rect.ToSKRect(), paint);
                }

                // "cut out" the icon out of created rectangle
                using (var paint = new SKPaint())
                {
                    paint.BlendMode = SKBlendMode.DstIn;
                    paint.IsAntialias = true;
                    tempCanvas.DrawPicture(_source.Picture, paint);
                }

                // draw an icon to the screen
                canvas.Save();
                canvas.DrawSurface(_surface, new SKPoint(0, 0));
                canvas.Restore();
                tempCanvas.Restore();
            }
        }
    }
}
