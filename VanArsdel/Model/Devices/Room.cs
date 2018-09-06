// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace VanArsdel.Devices
{
    public class Room : INotifyPropertyChanged, IComparable<Room>
    {
        public static List<Room> ParseRoomList(JsonArray data)
        {
            var retVal = new List<Room>(data.Count);

            foreach (var roomNode in data)
            {
                retVal.Add(ParseRoom(roomNode.GetObject()));
            }

            return retVal;
        }

        public static Room ParseRoom(JsonObject data)
        {
            return new Room(data["Id"].GetString(), data["Caption"].GetString());
        }

        public Room(string id, string caption)
        {
            _id = id;
            _caption = caption;
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

        private List<Device> _devices = new List<Device>();
        public IReadOnlyList<Device> Devices
        {
            get { return _devices; }
        }

        public void AddDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            _devices.Add(device);
        }

        public void RemoveDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            _devices.Remove(device);
        }

        public void OnHeaderClick(object sender, RoutedEventArgs e)
        {
            RoomSelected?.Invoke(this);
        }

        public event Action<Room> RoomSelected;

        public int CompareTo(Room other)
        {
            if (other == null)
            {
                return -1;
            }

            if (_id == other.Id)
            {
                return 0;
            }
            else
            {
                return _id.CompareTo(other.Id);
            }
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