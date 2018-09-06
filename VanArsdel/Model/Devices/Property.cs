// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public enum PropertyEditorType { None, ToggleSwitch, ComboBox, Slider, ColorPicker, ColorPalette, BitmapPicker, String };

    public interface IProperty : INotifyPropertyChanged
    {
        string Id { get; }
        bool IsActive { set; get; }
        PropertyEditorType Editor { get; }
        string Header { get; set; }
        event Action<IProperty> ValueChanged;
        bool ComparePresetValueString(string value);
        void SetPresetValueString(string value);

        bool IsVisibleInProductEditor { get; }
        bool IsVisibleInMyLights { get; }

        IReadOnlyList<PropertyMetadataItem> Metadata { get; }
        IProperty Clone();
    }

    public abstract class Property : IProperty
    {
        public static List<IProperty> ParsePropertyList(JsonArray data, Dictionary<string, List<PropertyBitmapPickerItem>> sharedBitmapLists)
        {
            var retVal = new List<IProperty>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParseProperty(node.GetObject(), sharedBitmapLists));
            }

            return retVal;
        }

        public static IProperty ParseProperty(JsonObject data, Dictionary<string, List<PropertyBitmapPickerItem>> sharedBitmapLists)
        {
            string type = data["Type"].GetString();

            IProperty retVal = null;
            switch (type)
            {
                case "PropertyBitmapPicker":
                    retVal = PropertyBitmapPicker.ParsePropertyBitmapPicker(data, sharedBitmapLists);
                    break;
                case "PropertyBool":
                    retVal = PropertyBool.ParsePropertyBool(data);
                    break;
                case "PropertyColor":
                    retVal = PropertyColor.ParsePropertyColor(data);
                    break;
                case "PropertyColorPalette":
                    retVal = PropertyColorPalette.ParsePropertyColorPalette(data);
                    break;
                case "PropertyList":
                    retVal = PropertyList.ParsePropertyList(data);
                    break;
                case "PropertyNumber":
                    retVal = PropertyNumber.ParsePropertyNumber(data);
                    break;
                case "PropertyString":
                    retVal = PropertyString.ParsePropertyString(data);
                    break;
                default:
                    throw new Exception(string.Format("Unexpected property type {0}", type));
            }

            return retVal;
        }

        protected Property(string id, PropertyEditorType editor, string header, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
        {
            _id = id;
            _editor = editor;
            _header = header;
            _isVisibleInProductEditor = isVisibleInProductEditor;
            _isVisibleInMyLights = isVisibleInMyLights;
            _metadata = metadata;
        }

        protected readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        protected bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if(_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChangedFromSource();

                    OnIsActiveChanged();
                }
            }
        }

        protected virtual void OnIsActiveChanged() { }

        protected readonly PropertyEditorType _editor;
        public PropertyEditorType Editor
        {
            get { return _editor; }
        }

        protected string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public event Action<IProperty> ValueChanged;
        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this);
        }

        public abstract bool ComparePresetValueString(string value);
        public abstract void SetPresetValueString(string value);

        protected readonly bool _isVisibleInProductEditor;
        public bool IsVisibleInProductEditor
        {
            get { return _isVisibleInProductEditor; }
        }
        protected readonly bool _isVisibleInMyLights;
        public bool IsVisibleInMyLights
        {
            get { return _isVisibleInMyLights; }
        }

        protected List<PropertyMetadataItem> _metadata;
        public IReadOnlyList<PropertyMetadataItem> Metadata
        {
            get { return _metadata; }
        }

        public abstract IProperty Clone();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    public abstract class PropertyForwarder<T> : IProperty where T : IProperty
    {
        public PropertyForwarder(string id, PropertyEditorType editor, IEnumerable<IProperty> linkedProperties)
        {
            _id = id;
            _editor = editor;

            if (linkedProperties != null)
            {
                var linkedPropertiesCount = linkedProperties.Count();
                _linkedProperties = new List<T>(linkedPropertiesCount);

                foreach (var prop in linkedProperties)
                {
                    AddLinkedProperty(prop);
                }

                if (_linkedProperties.Count > 0)
                {
                    _linkedProperties[0].ValueChanged += PropertyForwarder_ValueChanged;
                }
            }
        }

        private void PropertyForwarder_ValueChanged(IProperty obj)
        {
            ValueChanged?.Invoke(this);
        }

        protected List<T> _linkedProperties;

        public void AddLinkedProperty(IProperty prop)
        {
            if (prop == null)
            {
                throw new ArgumentNullException("prop");
            }
            if (prop.Id != _id)
            {
                throw new ArgumentException(string.Format("Attempted to add property with id {0} to forwarder with id {1}", prop.Id, _id));
            }
            if (_linkedProperties == null)
            {
                _linkedProperties = new List<T>();
            }
            if (prop is T p)
            {
                _linkedProperties.Add(p);
            }
            else
            {
                throw new ArgumentException(string.Format("Attempted to add property of type {0} to forwarder of type {1}", prop.GetType().ToString(), typeof(T).ToString()));
            }
        }

        protected readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        protected bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChangedFromSource();

                    OnIsActiveChanged();
                }
            }
        }

        protected virtual void OnIsActiveChanged() { }

        protected readonly PropertyEditorType _editor;
        public PropertyEditorType Editor
        {
            get { return _editor; }
        }

        public string Header
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].Header;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.Header = value;
                }
            }
        }

        public event Action<IProperty> ValueChanged;

        public bool ComparePresetValueString(string value)
        {
            if (_linkedProperties == null)
            {
                return false;
            }
            else
            {
                foreach (var prop in _linkedProperties)
                {
                    if (!prop.ComparePresetValueString(value))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SetPresetValueString(string value)
        {
            if (_linkedProperties != null)
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.SetPresetValueString(value);
                }
                ValueChanged?.Invoke(this);
            }
        }

        public bool IsVisibleInProductEditor
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return false;
                }
                return _linkedProperties[0].IsVisibleInProductEditor;
            }
        }
        public bool IsVisibleInMyLights
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return false;
                }
                return _linkedProperties[0].IsVisibleInMyLights;
            }
        }

        public IReadOnlyList<PropertyMetadataItem> Metadata
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].Metadata;
            }
        }

        public IProperty Clone()
        {
            // Should never happen in this app
            throw new Exception("Clone is not supported for property forwarders");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
