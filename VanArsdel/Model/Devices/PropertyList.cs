// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public interface IPropertyList : IProperty
    {
        PropertyListItem SelectedItem { get; set; }
        IReadOnlyList<PropertyListItem> ListItems { get; }
        void SelectById(string id);
    }

    public class PropertyList : Property, IPropertyList
    {
        public static PropertyList ParsePropertyList(JsonObject data)
        {
            List<PropertyListItem> listItems = PropertyListItem.ParsePropertyListItemList(data["ListItems"].GetArray());
            string valueId = data["Value"].GetString();
            PropertyListItem selected = listItems.FirstOrDefault((a) => a.Id == valueId);
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyList(data["Id"].GetString(), data["Caption"].GetString(), selected, listItems, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyList(string id, string header, PropertyListItem selectedItem, List<PropertyListItem> listItems, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.ComboBox, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _selectedItem = selectedItem;
            _listItems = listItems;
        }

        private PropertyListItem _selectedItem;
        public PropertyListItem SelectedItem
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

        private readonly List<PropertyListItem> _listItems;
        public IReadOnlyList<PropertyListItem> ListItems
        {
            get { return _listItems; }
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
            List<PropertyListItem> clonedItems = new List<PropertyListItem>(_listItems.Count);
            for (int i = 0; i < _listItems.Count; i++)
            {
                clonedItems.Add(_listItems[i].Clone());
            }
            PropertyListItem clonedSelected = null;
            if (_selectedItem != null)
            {
                clonedSelected = clonedItems.FirstOrDefault((a) => a.Id == _selectedItem.Id);
            }
            return new PropertyList(_id, _header, clonedSelected, clonedItems, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyListForwarder : PropertyForwarder<IPropertyList>, IPropertyList
    {
        public PropertyListForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.ComboBox, linkedProperties)
        { }

        public PropertyListItem SelectedItem
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

        public IReadOnlyList<PropertyListItem> ListItems
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
