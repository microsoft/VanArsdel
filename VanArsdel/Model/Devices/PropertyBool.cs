// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public interface IPropertyBool : IProperty
    {
        bool Value { get; set; }
    }

    public class PropertyBool : Property, IPropertyBool
    {
        public static PropertyBool ParsePropertyBool(JsonObject data)
        {
            bool value = bool.Parse(data["Value"].GetString());
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyBool(data["Id"].GetString(), data["Caption"].GetString(), value, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyBool(string id, string header, bool value, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.ToggleSwitch, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _value = value;
        }

        private bool _value;
        public bool Value
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
            bool v;
            if (bool.TryParse(value, out v))
            {
                return _value == v;
            }
            return false;
        }

        public override void SetPresetValueString(string value)
        {
            bool v;
            if (bool.TryParse(value, out v))
            {
                Value = v;
            }
        }

        public override IProperty Clone()
        {
            return new PropertyBool(_id, _header, _value, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyBoolForwarder : PropertyForwarder<IPropertyBool>, IPropertyBool
    {
        public PropertyBoolForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.ToggleSwitch, linkedProperties)
        { }

        public bool Value
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return false;
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
