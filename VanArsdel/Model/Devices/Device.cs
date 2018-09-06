// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace VanArsdel.Devices
{
    public class Device : INotifyPropertyChanged, IComparable<Device>
    {
        public static List<Device> ParseDeviceList(JsonArray data, IStringProvider stringProvider, IContextActionProvider contextActionProvider, List<Room> rooms, List<DeviceCategory> categories)
        {
            var retVal = new List<Device>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParseDevice(node.GetObject(), stringProvider, contextActionProvider, rooms, categories));
            }

            return retVal;
        }

        public static Device ParseDevice(JsonObject data, IStringProvider stringProvider, IContextActionProvider contextActionProvider, List<Room> rooms, List<DeviceCategory> categories)
        {
            Room room = null;
            if (data.ContainsKey("Room"))
            {
                string roomId = data["Room"].GetString();
                if (!string.IsNullOrEmpty(roomId))
                {
                    room = rooms.FirstOrDefault((a) => a.Id == roomId);
                    if (room == null)
                    {
                        throw new Exception(string.Format("Unable to locate room with id {0}", roomId));
                    }
                }
            }

            string categoryId = data["Category"].GetString();
            DeviceCategory category = categories.FirstOrDefault((a) => a.Id == categoryId);
            if (category == null)
            {
                throw new Exception(string.Format("Unable to locate category with id {0}", categoryId));
            }

            List<IProperty> properties = Property.ParsePropertyList(data["Properties"].GetArray(), category.SharedBitmapLists);

            Device retVal = new Device(stringProvider, contextActionProvider, data["Id"].GetString(), category, data["Caption"].GetString(), data["Thumbnail"].GetString(), room, properties);
            if (room != null)
            {
                room.AddDevice(retVal);
            }
            return retVal;
        }

        public static Device ParseDevice(JsonObject data, IStringProvider stringProvider, IContextActionProvider contextActionProvider, List<Room> rooms, DeviceCategory category)
        {
            Room room = null;
            if (data.ContainsKey("Room"))
            {
                string roomId = data["Room"].GetString();
                if (!string.IsNullOrEmpty(roomId) && rooms != null)
                {
                    room = rooms.FirstOrDefault((a) => a.Id == roomId);
                    if (room == null)
                    {
                        throw new Exception(string.Format("Unable to locate room with id {0}", roomId));
                    }
                }
            }

            List<IProperty> properties = Property.ParsePropertyList(data["Properties"].GetArray(), category.SharedBitmapLists);

            Device retVal = new Device(stringProvider, contextActionProvider, data["Id"].GetString(), category, data["Caption"].GetString(), data["Thumbnail"].GetString(), room, properties);
            if (room != null)
            {
                room.AddDevice(retVal);
            }
            return retVal;
        }

        public Device(IStringProvider stringProvider, IContextActionProvider contextActionProvider, string id, DeviceCategory category, string caption, string thumbnailPath, Room room, List<IProperty> properties)
        {
            _stringProvider = stringProvider;
            _contextActionProvider = contextActionProvider;
            _id = id;
            _category = category;
            _caption = caption;
            _thumbnailPath = thumbnailPath;
            _room = room;
            _properties = properties;

            if (_properties != null)
            {
                foreach (IProperty prop in _properties)
                {
                    if (prop != null)
                    {
                        prop.ValueChanged += Prop_ValueChanged;
                    }
                }
            }
        }

        private void Prop_ValueChanged(IProperty obj)
        {
            PropertyValueChanged?.Invoke(this, obj);
        }

        public event Action<Device, IProperty> PropertyValueChanged;

        private IStringProvider _stringProvider;
        private IContextActionProvider _contextActionProvider;

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private readonly DeviceCategory _category;
        public DeviceCategory Category
        {
            get { return _category; }
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

        private Room _room;
        public Room Room
        {
            get { return _room; }
            set
            {
                if (_room != value)
                {
                    _room = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private readonly List<IProperty> _properties;
        public IReadOnlyList<IProperty> Properties
        {
            get { return _properties; }
        }

        public IProperty GetPropertyById(string id)
        {
            if (_properties == null || _properties.Count == 0)
            {
                return null;
            }
            return _properties.FirstOrDefault<IProperty>((a) => a.Id == id);
        }

        public T GetPropertyById<T>(string id) where T : IProperty
        {
            if (_properties == null || _properties.Count == 0)
            {
                return default(T);
            }
            foreach (var prop in _properties)
            {
                if (prop.Id == id)
                {
                    if (prop is T ret)
                    {
                        return ret;
                    }
                    throw new Exception(string.Format("Found property with id {0} but it was type {1} rather than the requested {2}", id, prop.GetType().FullName, typeof(T).FullName));
                }
            }
            return default(T);
        }

        // This is not even remotely good MVVM but is the least-bad option given the way MenuFlyout and MenuFlyoutSubItem works
        public void Device_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            Windows.Foundation.Point clickPoint;
            if (!args.TryGetPosition(sender, out clickPoint))
            {
                return;
            }

            _contextActionProvider.HandleDeviceContextRequested(this, sender, clickPoint);
        }

        public void OnDeviceRenameRequested(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            _contextActionProvider.HandleRenameRequested(this, args.SwipeControl.Content as FrameworkElement);
        }

        public void OnDeviceRemoveRequested(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            _contextActionProvider.HandleDeleteRequested(this);
        }

        public void OnDeviceMoveRequested(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            _contextActionProvider.HandleMoveRequested(this, args.SwipeControl.Content as FrameworkElement);
        }

        public int CompareTo(Device other)
        {
            if (other == null)
            {
                return -1;
            }
            if (other.Room == null)
            {
                if (_room == null)
                {
                    if (other.Caption == null)
                    {
                        if (_caption == null)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (_caption == null)
                        {
                            return 1;
                        }
                        else
                        {
                            return _caption.CompareTo(other.Caption);
                        }
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (_room == null)
                {
                    return 1;
                }
                else
                {
                    int roomSort = _room.CompareTo(other.Room);
                    if (roomSort == 0)
                    {
                        if (other.Caption == null)
                        {
                            if (_caption == null)
                            {
                                return 0;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            if (_caption == null)
                            {
                                return 1;
                            }
                            else
                            {
                                return _caption.CompareTo(other.Caption);
                            }
                        }
                    }
                    else
                    {
                        return roomSort;
                    }
                }
            }
        }

        public Device Clone()
        {
            List<IProperty> clonedProperties = new List<IProperty>(_properties.Count);
            for (int i = 0; i < _properties.Count; i++)
            {
                clonedProperties.Add(_properties[i].Clone());
            }
            return new Device(_stringProvider, _contextActionProvider, _id, _category, _caption, _thumbnailPath, _room, clonedProperties);
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

    // Helper for dealing with how CollectionViewSource does grouping
    public class DevicesGroup : IGrouping<Room, Device>
    {
        public DevicesGroup(Room room, IEnumerable<Device> devices)
        {
            Key = room;
            if(devices == null)
            {
                _devices = new List<Device>();
            }
            else
            {
                _devices = devices;
            }
        }

        private IEnumerable<Device> _devices;
        public Room Key { get; }
        public IEnumerator<Device> GetEnumerator()
        {
            return _devices.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _devices.GetEnumerator();
        }
    }

    public static class DevicesGroupHelper
    {
        public static IEnumerable<DevicesGroup> FillEmptyRooms(this IEnumerable<DevicesGroup> groups, IEnumerable<Room> fillFrom)
        {
            List<Room> rooms = fillFrom.ToList();

            foreach (var group in groups)
            {
                if (rooms.Contains(group.Key))
                {
                    rooms.Remove(group.Key);
                }

                yield return group;
            }

            foreach (var room in rooms)
            {
                yield return new DevicesGroup(room, null);
            }
        }
    }
}
