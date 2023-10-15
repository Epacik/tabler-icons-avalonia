using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using Avalonia.VisualTree;
using Svg;
using System;
using System.Linq;
using SvgImage = Avalonia.Svg.Skia.SvgImage;

namespace TablerIcons.Avalonia
{
    public partial class TablerIcon : Control
    {
        static TablerIcon()
        {
            AffectsRender<TablerIcon>(
                IconProperty,
                StrokeWidthProperty,
                WidthProperty,
                MaxWidthProperty,
                MinWidthProperty,
                HeightProperty,
                MaxHeightProperty,
                MinHeightProperty,
                BrushProperty
                );

            WidthProperty.OverrideDefaultValue(typeof(TablerIcon), 30);
            HeightProperty.OverrideDefaultValue(typeof(TablerIcon), 30);
        }
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

        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<TablerIcon, IBrush>(
                nameof(Brush),
                Brushes.Black,
                false,
                BindingMode.TwoWay);

        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(Icon):
                case nameof(StrokeWidth):
                case nameof(Brush):
                case nameof(Width):
                case nameof(MaxWidth):
                case nameof(MinWidth):
                case nameof(Height):
                case nameof(MaxHeight):
                case nameof(MinHeight):
                    var visuals = this.GetVisualChildren();
                    var img = visuals.FirstOrDefault(x => x is Image);
                    img?.InvalidateVisual();
                    break;
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var source = new TablerIconSource()
            {
                Brush = Brush,
                Icon = Icon,
                StrokeWidth = StrokeWidth,
            };

            var rect = new Rect(DesiredSize - Margin);
            source.Draw(context, rect, rect);
        }

    }
}