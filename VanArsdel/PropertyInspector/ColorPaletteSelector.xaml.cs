// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace VanArsdel
{
    public sealed partial class ColorPaletteSelector : UserControl
    {
        public ColorPaletteSelector()
        {
            this.InitializeComponent();
        }

        #region ValueProperty

        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Color), typeof(ColorPaletteSelector), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnValuePropertyChanged)));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteSelector target)
            {
                target.OnValueChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnValueChanged(Color oldValue, Color newValue)
        {
            UpdateSelectionState();
        }

        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region CustomValueProperty

        public static DependencyProperty CustomValueProperty = DependencyProperty.Register("CustomValue", typeof(Color), typeof(ColorPaletteSelector), new PropertyMetadata(Colors.White, new PropertyChangedCallback(OnCustomValuePropertyChanged)));

        private static void OnCustomValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteSelector target)
            {
                target.OnCustomValueChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnCustomValueChanged(Color oldValue, Color newValue)
        {
            if(_paletteList != null)
            {
                foreach(var item in _paletteList)
                {
                    if(item.IsCustom)
                    {
                        item.Color = newValue;
                    }
                }
            }
            UpdateSelectionState();
        }

        public Color CustomValue
        {
            get { return (Color)GetValue(CustomValueProperty); }
            set { SetValue(CustomValueProperty, value); }
        }

        #endregion

        #region AllowCustomColorProperty

        public static DependencyProperty AllowCustomColorProperty = DependencyProperty.Register("AllowCustomColor", typeof(bool), typeof(ColorPaletteSelector), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowCustomColorPropertyChanged)));

        private static void OnAllowCustomColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteSelector target)
            {
                target.OnAllowCustomColorChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private void OnAllowCustomColorChanged(bool oldValue, bool newValue)
        {
            RebuildPaletteList();
        }

        public bool AllowCustomColor
        {
            get { return (bool)GetValue(AllowCustomColorProperty); }
            set { SetValue(AllowCustomColorProperty, value); }
        }

        #endregion

        #region HeaderProperty

        public static DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ColorPaletteSelector), new PropertyMetadata(null));

        public string Header
        {
            get { return GetValue(HeaderProperty) as string; }
            set { SetValue(HeaderProperty, value); }
        }

        #endregion

        #region PaletteColorsProperty

        public static DependencyProperty PaletteColorsProperty = DependencyProperty.Register("PaletteColors", typeof(IReadOnlyList<Color>), typeof(ColorPaletteSelector), new PropertyMetadata(null, new PropertyChangedCallback(OnPaletteColorsPropertyChanged)));

        private static void OnPaletteColorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteSelector target)
            {
                target.OnPaletteColorsChanged(e.OldValue as IReadOnlyList<Color>, e.NewValue as IReadOnlyList<Color>);
            }
        }

        private void OnPaletteColorsChanged(IReadOnlyList<Color> oldValue, IReadOnlyList<Color> newValue)
        {
            RebuildPaletteList();
        }

        public IReadOnlyList<Color> PaletteColors
        {
            get { return GetValue(PaletteColorsProperty) as IReadOnlyList<Color>; }
            set { SetValue(PaletteColorsProperty, value); }
        }

        #endregion

        private List<ColorPaletteSelectorItem> _paletteList;

        private void RebuildPaletteList()
        {
            if (_paletteList != null)
            {
                foreach (var paletteItem in _paletteList)
                {
                    paletteItem.PaletteItemClicked -= PaletteItem_PaletteItemClicked;
                    paletteItem.PaletteItemCustomColorChanged -= CustomItem_PaletteItemCustomColorChanged;
                }
            }

            var allowCustom = AllowCustomColor;
            var sourcePalette = PaletteColors;
            var customValue = CustomValue;
            var value = Value;

            if (sourcePalette == null || sourcePalette.Count == 0)
            {
                if (allowCustom)
                {
                    _paletteList = new List<ColorPaletteSelectorItem>(1);
                }
                else
                {
                    _paletteList = null;
                }
            }
            else
            {
                if (allowCustom)
                {
                    _paletteList = new List<ColorPaletteSelectorItem>(sourcePalette.Count + 1);
                }
                else
                {
                    _paletteList = new List<ColorPaletteSelectorItem>(sourcePalette.Count);
                }
            }

            if (sourcePalette != null)
            {
                foreach (var c in sourcePalette)
                {
                    _paletteList.Add(new ColorPaletteSelectorItem(c, null, null, c == value, false));
                }
            }

            if (allowCustom)
            {
                var customItem = new ColorPaletteSelectorItem(customValue, null, "\uE790", customValue == value, true);
                customItem.PaletteItemCustomColorChanged += CustomItem_PaletteItemCustomColorChanged;
                _paletteList.Add(customItem);
            }

            if (_paletteList != null)
            {
                foreach (var paletteItem in _paletteList)
                {
                    paletteItem.PaletteItemClicked += PaletteItem_PaletteItemClicked;
                }
            }

            PaletteItemsContainer.ItemsSource = _paletteList;
        }

        private void CustomItem_PaletteItemCustomColorChanged(ColorPaletteSelectorItem source)
        {
            if (source.IsCustom)
            {
                CustomValue = source.Color;
                Value = source.Color;
            }
        }

        private void PaletteItem_PaletteItemClicked(ColorPaletteSelectorItem source)
        {
            Value = source.Color;
        }

        private void UpdateSelectionState()
        {
            if (_paletteList == null)
            {
                return;
            }
            var c = Value;
            foreach (var paletteItem in _paletteList)
            {
                paletteItem.IsSelected = c == paletteItem.Color;
            }
        }
    }

    internal class ColorPaletteSelectorItem : INotifyPropertyChanged
    {
        public ColorPaletteSelectorItem(Color color, string caption = null, string glyph = null, bool isSelected = false, bool isCustom = false)
        {
            IsCustom = isCustom;

            _isSelected = isSelected;
            _glyph = glyph;
            _color = color;
            _caption = _color.ToString();
            SetGlyphColor();
        }

        public bool IsCustom { get; }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            private set
            {
                if (_caption != value)
                {
                    _caption = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private string _glyph;
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                if (_glyph != value)
                {
                    _glyph = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    Caption = _color.ToString();
                    SetGlyphColor();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private Color _glyphColor;
        public Color GlyphColor
        {
            get { return _glyphColor; }
            set
            {
                if(_glyphColor != value)
                {
                    _glyphColor = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private void SetGlyphColor()
        {
            var blackContrast = VanArsdel.Utils.ColorUtils.ContrastRatio(Colors.Black, _color);
            var whiteContrast = VanArsdel.Utils.ColorUtils.ContrastRatio(Colors.White, _color);
            if (blackContrast >= whiteContrast)
            {
                GlyphColor = Colors.Black;
            }
            else
            {
                GlyphColor = Colors.White;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsCustom)
            {
                if (sender is FrameworkElement sourceElement)
                {
                    Flyout pickerFlyout = new Flyout();
                    ColorPicker colorPicker = new ColorPicker();
                    colorPicker.Color = _color;
                    colorPicker.IsAlphaEnabled = false;
                    colorPicker.ColorChanged += ColorPicker_ColorChanged;
                    pickerFlyout.Placement = FlyoutPlacementMode.Bottom;
                    pickerFlyout.Content = colorPicker;
                    pickerFlyout.ShowAt(sourceElement);
                }
            }

            PaletteItemClicked?.Invoke(this);
        }

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            Color = args.NewColor;
            if (IsCustom)
            {
                PaletteItemCustomColorChanged?.Invoke(this);
            }
        }

        public event Action<ColorPaletteSelectorItem> PaletteItemClicked;
        public event Action<ColorPaletteSelectorItem> PaletteItemCustomColorChanged;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
