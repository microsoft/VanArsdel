// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using VanArsdel.Utils;
using System.Collections.Generic;
using System.Numerics;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace VanArsdel.Devices
{
    public class DeviceCategory
    {
        public static List<DeviceCategory> ParseDeviceCategoryList(JsonArray data, IStringProvider stringProvider, IContextActionProvider contextActionProvider)
        {
            var retVal = new List<DeviceCategory>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParseDeviceCategory(node.GetObject(), stringProvider, contextActionProvider));
            }

            return retVal;
        }

        public static DeviceCategory ParseDeviceCategory(JsonObject data, IStringProvider stringProvider, IContextActionProvider contextActionProvider)
        {
            bool showDeviceShelf = bool.Parse(data["ShowDeviceShelf"].GetString());
            Vector3 shelfOffset = data["ShelfOffset"].GetObject().GetVector3();
            Vector3 deviceOffset = data["DeviceOffset"].GetObject().GetVector3();
            Vector3 spotlightOffset = data["SpotlightOffset"].GetObject().GetVector3();
            Vector3 spotlight1Offset = data["Spotlight1Offset"].GetObject().GetVector3();
            Vector3 pointlightOffset = data["PointlightOffset"].GetObject().GetVector3();

            Dictionary<string, List<PropertyBitmapPickerItem>> sharedBitmapLists = new Dictionary<string, List<PropertyBitmapPickerItem>>();
            JsonArray nodes = data["SharedBitmapLists"].GetArray();
            foreach (var node in nodes)
            {
                JsonObject nodeObject = node.GetObject();
                string id = nodeObject["Id"].GetString();
                List<PropertyBitmapPickerItem> list = PropertyBitmapPickerItem.ParsePropertyBitmapPickerItemList(nodeObject["List"].GetArray());
                sharedBitmapLists.Add(id, list);
            }

            var retVal = new DeviceCategory(data["Id"].GetString(), showDeviceShelf, shelfOffset, deviceOffset, spotlightOffset, spotlight1Offset, pointlightOffset, sharedBitmapLists, null, data["Caption"].GetString(), data["Thumbnail"].GetString(), data["ThumbnailHeight"].GetNumber(), data["ThumbnailTopMargin"].GetNumber());
            var defaultDevice = Device.ParseDevice(data["DefaultDevice"].GetObject(), stringProvider, contextActionProvider, null, retVal);
            retVal.DefaultDevice = defaultDevice;
            return retVal;
        }

        public DeviceCategory(string id, bool showDeviceShelf, Vector3 shelfOffset, Vector3 deviceOffset, Vector3 spotlightOffset, Vector3 spotlight1Offset, Vector3 pointlightOffset, Dictionary<string, List<PropertyBitmapPickerItem>> sharedBitmapLists, Device defaultDevice, string caption, string thumbnailPath, double thumbnailHeight, double thumbnailTopMargin)
        {
            _id = id;
            _showDeviceShelf = showDeviceShelf;
            _shelfOffsett = shelfOffset;
            _deviceOffset = deviceOffset;
            _spotlightOffset = spotlightOffset;
            _spotlight1Offset = spotlight1Offset;
            _pointlightOffset = pointlightOffset;
            _sharedBitmapLists = sharedBitmapLists;
            _defaultDevice = defaultDevice;
            _caption = caption;
            _thumbnailPath = thumbnailPath;
            _thumbnailHeight = thumbnailHeight;
            _thumbnailMargin = new Thickness(0, thumbnailTopMargin, 0, 0);
        }

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private readonly bool _showDeviceShelf;
        public bool ShowDeviceShelf
        {
            get { return _showDeviceShelf; }
        }

        private readonly Vector3 _shelfOffsett;
        public Vector3 ShelfOffsett
        {
            get { return _shelfOffsett; }
        }

        private readonly Vector3 _deviceOffset;
        public Vector3 DeviceOffset
        {
            get { return _deviceOffset; }
        }

        private readonly Vector3 _spotlightOffset;
        public Vector3 SpotlightOffset
        {
            get { return _spotlightOffset; }
        }

        private readonly Vector3 _spotlight1Offset;
        public Vector3 Spotlight1Offset
        {
            get { return _spotlight1Offset; }
        }

        private readonly Vector3 _pointlightOffset;
        public Vector3 PointlightOffset
        {
            get { return _pointlightOffset; }
        }

        private Dictionary<string, List<PropertyBitmapPickerItem>> _sharedBitmapLists;
        public Dictionary<string, List<PropertyBitmapPickerItem>> SharedBitmapLists
        {
            get { return _sharedBitmapLists; }
        }

        private Device _defaultDevice;
        public Device DefaultDevice
        {
            get { return _defaultDevice; }
            set { _defaultDevice = value; }
        }

        private readonly string _caption;
        public string Caption
        {
            get { return _caption; }
        }

        private readonly string _thumbnailPath;
        public string ThumbnailPath
        {
            get { return _thumbnailPath; }
        }

        private readonly double _thumbnailHeight;
        public double ThumbnailHeight
        {
            get { return _thumbnailHeight; }
        }

        private readonly Thickness _thumbnailMargin;
        public Thickness ThumbnailMargin
        {
            get { return _thumbnailMargin; }
        }
    }
}