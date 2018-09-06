// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml.Markup;

namespace VanArsdel.Devices
{
    public interface IPropertyColor : IProperty
    {
        Color Value { get; set; }
    }

    public class PropertyColor : Property, IPropertyColor
    {
        public static PropertyColor ParsePropertyColor(JsonObject data)
        {
            Color value = VanArsdel.Utils.ColorUtils.ParseColorString(data["Value"].GetString());
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyColor(data["Id"].GetString(), data["Caption"].GetString(), value, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyColor(string id, string header, Color value, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.ColorPicker, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _value = value;
        }

        private Color _value;
        public Color Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaiseValueChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public override bool ComparePresetValueString(string value)
        {
            object boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
            if (boxedColor is Color c)
            {
                return c == _value;
            }
            return false;
        }

        public override void SetPresetValueString(string value)
        {
            object boxedColor = XamlBindingHelper.ConvertValue(typeof(Color), value);
            if (boxedColor is Color c)
            {
                Value = c;
            }
        }

        public override IProperty Clone()
        {
            return new PropertyColor(_id, _header, _value, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyColorForwarder : PropertyForwarder<IPropertyColor>, IPropertyColor
    {
        public PropertyColorForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.ColorPicker, linkedProperties)
        { }

        public Color Value
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return Colors.Black;
                }
                return _linkedProperties[0].Value;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.Value = value;
                }
            }
        }
    }
}
