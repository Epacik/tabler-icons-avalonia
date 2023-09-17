using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using Svg;
using System;
using SvgImage = Avalonia.Svg.Skia.SvgImage;

namespace TablerIcons.Avalonia
{
    public partial class TablerIcon : UserControl
    {
        public TablerIcon()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<Icons?> IconProperty =
            AvaloniaProperty.Register<TablerIcon, Icons?>(
                nameof(Icon),
                null,
                false,
                BindingMode.TwoWay);
        public Icons? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<double> SizeProperty =
            AvaloniaProperty.Register<TablerIcon, double>(
                nameof(Size),
                16,
                false,
                BindingMode.TwoWay);

        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public static readonly StyledProperty<float> StrokeWidthProperty =
            AvaloniaProperty.Register<TablerIcon, float>(
                nameof(StrokeWidth),
                2f,
                false,
                BindingMode.TwoWay);

        public float StrokeWidth
        {
            get => GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public static readonly StyledProperty<string> ColorProperty =
            AvaloniaProperty.Register<TablerIcon, string>(
                nameof(Color),
                "Black",
                false,
                BindingMode.TwoWay);

        public string Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        private SvgDocument _document;
        private SvgSource _source;

        private void SetImage()
        {
            if (Icon is null)
                return;

            _source?.Dispose();
            _source = null;

            (_document, _source) = Utils.GetSvgSource((Icons)Icon, (float)Size, StrokeWidth, Color);

            Img.Source = new SvgImage { Source = _source };
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property.Name == nameof(Icon))
            {
                SetImage();
            }

            if (_document is null || _source is null)
                return;

            switch (change.Property.Name)
            {
                case nameof(Color) when Color is string:
                    Utils.SetColor(_document, Color);
                    _source.FromSvgDocument(_document);
                    break;

                case nameof(StrokeWidth):
                    _document.StrokeWidth = StrokeWidth;
                    _source.FromSvgDocument(_document);
                    break;
            }

            Img.InvalidateVisual();
        }
    }
}