using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Data;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using Avalonia.Controls;
using SkiaSharp;
using Avalonia.Svg.Skia;
using Svg;
using Avalonia.Skia;
using Avalonia.Platform;

namespace TablerIcons.Avalonia
{
    internal class TablerIconSource : StyledElement, IImage
    {
        public TablerIconSource()
        {
            Icon = Icons.Icon123;
        }

        private Rect? _lastRect;
        public Size Size => new Size(
            _lastRect?.Width ?? 0,
            _lastRect?.Height ?? 0);


        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (_icon is null)
                return;

            var i = _icon.Value;

            if (destRect.Width < 0 || destRect.Height < 0)
                return;

            if (_lastRect != destRect)
            {
                _lastRect = destRect;
            }

            using (context.PushClip(destRect))
            {
                context.Custom(
                    new TablerIconDrawOperation(
                        destRect,
                        i,
                        _data[i],
                        _strokeWidth,
                        GetShader(destRect)));
            }
        }


        public static readonly StyledProperty<Icons?> IconProperty =
            AvaloniaProperty.Register<TablerIconSource, Icons?>(
                nameof(Icon),
                null,
                false,
                BindingMode.TwoWay);

        private Icons? _icon;
        //private ISvgData[] _data = Array.Empty<ISvgData>();
        private static readonly Dictionary<Icons, ISvgData[]> _data = new();
        public Icons? Icon
        {
            get => GetValue(IconProperty);
            set
            {
                SetValue(IconProperty, value);
                _icon = value;
                if (value is not null && !_data.ContainsKey((Icons)value))
                    _data[(Icons)value] = ((Icons)value).GetPathData();
            }
        }


        public static readonly StyledProperty<float> StrokeWidthProperty =
            AvaloniaProperty.Register<TablerIconSource, float>(
                nameof(StrokeWidth),
                2f,
                false,
                BindingMode.TwoWay);

        private float _strokeWidth = 2f;
        public float StrokeWidth
        {
            get => GetValue(StrokeWidthProperty);
            set
            {
                SetValue(StrokeWidthProperty, value);
                _strokeWidth = value;
            }
        }

        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<TablerIconSource, IBrush>(
                nameof(Brush),
                Brushes.Black,
                false,
                BindingMode.TwoWay);

        private IBrush _brush = Brushes.Black;
        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set
            {
                SetValue(BrushProperty, value);
                _brush = value;
            }
        }

        private static Dictionary<int, SKShader> _shaderCache = new();
        private SKShader GetShader(Rect destRect)
        {
            var hash = _brush.GetHashCode();

            if (_shaderCache.TryGetValue(hash, out var value))
                return value;

            var shader = _brush switch
            {
                ISolidColorBrush sb => CreateSolidColorShader(sb),
                ILinearGradientBrush lgb => CreateLinearGradientShader(lgb, destRect),
                IRadialGradientBrush rgb => CreateRadialGradientShader(rgb, destRect),
                IConicGradientBrush cgb => CreateConicGradientShader(cgb, destRect),
                _ => SKShader.CreateEmpty(),
            };

            //_shaderCache[hash] = shader;

            return shader;
        }

        private SKShader CreateConicGradientShader(IConicGradientBrush gradient, Rect destRect)
        {
            var (colors, colorPos) = GetGradientStops(
                gradient.GradientStops,
                gradient.Opacity);

            var center = GetSKPoint(gradient.Center, destRect);
            var angle = (float)Matrix.ToRadians(gradient.Angle - 90);
            var rotationMatrix = SKMatrix.CreateRotation(
                    angle,
                    center.X,
                    center.Y);

            var matrix = rotationMatrix;

            if (gradient.Transform is ITransform transform)
            {
                var brushMatrix = transform.Value.ToSKMatrix();
                matrix = matrix.PostConcat(brushMatrix);
            }

            matrix.PostConcat(Translate(destRect));

            var result = SKShader.CreateSweepGradient(
                center,
                colors,
                colorPos,
                gradient.SpreadMethod.ToSKShaderTileMode(),
                0f,
                360f,
                matrix);

            return result;
        }

        private SKShader CreateRadialGradientShader(IRadialGradientBrush gradient, Rect destRect)
        {
            var (colors, colorPos) = GetGradientStops(
                gradient.GradientStops,
                gradient.Opacity);


            var min = Math.Min(destRect.Width, destRect.Height);
            var angle = (float)Matrix.ToRadians(-90);

            var center = GetSKPoint(gradient.Center, destRect);

            var matrix = SKMatrix.Identity;

            if (gradient.Transform is ITransform transform)
            {
                var brushMatrix = transform.Value.ToSKMatrix();
                matrix = matrix.PostConcat(brushMatrix);
            }

            matrix.PostConcat(Translate(destRect));

            var result = SKShader.CreateRadialGradient(
                center,
                (float)(gradient.Radius * min),
                colors,
                colorPos,
                gradient.SpreadMethod.ToSKShaderTileMode(),
                matrix);

            return result;
        }

        private SKShader CreateLinearGradientShader(ILinearGradientBrush gradient, Rect destRect)
        {
            var (colors, colorPos) = GetGradientStops(
                gradient.GradientStops,
                gradient.Opacity);
             
            var matrix = SKMatrix.CreateIdentity();

            if (gradient.Transform is ITransform transform)
            {
                var brushMatrix = transform.Value.ToSKMatrix();
                matrix = matrix.PostConcat(brushMatrix);
            }

            
            matrix.PostConcat(Translate(destRect));

            return SKShader.CreateLinearGradient(
                GetSKPoint(gradient.StartPoint, destRect),
                GetSKPoint(gradient.EndPoint, destRect),
                colors,
                colorPos,
                gradient.SpreadMethod.ToSKShaderTileMode(),
                matrix);
        }

        private SKMatrix Translate(Rect destRect)
        {
            var offsetX = (destRect.Width > destRect.Height) switch
            {
                true => (float)((destRect.Width - destRect.Height) / 2),
                false => 0f,
            };

            var offsetY = (destRect.Width < destRect.Height) switch
            {
                true => (float)((destRect.Height - destRect.Width) / 2),
                false => 0f,
            };

            return SKMatrix.CreateTranslation(offsetX, offsetY);
        }

        private SKPoint GetSKPoint(RelativePoint point, Rect destRect)
        {
            if (point.Unit == RelativeUnit.Absolute)
                return point.Point.ToSKPoint();
            else
            {
                return new SKPoint(
                    (float)(point.Point.X * destRect.Width),
                    (float)(point.Point.Y * destRect.Height));
            }
        }

        private (SKColor[] colors, float[] positions) GetGradientStops(IReadOnlyList<IGradientStop> stops, double opacity)
        {
            var colors = new SKColor[stops.Count];
            var colorPos = new float[stops.Count];

            for (int i = 0; i < stops.Count; i++)
            {
                colors[i] = stops[i].Color.ToSKColor();
               // colors[i] = colors[i].WithAlpha((byte)(colors[i].Alpha * ((byte)(255 * opacity))));
                colorPos[i] = (float)stops[i].Offset;
            }

            return (colors, colorPos);
        }

        private SKShader CreateSolidColorShader(ISolidColorBrush sb)
        {
            return SKShader.CreateColor(sb.Color.ToSKColor().WithAlpha((byte)(sb.Opacity * 255)));
        }

    }
}
