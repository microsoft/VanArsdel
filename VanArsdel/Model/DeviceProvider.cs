// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VanArsdel.Devices;
using VanArsdel.Model;
using Windows.Data.Json;
using Windows.Storage;

namespace VanArsdel
{
    public interface IDeviceProvider
    {
        Task InitializeData(IContextActionProvider contextActionProvider, string dataPath);
        Task HandleSuspend();

        IReadOnlyList<Room> Rooms { get; }
        IReadOnlyList<Device> Devices { get; }
        IReadOnlyList<DeviceCategory> DeviceCategories { get; }
        IReadOnlyList<Preset> Presets { get; }
        IReadOnlyList<Product> Products { get; }

        IReadOnlyList<Device> SelectedDevices { get; }
        event Action<IDeviceProvider, IReadOnlyList<Device>> SelectedDevicesChanged;
        void SelectDevices(IEnumerable<object> selected);
        IReadOnlyList<IProperty> ActiveProperties { get; }

        void MoveDeviceToRoom(Device device, Room target);
        void RenameDevice(Device device, string newName);
        void RemoveDevice(Device device);
        void AddNewRoom(string name);
        event Action<IDeviceProvider> RoomStructureChanged;
    }

    public class DeviceProvider : IDeviceProvider
    {
        public DeviceProvider(IStringProvider stringProvider)
        {
            _stringProvider = stringProvider;
        }

        private IStringProvider _stringProvider;

        public async Task InitializeData(IContextActionProvider contextActionProvider, string dataPath)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dataPath));
            string dataString = await FileIO.ReadTextAsync(file);
            JsonObject rootObject = JsonObject.Parse(dataString);

            _rooms = Room.ParseRoomList(rootObject["Rooms"].GetArray());
            _presets = Preset.ParsePresetList(rootObject["Presets"].GetArray());
            _deviceCategories = DeviceCategory.ParseDeviceCategoryList(rootObject["DeviceCategories"].GetArray(), _stringProvider, contextActionProvider);
            _devices = Device.ParseDeviceList(rootObject["Devices"].GetArray(), _stringProvider, contextActionProvider, _rooms, _deviceCategories);
            _products = Product.ParseProductList(rootObject["Products"].GetArray(), _deviceCategories);

            SortDevices();

            foreach (var room in _rooms)
            {
                room.RoomSelected += Room_RoomSelected;
            }
        }

        private void Room_RoomSelected(Room room)
        {
            SelectDevices(room.Devices);
        }

        public Task HandleSuspend()
        {
            return Task.CompletedTask;
        }

        private List<Room> _rooms;
        public IReadOnlyList<Room> Rooms
        {
            get { return _rooms; }
        }

        private List<Device> _devices;
        public IReadOnlyList<Device> Devices
        {
            get { return _devices; }
        }

        private List<Preset> _presets;
        public IReadOnlyList<Preset> Presets
        {
            get { return _presets; }
        }

        private List<DeviceCategory> _deviceCategories;
        public IReadOnlyList<DeviceCategory> DeviceCategories
        {
            get { return _deviceCategories; }
        }

        private List<Product> _products;
        public IReadOnlyList<Product> Products
        {
            get { return _products; }
        }

        private void SortDevices()
        {
            _rooms.Sort((a, b) => a.CompareTo(b));
            _devices.Sort((a, b) => a.CompareTo(b));
        }

        private List<Device> _selectedDevices;
        public IReadOnlyList<Device> SelectedDevices
        {
            get { return _selectedDevices; }
        }

        public event Action<IDeviceProvider, IReadOnlyList<Device>> SelectedDevicesChanged;

        public void SelectDevices(IEnumerable<object> selected)
        {
            if (selected == null)
            {
                if (_devices == null || _devices.Count == 0)
                {
                    return;
                }
                else
                {
                    _selectedDevices = new List<Device>(1);
                    _selectedDevices[0] = _devices.FirstOrDefault();
                    ActiveProperties = PropertyForwarderFactory.GenerateForwardersFromDevices(_selectedDevices);
                    SelectedDevicesChanged?.Invoke(this, _selectedDevices);
                    return;
                }
            }

            bool changed = false;
            var newCount = selected.Count();
            if (_selectedDevices != null && newCount == _selectedDevices.Count)
            {
                int index = 0;
                foreach (var o in selected)
                {
                    if (o is Device device)
                    {
                        if (_selectedDevices[index].Id != device.Id)
                        {
                            changed = true;
                            break;
                        }
                    }
                    else
                    {
                        changed = true;
                        break;
                    }
                    index++;
                }
            }
            else
            {
                changed = true;
            }
            if (!changed)
            {
                return;
            }

            _selectedDevices = new List<Device>();
            foreach (var o in selected)
            {
                if (o is Device device)
                {
                    _selectedDevices.Add(device);
                }
            }

            _selectedDevices.Sort((a, b) => a.CompareTo(b));

            ActiveProperties = PropertyForwarderFactory.GenerateForwardersFromDevices(_selectedDevices);

            SelectedDevicesChanged?.Invoke(this, _selectedDevices);
        }

        private IReadOnlyList<IProperty> _activeProperties;
        public IReadOnlyList<IProperty> ActiveProperties
        {
            get { return _activeProperties; }
            private set
            {
                if (_activeProperties != null)
                {
                    foreach (var prop in _activeProperties)
                    {
                        prop.IsActive = false;
                    }
                }

                _activeProperties = value;

                if (_activeProperties != null)
                {
                    foreach (var prop in _activeProperties)
                    {
                        prop.IsActive = true;
                    }
                }

                if (_presets != null)
                {
                    foreach (var preset in _presets)
                    {
                        preset.SetActiveProperties(_activeProperties);
                    }
                }
            }
        }

        public void RenameDevice(Device device, string newName)
        {
            device.Caption = newName;
            SortDevices();
            RoomStructureChanged?.Invoke(this);
        }

        public void RemoveDevice(Device device)
        {
            _devices.Remove(device);
            device.Room.RemoveDevice(device);
            if (_selectedDevices != null && _selectedDevices.Contains(device))
            {
                _selectedDevices.Remove(device);
            }
            RoomStructureChanged?.Invoke(this);
        }

        public void MoveDeviceToRoom(Device device, Room target)
        {
            if (device.Room.Id == target.Id)
            {
                return;
            }

            device.Room.RemoveDevice(device);
            target.AddDevice(device);
            device.Room = target;

            SortDevices();

            RoomStructureChanged?.Invoke(this);
        }

        public void AddNewRoom(string name)
        {
            _rooms.Add(new Room(Guid.NewGuid().ToString(), name));

            SortDevices();

            RoomStructureChanged?.Invoke(this);
        }

        public event Action<IDeviceProvider> RoomStructureChanged;
    }
}
