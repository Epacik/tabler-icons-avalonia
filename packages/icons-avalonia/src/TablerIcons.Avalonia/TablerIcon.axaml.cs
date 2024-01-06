using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;

namespace TablerIcons
{
    public sealed partial class TablerIcon : Control
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
            var size = Math.Min(rect.Width, rect.Height);
            var centeredRect = rect.CenterRect(new Rect(0, 0, size, size));

            centeredRect = centeredRect
                .WithX(-centeredRect.X)
                .WithY(-centeredRect.Y);


            //source.Draw(context, rect, rect);

            context.DrawImage(source, rect);
        }

    }
}