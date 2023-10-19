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
    public class TablerIconDrawOperation : ICustomDrawOperation
    {
        private static readonly SKTypeface _typeface;
        private readonly Rect _rect;
        private readonly string _glyph;
        private readonly float _strokeWidth;
        private readonly SKShader _sKShader;
        private readonly double _opacity;

        static TablerIconDrawOperation()
        {
            var uri = $"avares://TablerIcons.Avalonia/Assets/tabler-icons.woff2";
            var font = AssetLoader.Open(new Uri(uri));
            //_typeface = SKTypeface.FromStream(font);
            _typeface = SKFontManager.Default.CreateTypeface(font, 0);
        }

        public TablerIconDrawOperation(Rect rect, string glyph, float strokeWidth, SKShader sKShader, double opacity)
        {
            _rect = rect;
            _glyph = glyph;
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

            if (_glyph is null)
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
                var weight = ((int)SKFontStyleWeight.ExtraLight);

                var face = SKFontManager.Default.MatchTypeface(
                    _typeface,
                    new SKFontStyle(
                        weight,
                        (int)SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright));
                var font = new SKFont(face, (float)(_rect.Height));
                // "cut out" the icon out of created rectangle
                using (var paint = new SKPaint(font))
                {
                    //paint.BlendMode = SKBlendMode.;
                    paint.IsAntialias = true;
                    paint.TextSize = (float)(_rect.Height);
                    paint.Shader = _sKShader;

                    paint.Color = new SKColor(255,255, 255);
                    paint.IsStroke = false;
                    paint.TextAlign = SKTextAlign.Center;

                    var path = paint.GetTextPath(
                        _glyph,
                        (float)(0),
                        (float)(_rect.Height * 0.9));

                    

                    canvas.DrawPath(path, paint);
                    //tempCanvas.DrawPicture(_source.Picture, paint);
                    //canvas.DrawText(
                    //    _glyph,
                    //    (float)(_rect.Width / 2),
                    //    (float)(_rect.Height * 0.9f),
                    //    paint);
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
