// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public class PropertyListItem : INotifyPropertyChanged
    {
        public static List<PropertyListItem> ParsePropertyListItemList(JsonArray data)
        {
            var retVal = new List<PropertyListItem>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParsePropertyListItem(node.GetObject()));
            }

            return retVal;
        }

        public static PropertyListItem ParsePropertyListItem(JsonObject data)
        {
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyListItem(data["Id"].GetString(), data["Caption"].GetString(), metadata);
        }

        public PropertyListItem(string id, string caption, List<PropertyMetadataItem> metadata)
        {
            _id = id;
            _caption = caption;
            _metadata = metadata;
        }

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private List<PropertyMetadataItem> _metadata;
        public IReadOnlyList<PropertyMetadataItem> Metadata
        {
            get { return _metadata; }
        }

        public PropertyListItem Clone()
        {
            return new PropertyListItem(_id, _caption, _metadata.Clone());
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
