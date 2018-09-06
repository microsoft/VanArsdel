// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VanArsdel.Devices
{
    public interface IPropertyString : IProperty
    {
        string Value { get; set; }
        string RawText { get; set; }
        void RichEditBox_TextChanged(object sender, RoutedEventArgs e);
    }

    public class PropertyString : Property, IPropertyString
    {
        public static PropertyString ParsePropertyString(JsonObject data)
        {
            string value = data["Value"].GetString();
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyString(data["Id"].GetString(), data["Caption"].GetString(), value, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyString(string id, string header, string value, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.String, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _value = value;
        }

        private string _value;
        public string Value
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

        private string _rawText;
        public string RawText
        {
            get { return _rawText; }
            set
            {
                if(_rawText != value)
                {
                    _rawText = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if(sender is RichEditBox source)
            {
                source.Document.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out _value);
                source.Document.GetText(Windows.UI.Text.TextGetOptions.None, out _rawText);
                RaiseValueChanged();
            }
        }

        public override bool ComparePresetValueString(string value)
        {
            return _value == value;
        }

        public override void SetPresetValueString(string value)
        {
            Value = value;
        }

        public override IProperty Clone()
        {
            return new PropertyString(_id, _header, _value, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyStringForwarder : PropertyForwarder<IPropertyString>, IPropertyString
    {
        public PropertyStringForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.String, linkedProperties)
        { }

        public string Value
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
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

        public string RawText
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].RawText;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.RawText = value;
                }
            }
        }

        public void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_linkedProperties == null || _linkedProperties.Count == 0)
            {
                return;
            }
            _linkedProperties[0].RichEditBox_TextChanged(sender, e);
        }
    }
}
