using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Svg.Skia;
using Svg;
using Avalonia.Media.Imaging;

using SvgImage = Avalonia.Svg.Skia.SvgImage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace TablerIcons.Avalonia
{
    public class TablerIconSource : SvgImage
    {
        public event EventHandler Invalidated;
        public static readonly StyledProperty<Icons?> IconProperty =
            AvaloniaProperty.Register<TablerIcon, Icons?>(nameof(Icon), null, false);

        public Icons? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<float> StrokeWidthProperty =
            AvaloniaProperty.Register<TablerIcon, float>(nameof(StrokeWidth), 2f, false);

        public float StrokeWidth
        {
            get => GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public static readonly StyledProperty<string> ColorProperty =
            AvaloniaProperty.Register<TablerIcon, string>(nameof(Color), "Black", false);

        public string Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }


        private SvgDocument _document;

        private void SetImage()
        {
            if (Icon is null)
                return;

            Source?.Dispose();
            Source = null;
            _document = null;

            (_document, Source) = Utils.GetSvgSource((Icons)Icon, 100f, StrokeWidth, Color);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Icon))
            {
                SetImage();
                Invalidated?.Invoke(this, EventArgs.Empty);
            }


            if (_document is null || Source is null)
                return;

            switch (change.Property.Name)
            {
                case nameof(Color) when Color is string:
                    Utils.SetColor(_document, Color);
                    Source.FromSvgDocument(_document);
                    break;

                case nameof(StrokeWidth):
                    _document.StrokeWidth = StrokeWidth;
                    Source.FromSvgDocument(_document);
                    break;
            }

            InvalidateImage();
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
    }
}
