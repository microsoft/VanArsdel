// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public interface IPropertyBitmapPicker : IProperty
    {
        PropertyBitmapPickerItem SelectedItem { get; set; }
        IReadOnlyList<PropertyBitmapPickerItem> ListItems { get; }
        void SelectById(string id);
    }

    public class PropertyBitmapPicker : Property, IPropertyBitmapPicker
    {
        public static PropertyBitmapPicker ParsePropertyBitmapPicker(JsonObject data, Dictionary<string, List<PropertyBitmapPickerItem>> sharedBitmapLists)
        {
            string bitmapListId = data["Bitmaps"].GetString();
            var bitmapItems = sharedBitmapLists[bitmapListId];

            string valueId = data["Value"].GetString();
            PropertyBitmapPickerItem selected = bitmapItems.FirstOrDefault((a) => a.Id == valueId);
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyBitmapPicker(data["Id"].GetString(), data["Caption"].GetString(), selected, bitmapItems, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyBitmapPicker(string id, string header, PropertyBitmapPickerItem selectedItem, List<PropertyBitmapPickerItem> listItems, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.BitmapPicker, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _selectedItem = selectedItem;
            _listItems = listItems;
        }

        private void Item_BitmapPickerItemClicked(PropertyBitmapPickerItem obj)
        {
            SelectedItem = obj;
        }

        private PropertyBitmapPickerItem _selectedItem;
        public PropertyBitmapPickerItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;

                    RaiseValueChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private readonly List<PropertyBitmapPickerItem> _listItems;
        public IReadOnlyList<PropertyBitmapPickerItem> ListItems
        {
            get { return _listItems; }
        }

        public void SelectById(string id)
        {
            if (id == null || _listItems == null || _listItems.Count == 0)
            {
                SelectedItem = null;
                return;
            }
            foreach (var item in _listItems)
            {
                if (item.Id == id)
                {
                    SelectedItem = item;
                    return;
                }
            }
        }

        public override bool ComparePresetValueString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return _selectedItem == null;
            }
            if (_selectedItem == null)
            {
                return false;
            }
            return _selectedItem.Id == value;
        }

        public override void SetPresetValueString(string value)
        {
            SelectById(value);
        }

        public override IProperty Clone()
        {
            List<PropertyBitmapPickerItem> clonedItems = new List<PropertyBitmapPickerItem>(_listItems.Count);
            for (int i = 0; i < _listItems.Count; i++)
            {
                clonedItems.Add(_listItems[i].Clone());
            }
            PropertyBitmapPickerItem clonedSelected = null;
            if (_selectedItem != null)
            {
                clonedSelected = clonedItems.FirstOrDefault((a) => a.Id == _selectedItem.Id);
            }
            return new PropertyBitmapPicker(_id, _header, clonedSelected, clonedItems, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyBitmapPickerForwarder : PropertyForwarder<IPropertyBitmapPicker>, IPropertyBitmapPicker
    {
        public PropertyBitmapPickerForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.BitmapPicker, linkedProperties)
        {
            foreach (var prop in linkedProperties)
            {
                prop.ValueChanged += Prop_ValueChanged;
            }
        }

        protected override void OnIsActiveChanged()
        {
            if (_linkedProperties != null)
            {
                if (_isActive)
                {
                    foreach (var prop in _linkedProperties)
                    {
                        prop.ValueChanged -= Prop_ValueChanged;
                        prop.ValueChanged += Prop_ValueChanged;
                    }
                }
                else
                {
                    foreach (var prop in _linkedProperties)
                    {
                        prop.ValueChanged -= Prop_ValueChanged;
                    }
                }
            }
        }

        private void Prop_ValueChanged(IProperty source)
        {
            if(!_isActive)
            {
                return;
            }
            if (source is IPropertyBitmapPicker sourcePicker)
            {
                if (_linkedProperties != null)
                {
                    foreach (var prop in _linkedProperties)
                    {
                        if (prop != sourcePicker)
                        {
                            prop.SelectById(sourcePicker.SelectedItem?.Id);
                        }
                    }
                }
            }
        }

        public PropertyBitmapPickerItem SelectedItem
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].SelectedItem;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.SelectById(value?.Id);
                }
            }
        }

        public void SelectById(string id)
        {
            foreach (var prop in _linkedProperties)
            {
                prop.SelectById(id);
            }
        }

        public IReadOnlyList<PropertyBitmapPickerItem> ListItems
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return null;
                }
                return _linkedProperties[0].ListItems;
            }
        }
    }
}
