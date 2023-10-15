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
    public class TablerIconSource : StyledElement, IImage
    {
        private SvgDocument _document;
        private SvgSource _source;
        static TablerIconSource()
        {
        }

        public Size Size => new Size(
            _source?.Picture.CullRect.Width ?? 0,
            _source?.Picture.CullRect.Height ?? 0);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (_source is null)
                return;

            bool reloadPic = false;

            if (_document.Width.Value != destRect.Width)
            {
                _document.Width = new SvgUnit(SvgUnitType.Pixel, (float)destRect.Width);
                reloadPic = true;
            }
            if (_document.Height.Value != destRect.Height)
            {
                _document.Height = new SvgUnit(SvgUnitType.Pixel, (float)destRect.Height);
                reloadPic = true;
            }

            if (reloadPic)
            {
                _source.FromSvgDocument(_document);
            }

            var picSize = _source.Picture.CullRect;
            if (picSize.Width < 0 || picSize.Height < 0)
                return;

            using (context.PushClip(destRect))
            {
                context.Custom(
                    new TablerIconDrawOperation(
                        new Rect(0, 0, picSize.Width, picSize.Height),
                        _source,
                        GetShader(picSize.ToAvaloniaRect()),
                        _brush?.Opacity ?? 1));
            }
        }


        public static readonly StyledProperty<Icons?> IconProperty =
            AvaloniaProperty.Register<TablerIconSource, Icons?>(
                nameof(Icon),
                null,
                false,
                BindingMode.TwoWay);

        public Icons? Icon
        {
            get => GetValue(IconProperty);
            set
            {
                SetValue(IconProperty, value);
                InvalidateImage();
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

        private IBrush _brush;
        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set
            {
                SetValue(BrushProperty, value);
                _brush = value;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Icon))
            {
                if (_source is SvgSource)
                {
                    _source.Dispose();
                }

                if (Icon is Icons icon)
                {
                    (_document, _source) = GetSvgSource(
                        icon,
                        100f,
                        _strokeWidth);
                }
            }


            if (_document is null || _source is null)
                return;

            switch (change.Property.Name)
            {
                case nameof(StrokeWidth):
                    _document.StrokeWidth = StrokeWidth;
                    _source.FromSvgDocument(_document);
                    InvalidateImage();
                    break;

                case nameof(Brush):
                case nameof(Icon):
                    InvalidateImage();
                    break;
            }

            
        }
        private SKShader GetShader(Rect destRect)
        {
            switch (_brush)
            {
                case ISolidColorBrush sb:
                    return CreateSolidColorShader(sb);
                case ILinearGradientBrush lgb:
                    return CreateLinearGradientShader(lgb, destRect);
                case IRadialGradientBrush rgb:
                    return CreateRadialGradientShader(rgb, destRect);
                case IConicGradientBrush cgb:
                    return CreateConicGradientShader(cgb, destRect);
                default:
                    return SKShader.CreateEmpty();
                    //throw new InvalidOperationException(
                    //"Used gradient has to be of one of the following types: \n" +
                    //    "\tAvalonia.Media.ISolidColorBrush,\n" +
                    //    "\tAvalonia.Media.ILinearGradientBrush,\n" +
                    //    "\tAvalonia.Media.IRadialGradientBrush,\n" +
                    //    "\tAvalonia.Media.IConicGradientBrush");
            }
        }

        private SKShader CreateConicGradientShader(IConicGradientBrush gradient, Rect destRect)
        {
            var (colors, colorPos) = GetGradientStops(
                gradient.GradientStops,
                gradient.Opacity);

            var angle = (float)Matrix.ToRadians(gradient.Angle - 90);
            var rotationMatrix = SKMatrix.CreateRotation(
                    angle,
                    (float)(destRect.Width / 2),
                    (float)(destRect.Height / 2));

            var matrix = rotationMatrix;

            if (gradient.Transform is ITransform transform)
            {
                var brushMatrix = transform.Value.ToSKMatrix();
                matrix = matrix.PostConcat(brushMatrix);
            }

            var center = GetSKPoint(gradient.Center, destRect);
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

            var matrix = SKMatrix.CreateRotation(
                    0,
                    (float)(destRect.Width / 2),
                    (float)(destRect.Height / 2));

            if (gradient.Transform is ITransform transform)
            {
                var brushMatrix = transform.Value.ToSKMatrix();
                matrix = matrix.PostConcat(brushMatrix);
            }

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

            return SKShader.CreateLinearGradient(
                GetSKPoint(gradient.StartPoint, destRect),
                GetSKPoint(gradient.EndPoint, destRect),
                colors,
                colorPos,
                gradient.SpreadMethod.ToSKShaderTileMode(),
                matrix);
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
        private void InvalidateImage()
        {
            var lifetime = Application.Current.ApplicationLifetime;
            if (lifetime is IClassicDesktopStyleApplicationLifetime classic)
            {
                foreach (var window in classic.Windows)
                {
                    foreach (var visual in (IEnumerable<Visual>)window.GetVisualDescendants())
                    {
                        var img = visual as Image;

                        if (img is null || img.Source != this)
                            continue;

                        img.InvalidateVisual();
                    }
                }
            }
            else if (lifetime is ISingleViewApplicationLifetime singleView)
            {
                foreach (var visual in (IEnumerable<Visual>)singleView.MainView.GetVisualDescendants())
                {
                    var img = visual as Image;

                    if (img is null || img.Source != this)
                        continue;

                    img.InvalidateVisual();
                }
            }
        }

        internal static (SvgDocument document, SvgSource source) GetSvgSource(Icons icon, float? size, float stroke)
        {
            var name = icon.GetAttributeOfType<ValueAttribute>().Value;
            var uri = $"avares://TablerIcons.Avalonia/Assets/TablerIcons/{name}.svg";

            using (var stream = AssetLoader.Open(new Uri(uri)))
            {
                var document = SvgDocument.Open<SvgDocument>(stream);

                document.StrokeWidth = stroke;
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
