// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml.Markup;

namespace VanArsdel.Devices
{
    public interface IPropertyColorPalette : IProperty
    {
        Color SelectedColor { get; set; }
        Color CustomColor { get; set; }
        IReadOnlyList<Color> Colors { get; }
    }

    public class PropertyColorPalette : Property, IPropertyColorPalette
    {
        public static PropertyColorPalette ParsePropertyColorPalette(JsonObject data)
        {
            Color value = VanArsdel.Utils.ColorUtils.ParseColorString(data["Value"].GetString());
            Color customColor = VanArsdel.Utils.ColorUtils.ParseColorString(data["CustomColor"].GetString());
            JsonArray paletteData = data["Palette"].GetArray();
            List<Color> palette = new List<Color>();
            foreach (var node in paletteData)
            {
                palette.Add(VanArsdel.Utils.ColorUtils.ParseColorString(node.GetString()));
            }

            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyColorPalette(data["Id"].GetString(), data["Caption"].GetString(), value, palette, customColor, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyColorPalette(string id, string header, Color selectedColor, IReadOnlyList<Color> colors, Color customColor, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
           : base(id, PropertyEditorType.ColorPalette, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _colors = colors;
            _selectedColor = selectedColor;
            _customColor = customColor;
        }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    RaiseValueChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private Color _customColor;
        public Color CustomColor
        {
            get { return _customColor; }
            set
            {
                if (_customColor != value)
                {
                    _customColor = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private readonly IReadOnlyList<Color> _colors;
        public IReadOnlyList<Color> Colors
        {
            get { return _colors; }
        }

        public override bool ComparePresetValueString(string value)
        {
            object boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
            if (boxedColor is Color c)
            {
                return c == _selectedColor;
            }
            return false;
        }

        public override void SetPresetValueString(string value)
        {
            object boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
            if (boxedColor is Color c)
            {
                if (!_colors.Contains(c))
                {
                    CustomColor = c;
                }
                SelectedColor = c;
            }
        }


        public override IProperty Clone()
        {
            List<Color> _clonedColors = new List<Color>(_colors.Count);
            for (int i = 0; i < _colors.Count; i++)
            {
                _clonedColors.Add(_colors[i]);
            }
            return new PropertyColorPalette(_id, _header, _selectedColor, _clonedColors, _customColor, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyColorPaletteForwarder : PropertyForwarder<IPropertyColorPalette>, IPropertyColorPalette
    {
        public PropertyColorPaletteForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.ColorPalette, linkedProperties)
        { }

        public Color SelectedColor
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return Windows.UI.Colors.Black;
                }
                return _linkedProperties[0].SelectedColor;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.SelectedColor = value;
                }
            }
        }

        public Color CustomColor
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return Windows.UI.Colors.White;
                }
                return _linkedProperties[0].CustomColor;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.CustomColor = value;
                }
            }
        }

        public IReadOnlyList<Color> Colors
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].Colors;
            }
        }
    }
}
