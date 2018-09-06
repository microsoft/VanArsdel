// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace VanArsdel
{
    public class ColorPaletteToggleButton : ToggleButton
    {
        public ColorPaletteToggleButton()
        {
            this.SetValue(ColorBrushProperty, new SolidColorBrush(Color));
            this.SetValue(ContrastColorBrushProperty, new SolidColorBrush(ContrastColor));

            this.DefaultStyleKey = typeof(ColorPaletteToggleButton);
        }

        #region ColorProperty

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorPaletteToggleButton), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnColorPropertyChanged)));

        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteToggleButton target)
            {
                target.OnColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            SolidColorBrush brush = ColorBrush;
            if(brush == null)
            {
                brush = new SolidColorBrush(newValue);
                SetValue(ColorBrushProperty, brush);
            }
            else
            {
                brush.Color = newValue;
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        #region ColorBrushProperty

        public static readonly DependencyProperty ColorBrushProperty = DependencyProperty.Register("ColorBrush", typeof(SolidColorBrush), typeof(ColorPaletteToggleButton), new PropertyMetadata(null));

        public SolidColorBrush ColorBrush
        {
            get { return GetValue(ColorBrushProperty) as SolidColorBrush; }
        }

        #endregion

        #region ContrastColorProperty

        public static readonly DependencyProperty ContrastColorProperty = DependencyProperty.Register("ContrastColor", typeof(Color), typeof(ColorPaletteToggleButton), new PropertyMetadata(Colors.White, new PropertyChangedCallback(OnContrastColorPropertyChanged)));

        private static void OnContrastColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteToggleButton target)
            {
                target.OnContrastColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnContrastColorChanged(Color oldValue, Color newValue)
        {
            SolidColorBrush brush = ContrastColorBrush;
            if (brush == null)
            {
                brush = new SolidColorBrush(newValue);
                SetValue(ContrastColorBrushProperty, brush);
            }
            else
            {
                brush.Color = newValue;
            }
        }

        public Color ContrastColor
        {
            get { return (Color)GetValue(ContrastColorProperty); }
            set { SetValue(ContrastColorProperty, value); }
        }

        #endregion

        #region ContrastColorBrushProperty

        public static readonly DependencyProperty ContrastColorBrushProperty = DependencyProperty.Register("ContrastColorBrush", typeof(SolidColorBrush), typeof(ColorPaletteToggleButton), new PropertyMetadata(null));

        public SolidColorBrush ContrastColorBrush
        {
            get { return GetValue(ContrastColorBrushProperty) as SolidColorBrush; }
        }

        #endregion

        #region GlyphProperty

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(ColorPaletteToggleButton), new PropertyMetadata(null));

        public string Glyph
        {
            get { return GetValue(GlyphProperty) as string; }
            set { SetValue(GlyphProperty, value); }
        }

        #endregion

        #region CaptionProperty

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ColorPaletteToggleButton), new PropertyMetadata(null));

        public string Caption
        {
            get { return GetValue(CaptionProperty) as string; }
            set { SetValue(CaptionProperty, value); }
        }

        #endregion
    }
}
