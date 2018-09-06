// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VanArsdel.Utils;
using Windows.Data.Json;
using Windows.Foundation;

namespace VanArsdel.Devices
{
    public class PropertyBitmapPickerItem : INotifyPropertyChanged
    {
        public static List<PropertyBitmapPickerItem> ParsePropertyBitmapPickerItemList(JsonArray data)
        {
            var retVal = new List<PropertyBitmapPickerItem>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParsePropertyBitmapPickerItem(node.GetObject()));
            }

            return retVal;
        }

        public static PropertyBitmapPickerItem ParsePropertyBitmapPickerItem(JsonObject data)
        {
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            string thumbnail = null;
            if (data.ContainsKey("Thumbnail"))
            {
                thumbnail = data["Thumbnail"].GetString();
            }
            else
            {
                thumbnail = data["Path"].GetString();
            }
            Size? assetSize = null;
            if (data.ContainsKey("Size"))
            {
                assetSize = data["Size"].GetObject().GetSize();
            }
            return new PropertyBitmapPickerItem(data["Id"].GetString(), data["Caption"].GetString(), data["Path"].GetString(), assetSize, thumbnail, metadata);
        }

        public PropertyBitmapPickerItem(string id, string caption, string assetPath, Size? assetSize, string thumbnailPath, List<PropertyMetadataItem> metadata)
        {
            _id = id;
            _caption = caption;
            _assetPath = assetPath;
            _assetSize = assetSize;
            _thumbnailPath = thumbnailPath;
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

        private string _assetPath;
        public string AssetPath
        {
            get { return _assetPath; }
            set
            {
                if (_assetPath != value)
                {
                    _assetPath = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private Size? _assetSize;
        public Size? AssetSize
        {
            get { return _assetSize; }
            set
            {
                if (_assetSize != value)
                {
                    _assetSize = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private string _thumbnailPath;
        public string ThumbnailPath
        {
            get { return _thumbnailPath; }
            set
            {
                if (_thumbnailPath != value)
                {
                    _thumbnailPath = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private List<PropertyMetadataItem> _metadata;
        public IReadOnlyList<PropertyMetadataItem> Metadata
        {
            get { return _metadata; }
        }

        public PropertyBitmapPickerItem Clone()
        {
            return new PropertyBitmapPickerItem(_id, _caption, _assetPath, _assetSize, _thumbnailPath, _metadata.Clone());
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